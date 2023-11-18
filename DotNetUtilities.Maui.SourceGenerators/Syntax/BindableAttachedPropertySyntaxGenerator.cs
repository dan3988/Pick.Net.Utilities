namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttached");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly");

	public static BindableAttachedPropertySyntaxGenerator Create(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, TypeSyntax attachedType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, attachedType, getModifiers)
			: new ReadOnlyGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, attachedType, getModifiers, setModifiers);
	}

	private readonly TypeSyntax attachedType;

	private BindableAttachedPropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, TypeSyntax attachedType)
		: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory)
	{
		this.attachedType = attachedType;
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var expression = CastExpression(propertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), nameGetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return MethodDeclaration(propertyType, "Get" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj })))
			.WithReturnType(propertyType)
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var paramValue = Parameter(nameValue.Identifier).WithType(propertyType);
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), nameSetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(nameValue));

		return MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));

		method = MethodDeclaration(propertyType, $"Generate{propertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramBindable.WithType(attachedType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
				.AddArgumentListArguments(
					Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))));

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
				paramBindable.WithType(attachedType),
				paramOldValue.WithType(propertyType),
				paramNewValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new ParameterSyntax[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
			.AddArgumentListArguments(
				Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	private sealed class WritableGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList modifiers;

		internal WritableGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, TypeSyntax attachedType, SyntaxTokenList modifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, attachedType)
		{
			this.modifiers = modifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			GenerateBindablePropertyDeclaration(members, modifiers, bindablePropertyField, nameBindableProperty, nameCreate);
			members.Add(GenerateAttachedBindablePropertyGetMethod(modifiers, bindablePropertyField));
			members.Add(GenerateAttachedBindablePropertySetMethod(modifiers, bindablePropertyField));
		}
	}

	private sealed class ReadOnlyGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList getModifiers;
		private readonly SyntaxTokenList setModifiers;

		internal ReadOnlyGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, attachedType)
		{
			this.getModifiers = getModifiers;
			this.setModifiers = setModifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyKeyField = IdentifierName(propertyName + "PropertyKey");
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			GenerateBindablePropertyDeclaration(members, setModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateReadOnly);
			GenerateReadOnlyBindablePropertyDeclaration(members, getModifiers, bindablePropertyField, bindablePropertyKeyField);
			members.Add(GenerateAttachedBindablePropertyGetMethod(getModifiers, bindablePropertyField));
			members.Add(GenerateAttachedBindablePropertySetMethod(setModifiers, bindablePropertyField));
		}
	}
}