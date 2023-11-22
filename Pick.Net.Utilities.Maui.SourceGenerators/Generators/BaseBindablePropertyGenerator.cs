using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;
using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

public abstract class BaseBindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly HashSet<TypeCode>?[] ImplicitConversionTypes =
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

		var convertible = ImplicitConversionTypes[(int)to];
		return convertible != null && convertible.Contains(from);
	}

	private static HashSet<T> Set<T>(params T[] values)
		=> new(values);

	private static void GenerateProperties(SourceProductionContext context, BindablePropertyGeneratorEntry entry)
	{
		foreach (var diagnostic in entry.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		var (@namespace, typeName, fileName, _) = entry.ClassInfo;
		var type = TypeDeclaration(SyntaxKind.ClassDeclaration, typeName).AddModifier(SyntaxKind.PartialKeyword);

		type = entry.Properties.Aggregate(type, (current, property) => property.Generate(current));
		type = entry.ClassInfo.ParentTypes.Aggregate(type, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).AddModifier(SyntaxKind.PartialKeyword).AddMembers(current));

		var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
		var ns = NamespaceDeclaration(IdentifierName(@namespace)).AddMembers(type);
		var unit = CompilationUnit()
			.AddMembers(ns)
			.WithLeadingTrivia(Trivia(nullableEnable))
			.AddFormatting();

		context.AddSource(fileName, unit);
#if DEBUG
		var text = unit.ToFullString();
		Console.WriteLine(fileName);
		Console.WriteLine(text);
#endif
	}

	protected static void CheckPropertyTypeNullability(AttributeData attribute, DiagnosticsBuilder diagnostics, ref INamedTypeSymbol symbol)
	{
		if (symbol.NullableAnnotation != NullableAnnotation.Annotated)
			return;

		diagnostics.Add(DiagnosticDescriptors.BindablePropertyNullableValueType, attribute.ApplicationSyntaxReference);
		symbol = (INamedTypeSymbol)(symbol.IsValueType ? symbol.TypeArguments[0] : symbol.WithNullableAnnotation(NullableAnnotation.None));
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

		if (!SyntaxFacts.IsValidIdentifier(name))
		{
			builder.Add(DiagnosticDescriptors.BindablePropertyInvalidPropertyName, attribute.ApplicationSyntaxReference, name);
			return false;
		}

#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
		return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
	}

	private BindablePropertyGeneratorEntry MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var type = (INamedTypeSymbol)context.TargetSymbol;
		var classInfo = ClassInfo.Create(type, FileNameSuffix);
		var attributes = context.Attributes;
		var properties = ImmutableArray.CreateBuilder<BindablePropertySyntaxGenerator>(attributes.Length);
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		foreach (var attribute in attributes)
		{
			var parser = CreateParser(type, attribute, diagnostics);
			if (parser == null)
				continue;

			parser.ParseNamedArguments(diagnostics);
			var generator = parser.CreateGenerator();
			properties.Add(generator);
		}

		return new(classInfo, properties.ToImmutable(), diagnostics.ToImmutable());
	}

	private static bool MetadataPredicate(SyntaxNode node, CancellationToken token)
		=> node.IsKind(SyntaxKind.ClassDeclaration);

	public abstract Type AttributeType { get; }

	public virtual string? FileNameSuffix => null;

	public abstract AttributeParser? CreateParser(INamedTypeSymbol declaringType, AttributeData data, DiagnosticsBuilder diagnostics);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataName(AttributeType.FullName, MetadataPredicate, MetadataTransform);
		context.RegisterSourceOutput(info, GenerateProperties);
	}
}