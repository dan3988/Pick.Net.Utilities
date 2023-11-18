using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using DotNetUtilities.Maui.SourceGenerators;
using DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators.Generators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

public abstract class BaseBindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly HashSet<TypeCode>?[] _implicitConversionTypes =
	{
		/* TypeCode.Empty = 0		*/ null,
		/* TypeCode.Object = 1		*/ null,
		/* TypeCode.DBNull = 2		*/ null,
		/* TypeCode.Boolean = 3		*/ null,
		/* TypeCode.Char = 4		*/ null,
		/* TypeCode.SByte = 5		*/ null,
		/* TypeCode.Byte = 6		*/ null,
		/* TypeCode.Int16 = 7		*/ Set(TypeCode.SByte, TypeCode.Byte),
		/* TypeCode.UInt16 = 8		*/ Set(TypeCode.Byte),
		/* TypeCode.Int32 = 9		*/ Set(TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16),
		/* TypeCode.UInt32 = 10		*/ Set(TypeCode.Byte, TypeCode.UInt16),
		/* TypeCode.Int64 = 9		*/ Set(TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32),
		/* TypeCode.UInt64 = 10		*/ Set(TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt16, TypeCode.UInt32),
		/* TypeCode.Single = 13		*/ Set(TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64),
		/* TypeCode.Double = 14		*/ Set(TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single),
		/* TypeCode.Decimal = 15	*/ Set(TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double),
		/* TypeCode.DateTime = 16	*/ null,
		/* gap						*/ null,
		/* TypeCode.String = 18		*/ null,
	};

	private static bool CanConvertTo(TypeCode from, TypeCode to)
	{
		if (from == to || to == TypeCode.Object)
			return true;

		var convertible = _implicitConversionTypes[(int)to];
		return convertible != null && convertible.Contains(from);
	}

	private static HashSet<T> Set<T>(params T[] values)
		=> new(values);

	private static SyntaxTokenList CreateTokenList(SyntaxKind kind)
		=> new(Token(kind));

	private static SyntaxTokenList CreateTokenList(params SyntaxKind[] kinds)
		=> new(kinds.Select(Token));

	private static void GenerateProperties(SourceProductionContext context, ClassEntry entry)
	{
		foreach (var diagnostic in entry.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		var type = TypeDeclaration(SyntaxKind.ClassDeclaration, entry.TypeName).AddModifier(SyntaxKind.PartialKeyword);

		foreach (var property in entry.Properties)
			type = property.Generate(type);

		var types = entry.ParentTypes;
		for (var i = 0; i < types.Length; i++)
			type = TypeDeclaration(SyntaxKind.ClassDeclaration, types[i]).AddModifier(SyntaxKind.PartialKeyword).AddMembers(type);

		var ns = NamespaceDeclaration(IdentifierName(entry.Namespace)).AddMembers(type);
		var unit = CompilationUnit().AddMembers(ns).AddFormatting();

		context.AddSource(entry.FileName, unit);
#if DEBUG
		var text = unit.ToFullString();
		Console.WriteLine(entry.FileName);
		Console.WriteLine(text);
#endif
	}

	protected static bool TryParseAttributePositionalArgs(AttributeData attribute, DiagnosticsBuilder builder, [MaybeNullWhen(false)] out string name)
	{
		var arguments = attribute.ConstructorArguments;
		if (arguments.Length < 1)
		{
			name = null;
			return false;
		}

		name = (string?)arguments[0].Value;

		if (string.IsNullOrEmpty(name))
		{
			builder.Add(DiagnosticDescriptors.BindablePropertyEmptyPropertyName, attribute.ApplicationSyntaxReference);
			return false;
		}

#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
		return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
	}

	private static ImmutableArray<string> GetContainingTypes(INamedTypeSymbol type)
	{
		var parent = type.ContainingType;
		if (parent == null)
		{
			return ImmutableArray<string>.Empty;
		}
		else
		{
			var builder = ImmutableArray.CreateBuilder<string>();

			do
			{
				builder.Add(parent.MetadataName);
				parent = parent.ContainingType;
			}
			while (parent != null);

			return builder.ToImmutable();
		}
	}

	private static string GetFileName(string @namespace, string typeName, ImmutableArray<string> parentTypes, string? suffix)
	{
		var sb = new StringBuilder(@namespace).Append('.');

		var types = parentTypes;
		for (var i = types.Length; --i >= 0;)
			sb.Append(types[i]).Append('+');

		sb.Append(typeName);

		if (suffix != null)
			sb.Append('.').Append(suffix);

		return sb.Append(".g.cs").ToString();
	}

	private ClassEntry MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var type = (INamedTypeSymbol)context.TargetSymbol;
		var parentTypes = GetContainingTypes(type);
		var ns = type.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
		var attributes = context.Attributes;
		var properties = ImmutableArray.CreateBuilder<BindablePropertySyntaxGenerator>(attributes.Length);
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		for (var i = 0; i < attributes.Length; i++)
		{
			var attribute = attributes[i];
			var parser = CreateParser(type, attribute, diagnostics);
			if (parser != null)
			{
				parser.ParseNamedArguments(diagnostics);
				var generator = parser.CreateGenerator();
				properties.Add(generator);
			}
		}

		var fileName = GetFileName(ns, type.Name, parentTypes, FileNameSuffix);
		return new(ns, type.Name, fileName, parentTypes, properties.ToImmutable(), diagnostics.ToImmutable());
	}

	private static bool MetadataPredictate(SyntaxNode node, CancellationToken token)
		=> node.IsKind(SyntaxKind.ClassDeclaration);

	public abstract Type AttributeType { get; }

	public virtual string? FileNameSuffix => null;

	public abstract AttributeParser? CreateParser(INamedTypeSymbol declaringType, AttributeData data, DiagnosticsBuilder diagnostics);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataName(AttributeType.FullName, MetadataPredictate, MetadataTransform);
		context.RegisterSourceOutput(info, GenerateProperties);
	}
}