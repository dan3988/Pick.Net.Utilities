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

	public TypeSyntax AttachedType { get; }

	public SyntaxTokenList GetMethodModifiers { get; }

	public SyntaxTokenList SetMethodModifiers { get; }

	internal BindableAttachedPropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties, TypeSyntax attachedType, SyntaxTokenList getMethodModifiers, SyntaxTokenList setMethodModifiers) : base(in properties)
	{
		AttachedType = attachedType;
		GetMethodModifiers = getMethodModifiers;
		SetMethodModifiers = setMethodModifiers;
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
		if (GetMethodModifiers.Count > 0)
			members.Add(GenerateAttachedBindablePropertyGetMethod(GetMethodModifiers, AnnotatedPropertyType, PropertyName, AttachedType, propertyField));

		if (SetMethodModifiers.Count > 0)
			members.Add(GenerateAttachedBindablePropertySetMethod(SetMethodModifiers, AnnotatedPropertyType, PropertyName, AttachedType, propertyKeyField));
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));

		method = MethodDeclaration(AnnotatedPropertyType, $"Generate{PropertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramBindable.WithType(AttachedType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
				.AddArgumentListArguments(
					Argument(CastExpression(AttachedType, IdentifierName(paramBindable.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(AnnotatedPropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(AnnotatedPropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(AnnotatedPropertyType, $"Coerce{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(AnnotatedPropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = IdentifierName(method.Identifier);
		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(AnnotatedPropertyType, IdentifierName(paramValue.Identifier)));
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
				paramBindable.WithType(AttachedType),
				paramOldValue.WithType(AnnotatedPropertyType),
				paramNewValue.WithType(AnnotatedPropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
			.AddArgumentListArguments(
				Argument(CastExpression(AttachedType, IdentifierName(paramBindable.Identifier))),
				Argument(CastExpression(AnnotatedPropertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(AnnotatedPropertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}
}