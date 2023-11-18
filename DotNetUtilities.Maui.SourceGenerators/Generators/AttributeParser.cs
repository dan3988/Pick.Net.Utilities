using System.Collections.Immutable;

using DotNetUtilities.Maui.Helpers;
using DotNetUtilities.Maui.SourceGenerators;
using DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators.Generators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

public abstract class AttributeParser
{
	private static readonly IdentifierNameSyntax nameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax nameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

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

	private static SyntaxTokenList CreateTokenList(SyntaxKind kind)
		=> new(Token(kind));

	private static SyntaxTokenList CreateTokenList(params SyntaxKind[] kinds)
		=> new(kinds.Select(Token));

	private static bool TryParseVisibility(AttributeData attribute, DiagnosticsBuilder diagnostics, object? value, ref SyntaxTokenList tokens)
	{
		var level = (PropertyVisibility)Convert.ToInt32(value);

		if (visibilityTokens.TryGetValue(level, out var result))
		{
			tokens = result;
			return true;
		}
		else
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidVisibility, attribute.ApplicationSyntaxReference, level);
			return false;
		}
	}

	private static bool TryParseDefaultValue(AttributeData attribute, DiagnosticsBuilder diagnostics, INamedTypeSymbol propertyType, INamedTypeSymbol valueType, object? value, ref ExpressionSyntax syntax)
	{
		static bool TryParseCore(AttributeData attribute, DiagnosticsBuilder diagnostics, INamedTypeSymbol propertyType, INamedTypeSymbol realPropertyType, ITypeSymbol? valueType, object? value, ref ExpressionSyntax syntax)
		{
			if (!propertyType.SpecialType.TryGetTypeCode(out var target))
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
			var member = propertyType.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(v => Equals(v.ConstantValue, value));
			if (member != null)
			{
				syntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(typeName), IdentifierName(member.Name));
				return true;
			}

			var underlyingType = propertyType.EnumUnderlyingType!;
			if (TryParseCore(attribute, diagnostics, underlyingType, propertyType, valueType, value, ref syntax))
			{
				syntax = CastExpression(IdentifierName(typeName), syntax);
				return true;
			}

			return false;
		}
		else
		{
			return TryParseCore(attribute, diagnostics, propertyType, propertyType, valueType, value, ref syntax);
		}
	}

	private static ExpressionSyntax ParseDefaultBindingMode(AttributeData attribute, object? value, DiagnosticsBuilder diagnostics)
	{
		var enumValue = Enum.ToObject(typeof(BindingMode), value);
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

	public AttributeData AttributeData { get; }

	public string PropertyName { get; }

	public INamedTypeSymbol PropertyType { get; }

	public INamedTypeSymbol DeclaringType { get; }

	private SyntaxTokenList getterAccessors = tokensPublic;
	protected SyntaxTokenList GetterAccessors => getterAccessors;

	private SyntaxTokenList setterAccessors;
	protected SyntaxTokenList SetterAccessors => setterAccessors;

	private ExpressionSyntax defaultModeSyntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nameBindingMode, nameBindingModeOneWay);
	protected ExpressionSyntax DefaultModeSyntax => defaultModeSyntax;

	private ExpressionSyntax defaultValueSyntax = SyntaxHelper.Null;
	protected ExpressionSyntax DefaultValueSyntax => defaultValueSyntax;

	private bool defaultValueFactory = false;
	protected bool DefaultValueFactory => defaultValueFactory;

	protected AttributeParser(INamedTypeSymbol declaringType, AttributeData attribute, string name, INamedTypeSymbol propertyType)
	{
		DeclaringType = declaringType;
		AttributeData = attribute;
		PropertyName = name;
		PropertyType = propertyType;
	}

	internal void ParseNamedArguments(DiagnosticsBuilder diagnostics)
	{
		foreach (var (key, value) in AttributeData.NamedArguments)
			TryParseNamedArgument(diagnostics, key, (INamedTypeSymbol)value.Type!, value.Value);

		if (!DefaultValueSyntax.IsKind(SyntaxKind.NullLiteralExpression) && defaultValueFactory)
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory, AttributeData.ApplicationSyntaxReference);
	}

	protected virtual bool TryParseNamedArgument(DiagnosticsBuilder diagnostics, string key, INamedTypeSymbol type, object? value)
	{
		switch (key)
		{
			case nameof(BaseBindablePropertyAttribute.DefaultMode):
				defaultModeSyntax = ParseDefaultBindingMode(AttributeData, value, diagnostics);
				return true;
			case nameof(BaseBindablePropertyAttribute.Visibility):
				TryParseVisibility(AttributeData, diagnostics, value, ref getterAccessors);
				return true;
			case nameof(BaseBindablePropertyAttribute.WriteVisibility):
				TryParseVisibility(AttributeData, diagnostics, value, ref setterAccessors);
				return true;
			case nameof(BaseBindablePropertyAttribute.DefaultValue):
				TryParseDefaultValue(AttributeData, diagnostics, PropertyType, type, value, ref defaultValueSyntax);
				return true;
			case nameof(BaseBindablePropertyAttribute.DefaultValueFactory):
				defaultValueFactory = (bool)value!;
				return true;
		}

		return false;
	}

	internal abstract BindablePropertySyntaxGenerator CreateGenerator();
}
