using System.Collections.Immutable;

using Pick.Net.Utilities.Maui.Helpers;
using Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;
using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

public abstract class AttributeParser
{
	private static readonly IdentifierNameSyntax NameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax NameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

	private static readonly SyntaxTokenList TokensPublic = CreateTokenList(SyntaxKind.PublicKeyword);

	private static readonly Dictionary<PropertyVisibility, SyntaxTokenList> VisibilityTokens = new()
	{
		[PropertyVisibility.Public] = TokensPublic,
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

		if (VisibilityTokens.TryGetValue(level, out var result))
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

	private static bool TryParseDefaultValue(AttributeData attribute, DiagnosticsBuilder diagnostics, INamedTypeSymbol propertyType, object? value, ref ExpressionSyntax syntax)
	{
		static bool TryParseCore(AttributeData attribute, DiagnosticsBuilder diagnostics, ITypeSymbol propertyType, INamedTypeSymbol realPropertyType, object? value, ref ExpressionSyntax syntax)
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
			if (TryParseCore(attribute, diagnostics, underlyingType, propertyType, value, ref syntax))
			{
				syntax = CastExpression(IdentifierName(typeName), syntax);
				return true;
			}

			return false;
		}
		else
		{
			return TryParseCore(attribute, diagnostics, propertyType, propertyType, value, ref syntax);
		}
	}

	private static ExpressionSyntax ParseDefaultBindingMode(AttributeData attribute, object? value, DiagnosticsBuilder diagnostics)
	{
		var enumValue = Enum.ToObject(typeof(BindingMode), value);
		if (!Enum.IsDefined(typeof(BindingMode), enumValue))
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, attribute.ApplicationSyntaxReference, enumValue);
			return CastExpression(NameBindingMode, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)enumValue)));
		}
		else
		{
			return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, IdentifierName(enumValue.ToString()));
		}
	}

	public AttributeData AttributeData { get; }

	public string PropertyName { get; }

	public INamedTypeSymbol PropertyType { get; }

	public INamedTypeSymbol DeclaringType { get; }

	private SyntaxTokenList _getterAccessors = TokensPublic;
	protected SyntaxTokenList GetterAccessors => _getterAccessors;

	private SyntaxTokenList _setterAccessors;
	protected SyntaxTokenList SetterAccessors => _setterAccessors;

	private ExpressionSyntax _defaultModeSyntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, NameBindingModeOneWay);
	private ExpressionSyntax _defaultValueSyntax = SyntaxHelper.Null;
	private bool? _defaultValueFactory;
	private bool? _coerceValueCallback;
	private bool? _validateValueCallback;
	private bool? _isNullable;

	protected AttributeParser(INamedTypeSymbol declaringType, AttributeData attribute, string name, INamedTypeSymbol propertyType)
	{
		DeclaringType = declaringType;
		AttributeData = attribute;
		PropertyName = name;
		PropertyType = propertyType;
	}

	internal BindablePropertySyntaxGeneratorConstructorParameters CreateParameters()
	{
		var propType = (TypeSyntax)PropertyType.ToIdentifier();
		var propTypeUnannotated = propType;
		if (_isNullable == true)
		{
			propType = propType.AsNullable();

			if (PropertyType.IsValueType)
				propTypeUnannotated = propType;
		}

		return new(PropertyName, propType, propTypeUnannotated, DeclaringType.ToIdentifier(), _defaultValueSyntax, _defaultModeSyntax, _defaultValueFactory ?? false, _coerceValueCallback ?? false, _validateValueCallback ?? false);
	}

	internal void ParseNamedArguments(DiagnosticsBuilder diagnostics)
	{
		foreach (var (key, value) in AttributeData.NamedArguments)
			TryParseNamedArgument(diagnostics, key, (INamedTypeSymbol)value.Type!, value.Value);

		var hasDefaultValue = !_defaultValueSyntax.IsKind(SyntaxKind.NullLiteralExpression);
		var hasDefaultValueFactory = _defaultValueFactory == true;

		if (hasDefaultValue)
		{
			if (hasDefaultValueFactory)
			{
				diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory, AttributeData.ApplicationSyntaxReference);
			}
		}
		else if (!hasDefaultValueFactory)
		{
			if (_isNullable != true && !PropertyType.IsValueType)
			{
				diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueNull, AttributeData.ApplicationSyntaxReference, PropertyName);
			}
		}
	}

	protected virtual bool TryParseNamedArgument(DiagnosticsBuilder diagnostics, string key, INamedTypeSymbol type, object? value)
	{
		switch (key)
		{
			case nameof(BaseBindablePropertyAttribute.DefaultMode):
				_defaultModeSyntax = ParseDefaultBindingMode(AttributeData, value, diagnostics);
				return true;
			case nameof(BaseBindablePropertyAttribute.Visibility):
				TryParseVisibility(AttributeData, diagnostics, value, ref _getterAccessors);
				return true;
			case nameof(BaseBindablePropertyAttribute.WriteVisibility):
				TryParseVisibility(AttributeData, diagnostics, value, ref _setterAccessors);
				return true;
			case nameof(BaseBindablePropertyAttribute.DefaultValue):
				TryParseDefaultValue(AttributeData, diagnostics, PropertyType, value, ref _defaultValueSyntax);
				return true;
			case nameof(BaseBindablePropertyAttribute.IsNullable):
				_isNullable = (bool?)value;
				return true;
			case nameof(BaseBindablePropertyAttribute.DefaultValueFactory):
				_defaultValueFactory = (bool?)value;
				return true;
			case nameof(BaseBindablePropertyAttribute.CoerceValueCallback):
				_coerceValueCallback = (bool?)value;
				return true;
			case nameof(BaseBindablePropertyAttribute.ValidateValueCallback):
				_validateValueCallback = (bool?)value;
				return true;
		}

		return false;
	}

	internal abstract BindablePropertySyntaxGenerator CreateGenerator();
}
