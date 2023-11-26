namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal sealed class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameValue = IdentifierName("value");
	private static readonly IdentifierNameSyntax NameGetValue = IdentifierName("GetValue");
	private static readonly IdentifierNameSyntax NameSetValue = IdentifierName("SetValue");

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(SyntaxTokenList modifiers, TypeSyntax propertyType, string propertyName, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var expression = CastExpression(propertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), NameGetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return MethodDeclaration(propertyType, "Get" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj })))
			.WithReturnType(propertyType)
			.WithModifiers(modifiers)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(SyntaxTokenList modifiers, TypeSyntax propertyType, string propertyName, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var paramValue = Parameter(NameValue.Identifier).WithType(propertyType);
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), NameSetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(NameValue));

		return MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(modifiers)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	protected override string CreateMethod => "CreateAttached";

	protected override string CreateReadOnlyMethod => "CreateAttachedReadOnly";

	public ITypeSymbol AttachedType { get; }

	public SyntaxTokenList GetMethodModifiers { get; }

	public SyntaxTokenList SetMethodModifiers { get; }

	internal BindableAttachedPropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties, ITypeSymbol attachedType, SyntaxTokenList getMethodModifiers, SyntaxTokenList setMethodModifiers) : base(in properties)
	{
		AttachedType = attachedType;
		GetMethodModifiers = getMethodModifiers;
		SetMethodModifiers = setMethodModifiers;
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
		if (GetMethodModifiers.Count > 0)
			members.Add(GenerateAttachedBindablePropertyGetMethod(GetMethodModifiers, PropertyType.ToIdentifier(), PropertyName, AttachedType.ToIdentifier(), propertyField));

		if (SetMethodModifiers.Count > 0)
			members.Add(GenerateAttachedBindablePropertySetMethod(SetMethodModifiers, PropertyType.ToIdentifier(), PropertyName, AttachedType.ToIdentifier(), propertyKeyField));
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		var attachedType = AttachedType.ToIdentifier();
		var paramBindable = Parameter(Identifier("bindable"));

		method = MethodDeclaration(propertyType, $"Generate{PropertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramBindable.WithType(attachedType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
				.AddArgumentListArguments(
					Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(propertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(propertyType, $"Coerce{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(propertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(TypeSyntax propertyType, TypeSyntax declaringType, string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));
		var attachedType = AttachedType.ToIdentifier();

		method = MethodDeclaration(SyntaxHelper.TypeVoid, name)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(
				paramBindable.WithType(attachedType),
				paramOldValue.WithType(propertyType),
				paramNewValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
			.AddArgumentListArguments(
				Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}
}