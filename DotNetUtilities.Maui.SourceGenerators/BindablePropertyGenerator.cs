using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using DotNetUtilities.Maui.Helpers;
using DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly IdentifierNameSyntax nameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax nameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

	private static readonly string attributeInstanceName = typeof(BindablePropertyAttribute<>).FullName;
	private static readonly string attributeAttachedName = typeof(AttachedBindablePropertyAttribute<,>).FullName;

	private static readonly SyntaxTokenList tokensPublic = CreateTokenList(SyntaxKind.PublicKeyword);

	private static readonly Dictionary<PropertyVisibility, SyntaxTokenList> visibilityTokens = new()
	{
		[PropertyVisibility.Public] = tokensPublic,
		[PropertyVisibility.Protected] = CreateTokenList(SyntaxKind.ProtectedKeyword),
		[PropertyVisibility.Internal] = CreateTokenList(SyntaxKind.InternalKeyword),
		[PropertyVisibility.Private] = CreateTokenList(SyntaxKind.PrivateKeyword),
		[PropertyVisibility.ProtectedInternal] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword),
		[PropertyVisibility.ProtectedPrivate] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword),
	};

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

	private static readonly Dictionary<SpecialType, TypeCode> _specialTypesMap = new()
	{
		[SpecialType.System_Object]		= TypeCode.Object,
		[SpecialType.System_Boolean]	= TypeCode.Boolean,
		[SpecialType.System_Char]		= TypeCode.Char,
		[SpecialType.System_SByte]		= TypeCode.SByte,
		[SpecialType.System_Byte]		= TypeCode.Byte,
		[SpecialType.System_Int16]		= TypeCode.Int16,
		[SpecialType.System_UInt16]		= TypeCode.UInt16,
		[SpecialType.System_Int32]		= TypeCode.Int32,
		[SpecialType.System_UInt32]		= TypeCode.UInt32,
		[SpecialType.System_Int64]		= TypeCode.Int64,
		[SpecialType.System_UInt64]		= TypeCode.UInt64,
		[SpecialType.System_Single]		= TypeCode.Single,
		[SpecialType.System_Double]		= TypeCode.Double,
		[SpecialType.System_Decimal]	= TypeCode.Decimal,
		[SpecialType.System_String]		= TypeCode.String,
		[SpecialType.System_DateTime]	= TypeCode.DateTime
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

	private static void GenerateProperties(SourceProductionContext context, ClassEntry entry, string? suffix)
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
		var fileName = entry.GetFileName(suffix);

		context.AddSource(fileName, unit);
#if DEBUG
		var text = unit.ToFullString();
		Console.WriteLine(fileName);
		Console.WriteLine(text);
#endif
	}

	private static bool ToSyntaxTokens(SyntaxReference? reference, in TypedConstant value, out SyntaxTokenList tokens, [MaybeNullWhen(true)] out Diagnostic diagnostic)
	{
		var level = (PropertyVisibility)Convert.ToInt32(value.Value);

		if (visibilityTokens.TryGetValue(level, out tokens))
		{
			diagnostic = null;
			return true;
		}
		else
		{
			var location = reference == null ? null : Location.Create(reference.SyntaxTree, reference.Span);
			diagnostic = Diagnostic.Create(DiagnosticDescriptors.BindablePropertyInvalidVisibility, location, level);
			return false;
		}
	}

	private static bool TryParseDefaultValue(AttributeData attribute, DiagnosticsBuilder diagnostics, TypedConstant value, INamedTypeSymbol propertyType, ref ExpressionSyntax syntax)
	{
		static bool TryParseCore(AttributeData attribute, DiagnosticsBuilder diagnostics, INamedTypeSymbol propertyType, INamedTypeSymbol realPropertyType, ITypeSymbol? valueType, object? value, ref ExpressionSyntax syntax)
		{
			if (!_specialTypesMap.TryGetValue(propertyType.SpecialType, out var target))
			{
				diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueNotSupported, attribute.ApplicationSyntaxReference, realPropertyType);
				return false;
			}

			syntax = SyntaxHelper.Literal(value, target);
			return true;
		}

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			var typeName = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var member = propertyType.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(v => Equals(v.ConstantValue, value.Value));
			if (member != null)
			{
				syntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(typeName), IdentifierName(member.Name));
				return true;
			}

			var underlyingType = propertyType.EnumUnderlyingType!;
			if (TryParseCore(attribute, diagnostics, underlyingType, propertyType, value.Type, value.Value, ref syntax))
			{
				syntax = CastExpression(IdentifierName(typeName), syntax);
				return true;
			}

			return false;
		}
		else
		{
			return TryParseCore(attribute, diagnostics, propertyType, propertyType, value.Type, value.Value, ref syntax);
		}
	}

	private static bool TryParseAttributePositionalArgs(AttributeData attribute, DiagnosticsBuilder builder, [MaybeNullWhen(false)] out string name)
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

	private static bool TryParseAttributeGenericArgs(AttributeData attribute, [MaybeNullWhen(false)] out IdentifierNameSyntax type, [MaybeNullWhen(false)] out INamedTypeSymbol typeSymbol, out IdentifierNameSyntax? attachedType)
	{
		var attrType = attribute.AttributeClass;
		if (attrType != null && attrType.IsGenericType)
		{
			var args = attrType.TypeArguments;
			if (args.Length >= 1)
			{
				var arg = args[0];
				var name = arg.GetFullTypeName();
				type = IdentifierName(name);
				typeSymbol = (INamedTypeSymbol)arg;
				attachedType = args.Length == 1 ? null : IdentifierName(args[1].GetFullTypeName());
				return true;
			}
		}

		type = null;
		typeSymbol = null;
		attachedType = null;
		return false;
	}

	private static ExpressionSyntax ParseDefaultBindingMode(AttributeData attribute, TypedConstant value, DiagnosticsBuilder diagnostics)
	{
		var enumValue = Enum.ToObject(typeof(BindingMode), value.Value);
		if (!Enum.IsDefined(typeof(BindingMode), enumValue))
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, attribute.ApplicationSyntaxReference, enumValue);
			return CastExpression(nameBindingMode, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)enumValue)));
		}
		else
		{
			return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nameBindingMode, IdentifierName(enumValue.ToString()));
		}
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

	private static ClassEntry MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var type = (INamedTypeSymbol)context.TargetSymbol;
		var parentTypes = GetContainingTypes(type);
		var ns = type.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
		var declaringTypeSyntax = IdentifierName(type.GetFullTypeName());
		var attributes = context.Attributes;
		var properties = ImmutableArray.CreateBuilder<BindablePropertySyntaxGenerator>(attributes.Length);
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		Diagnostic? diagnostic;

		for (var i = 0; i < attributes.Length; i++)
		{
			var attribute = attributes[i];
			if (!TryParseAttributeGenericArgs(attribute, out var propType, out var propTypeSymbol, out var attachedType) || !TryParseAttributePositionalArgs(attribute, diagnostics, out var name))
				continue;

			var getterAccessors = tokensPublic;
			var setterAccessors = default(SyntaxTokenList);

			ExpressionSyntax defaultModeSyntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nameBindingMode, nameBindingModeOneWay);
			ExpressionSyntax defaultValueSyntax = SyntaxHelper.Null;

			foreach (var (key, value) in attribute.NamedArguments)
			{
				switch (key)
				{
					case nameof(BaseBindablePropertyAttribute.DefaultMode):
						defaultModeSyntax = ParseDefaultBindingMode(attribute, value, diagnostics);
						break;
					case nameof(BaseBindablePropertyAttribute.Visibility):
						if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out getterAccessors, out diagnostic))
							diagnostics.Add(diagnostic);

						break;
					case nameof(BaseBindablePropertyAttribute.WriteVisibility):
						if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out setterAccessors, out diagnostic))
							diagnostics.Add(diagnostic);

						break;
					case nameof(BaseBindablePropertyAttribute.DefaultValue):
						TryParseDefaultValue(attribute, diagnostics, value, propTypeSymbol, ref defaultValueSyntax);
						break;
				}
			}

			BindablePropertySyntaxGenerator generator = attachedType == null
				? BindableInstancePropertySyntaxGenerator.Create(name, propType, declaringTypeSyntax, defaultValueSyntax, defaultModeSyntax, getterAccessors, setterAccessors)
				: BindableAttachedPropertySyntaxGenerator.Create(name, propType, declaringTypeSyntax, defaultValueSyntax, defaultModeSyntax, attachedType, getterAccessors, setterAccessors);

			properties.Add(generator);
		}

		return new(ns, type.Name, parentTypes, properties.ToImmutable(), diagnostics.ToImmutable());
	}

	private static bool MetadataPredictate(SyntaxNode node, CancellationToken token)
		=> node.IsKind(SyntaxKind.ClassDeclaration);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var instanceProps = context.SyntaxProvider.ForAttributeWithMetadataName(attributeInstanceName, MetadataPredictate, MetadataTransform);
		var attachedProps = context.SyntaxProvider.ForAttributeWithMetadataName(attributeAttachedName, MetadataPredictate, MetadataTransform);
		context.RegisterSourceOutput(instanceProps, static (context, entry) => GenerateProperties(context, entry, null));
		context.RegisterSourceOutput(attachedProps, static (context, entry) => GenerateProperties(context, entry, "Attached"));
	}
}