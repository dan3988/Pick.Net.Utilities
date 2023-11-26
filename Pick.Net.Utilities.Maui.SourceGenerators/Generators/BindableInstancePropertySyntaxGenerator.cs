namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal sealed class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	protected override string CreateMethod => "Create";

	protected override string CreateReadOnlyMethod => "CreateReadOnly";

	internal BindableInstancePropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties) : base(in properties)
	{
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		method = MethodDeclaration(propertyType, $"Generate{PropertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.WithSemicolonToken();

		var paramBindable = Parameter(Identifier("bindable"));
		var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(propertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(TypeSyntax propertyType, TypeSyntax declaringType, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(propertyType, $"Coerce{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(propertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(TypeSyntax propertyType, TypeSyntax declaringType, string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));

		method = MethodDeclaration(SyntaxHelper.TypeVoid, name)
			.AddModifier(SyntaxKind.PartialKeyword)
			.AddParameterListParameters(
				paramOldValue.WithType(propertyType),
				paramNewValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)))
			.AddArgumentListArguments(
				Argument(CastExpression(propertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}
}