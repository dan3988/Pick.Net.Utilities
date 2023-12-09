namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal sealed class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(MethodSignature signature, TypeSyntax propertyType, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var expression = CastExpression(propertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(signature.ParameterNames[0]), Identifiers.GetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return signature.BuildMethod(propertyType, [attachedType], ArrowExpressionClause(expression));
	}

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(MethodSignature signature, TypeSyntax propertyType, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
	{
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(signature.ParameterNames[0]), Identifiers.SetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(IdentifierName(signature.ParameterNames[1])));

		return signature.BuildMethod(SyntaxHelper.TypeVoid, [attachedType, propertyType], ArrowExpressionClause(expression));
	}

	protected override string CreateMethod => "CreateAttached";

	protected override string CreateReadOnlyMethod => "CreateAttachedReadOnly";

	protected override string CommentFormat => "Bindable {0} for the attached property <c>{1}</c>.";

	public TypeSyntax AttachedType { get; }

	public MethodSignature? GetMethodSignature { get; }

	public MethodSignature? SetMethodSignature { get; }

	internal BindableAttachedPropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties, TypeSyntax attachedType, MethodSignature? getMethodSignature, MethodSignature? setMethodSignature) : base(in properties)
	{
		AttachedType = attachedType;
		GetMethodSignature = getMethodSignature;
		SetMethodSignature = setMethodSignature;
	}

	protected override void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField)
	{
		if (GetMethodSignature != null)
			members.Add(GenerateAttachedBindablePropertyGetMethod(GetMethodSignature, AnnotatedPropertyType, AttachedType, propertyField));

		if (SetMethodSignature != null)
			members.Add(GenerateAttachedBindablePropertySetMethod(SetMethodSignature, AnnotatedPropertyType, AttachedType, propertyKeyField));
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.WithModifiers(ModifierLists.PrivateStaticPartial)
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
			.WithModifiers(ModifierLists.PrivateStaticPartial)
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
			.WithModifiers(ModifierLists.StaticPartial)
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