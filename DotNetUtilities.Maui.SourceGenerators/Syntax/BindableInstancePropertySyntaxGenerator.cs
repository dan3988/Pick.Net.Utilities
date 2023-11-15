using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	public static BindableInstancePropertySyntaxGenerator Create(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(propertyName, propertyType, declaringType, defaultModeExpression, getModifiers)
			: new ReadOnlyGenerator(propertyName, propertyType, declaringType, defaultModeExpression, getModifiers, setModifiers);
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

	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");

	private BindableInstancePropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression) : base(propertyName, propertyType, declaringType, defaultModeExpression)
	{
	}

	private PropertyDeclarationSyntax GenerateBindablePropertyAccessors(SyntaxTokenList getModifiers, SyntaxTokenList setModifiers, TypeSyntax readablePropertyField, TypeSyntax writablePropertyField)
	{
		return PropertyDeclaration(propertyType, propertyName)
			.WithModifiers(getModifiers)
			.AddAccessorListAccessors(
				GenerateGetter(propertyType, readablePropertyField),
				GenerateSetter(writablePropertyField, setModifiers));
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));

		method = MethodDeclaration(SyntaxHelper.VoidType, name)
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

		internal WritableGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, SyntaxTokenList modifiers)
			: base(propertyName, propertyType, declaringType, defaultModeExpression)
		{
			this.modifiers = modifiers;
		}

		protected override MemberDeclarationSyntax[] GenerateMembers()
		{
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			return new MemberDeclarationSyntax[]
			{
				GenerateBindablePropertyDeclaration(modifiers, bindablePropertyField, nameBindableProperty, nameCreate, out var onChanging, out var onChanged),
				GenerateBindablePropertyAccessors(modifiers, default, bindablePropertyField, bindablePropertyField),
				onChanging,
				onChanged,
			};
		}
	}

	private sealed class ReadOnlyGenerator : BindableInstancePropertySyntaxGenerator
	{
		private readonly SyntaxTokenList getModifiers;
		private readonly SyntaxTokenList setModifiers;

		internal ReadOnlyGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(propertyName, propertyType, declaringType, defaultModeExpression)
		{
			this.getModifiers = getModifiers;
			this.setModifiers = setModifiers;
		}

		protected override MemberDeclarationSyntax[] GenerateMembers()
		{
			var bindablePropertyKeyField = IdentifierName(propertyName + "PropertyKey");
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			return new MemberDeclarationSyntax[]
			{
				GenerateBindablePropertyDeclaration(setModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateReadOnly, out var onChanging, out var onChanged),
				GenerateReadOnlyBindablePropertyDeclaration(getModifiers, bindablePropertyField, bindablePropertyKeyField),
				GenerateBindablePropertyAccessors(getModifiers, setModifiers, bindablePropertyField, bindablePropertyKeyField),
				onChanging,
				onChanged
			};
		}
	}
}
