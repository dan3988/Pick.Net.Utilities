namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal sealed class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(AttachedPropertyGetterInfo info, TypeSyntax propertyType, string propertyName, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(info.ObjectParamName).WithType(attachedType);
		var expression = CastExpression(propertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(info.ObjectParamName), Identifiers.GetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return MethodDeclaration(propertyType, info.MethodName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj })))
			.WithReturnType(propertyType)
			.WithModifiers(info.Modifiers)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(AttachedPropertySetterInfo info, TypeSyntax propertyType, string propertyName, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(info.ObjectParamName).WithType(attachedType);
		var paramValue = Parameter(info.ValueParamName).WithType(propertyType);
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(info.ObjectParamName), Identifiers.SetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(IdentifierName(info.ValueParamName)));

		return MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(info.Modifiers)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	protected override string CreateMethod => "CreateAttached";

	protected override string CreateReadOnlyMethod => "CreateAttachedReadOnly";

	public TypeSyntax AttachedType { get; }

	public AttachedPropertyGetterInfo? GetMethodGeneration { get; }

	public AttachedPropertySetterInfo? SetMethodGeneration { get; }

	internal BindableAttachedPropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties, TypeSyntax attachedType, AttachedPropertyGetterInfo? getMethodGeneration, AttachedPropertySetterInfo? setMethodGeneration) : base(in properties)
	{
		AttachedType = attachedType;
		GetMethodGeneration = getMethodGeneration;
		SetMethodGeneration = setMethodGeneration;
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
		if (GetMethodGeneration != null)
			members.Add(GenerateAttachedBindablePropertyGetMethod(GetMethodGeneration, AnnotatedPropertyType, PropertyName, AttachedType, propertyField));

		if (SetMethodGeneration != null)
			members.Add(GenerateAttachedBindablePropertySetMethod(SetMethodGeneration, AnnotatedPropertyType, PropertyName, AttachedType, propertyKeyField));
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