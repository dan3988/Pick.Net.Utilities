namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal sealed class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	protected override string CreateMethod => "Create";

	protected override string CreateReadOnlyMethod => "CreateReadOnly";

	protected override string CommentFormat => "Bindable {0} for <see cref=\"{1}\"/>.";

	internal BindableInstancePropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties) : base(in properties)
	{
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.WithModifiers(ModifierLists.PrivatePartial)
			.AddParameterListParameters(paramValue.WithType(AnnotatedPropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(AnnotatedPropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(AnnotatedPropertyType, $"Coerce{PropertyName}Value")
			.WithModifiers(ModifierLists.PrivatePartial)
			.AddParameterListParameters(paramValue.WithType(AnnotatedPropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(AnnotatedPropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(MethodSignature signature, out MethodDeclarationSyntax method)
	{
		method = signature.BuildMethod(SyntaxHelper.TypeVoid, [AnnotatedPropertyType, AnnotatedPropertyType]);

		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));
		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)))
			.AddArgumentListArguments(
				Argument(CastExpression(AnnotatedPropertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(AnnotatedPropertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}
}