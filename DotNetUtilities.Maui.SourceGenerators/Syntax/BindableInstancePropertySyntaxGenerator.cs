namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using System.Collections.Generic;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");

	public static BindableInstancePropertySyntaxGenerator Create(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, getModifiers)
			: new ReadOnlyGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory, getModifiers, setModifiers);
	}

	private static AccessorDeclarationSyntax GenerateGetter(TypeSyntax propertyType, TypeSyntax bindablePropertyField)
	{
		var expression = CastExpression(propertyType, InvocationExpression(nameGetValue).AddArgumentListArguments(Argument(bindablePropertyField)));

		return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private static AccessorDeclarationSyntax GenerateSetter(TypeSyntax bindablePropertyField, SyntaxTokenList accessors)
	{
		var expression = InvocationExpression(nameSetValue)
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(nameValue));

		return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
			.WithModifiers(accessors)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private BindableInstancePropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory)
		: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory)
	{
	}

	private void GenerateBindablePropertyAccessors(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers, TypeSyntax readablePropertyField, TypeSyntax writablePropertyField)
	{
		var prop = PropertyDeclaration(propertyType, propertyName)
			.WithModifiers(getModifiers)
			.AddAccessorListAccessors(
				GenerateGetter(propertyType, readablePropertyField),
				GenerateSetter(writablePropertyField, setModifiers));

		members.Add(prop);
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method)
	{
		method = MethodDeclaration(propertyType, $"Generate{propertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.WithSemicolonToken();

		var paramBindable = Parameter(Identifier("bindable"));
		var parameters = ParameterList(SeparatedList(new[] { paramBindable}));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method)
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

		var parameters = ParameterList(SeparatedList(new ParameterSyntax[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)))
			.AddArgumentListArguments(
				Argument(CastExpression(propertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	private sealed class WritableGenerator : BindableInstancePropertySyntaxGenerator
	{
		private readonly SyntaxTokenList modifiers;

		internal WritableGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, SyntaxTokenList modifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory)
		{
			this.modifiers = modifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			GenerateBindablePropertyDeclaration(members, modifiers, bindablePropertyField, nameBindableProperty, nameCreate);
			GenerateBindablePropertyAccessors(members, modifiers, default, bindablePropertyField, bindablePropertyField);
		}
	}

	private sealed class ReadOnlyGenerator : BindableInstancePropertySyntaxGenerator
	{
		private readonly SyntaxTokenList getModifiers;
		private readonly SyntaxTokenList setModifiers;

		internal ReadOnlyGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, bool defaultValueFactory, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory)
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
			GenerateBindablePropertyAccessors(members, getModifiers, setModifiers, bindablePropertyField, bindablePropertyKeyField);
		}
	}
}
