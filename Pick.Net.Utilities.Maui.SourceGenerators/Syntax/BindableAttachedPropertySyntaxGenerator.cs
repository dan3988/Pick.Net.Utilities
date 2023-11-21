namespace Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

using static SyntaxFactory;

internal abstract class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttached");
	private static readonly IdentifierNameSyntax NameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly");

	public static BindableAttachedPropertySyntaxGenerator Create(in BindablePropertySyntaxGeneratorConstructorParameters values, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(in values, attachedType, getModifiers)
			: new ReadOnlyGenerator(in values, attachedType, getModifiers, setModifiers);
	}

	private readonly TypeSyntax _attachedType;

	private BindableAttachedPropertySyntaxGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values, TypeSyntax attachedType)
		: base(in values)
	{
		_attachedType = attachedType;
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(_attachedType);
		var expression = CastExpression(PropertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), NameGetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return MethodDeclaration(PropertyType, "Get" + PropertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj })))
			.WithReturnType(PropertyType)
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(_attachedType);
		var paramValue = Parameter(NameValue.Identifier).WithType(PropertyType);
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), NameSetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(NameValue));

		return MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + PropertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));

		method = MethodDeclaration(PropertyType, $"Generate{PropertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramBindable.WithType(_attachedType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
				.AddArgumentListArguments(
					Argument(CastExpression(_attachedType, IdentifierName(paramBindable.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(PropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(PropertyType, $"Coerce{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(PropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));

		method = MethodDeclaration(SyntaxHelper.TypeVoid, name)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(
				paramBindable.WithType(_attachedType),
				paramOldValue.WithType(PropertyType),
				paramNewValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
			.AddArgumentListArguments(
				Argument(CastExpression(_attachedType, IdentifierName(paramBindable.Identifier))),
				Argument(CastExpression(PropertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(PropertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	private sealed class WritableGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList _modifiers;

		internal WritableGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values, TypeSyntax attachedType, SyntaxTokenList modifiers)
			: base(in values, attachedType)
		{
			_modifiers = modifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyField = IdentifierName(PropertyName + "Property");
			GenerateBindablePropertyDeclaration(members, _modifiers, bindablePropertyField, NameBindableProperty, NameCreate);
			members.Add(GenerateAttachedBindablePropertyGetMethod(_modifiers, bindablePropertyField));
			members.Add(GenerateAttachedBindablePropertySetMethod(_modifiers, bindablePropertyField));
		}
	}

	private sealed class ReadOnlyGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList _getModifiers;
		private readonly SyntaxTokenList _setModifiers;

		internal ReadOnlyGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(in values, attachedType)
		{
			_getModifiers = getModifiers;
			_setModifiers = setModifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyKeyField = IdentifierName(PropertyName + "PropertyKey");
			var bindablePropertyField = IdentifierName(PropertyName + "Property");
			GenerateBindablePropertyDeclaration(members, _setModifiers, bindablePropertyKeyField, NameBindablePropertyKey, NameCreateReadOnly);
			GenerateReadOnlyBindablePropertyDeclaration(members, _getModifiers, bindablePropertyField, bindablePropertyKeyField);
			members.Add(GenerateAttachedBindablePropertyGetMethod(_getModifiers, bindablePropertyField));
			members.Add(GenerateAttachedBindablePropertySetMethod(_setModifiers, bindablePropertyField));
		}
	}
}