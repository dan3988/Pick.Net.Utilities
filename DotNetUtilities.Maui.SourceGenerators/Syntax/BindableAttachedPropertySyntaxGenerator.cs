using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private sealed class WritableGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList modifiers;

		internal WritableGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType, SyntaxTokenList modifiers)
			: base(propertyName, propertyType, declaringType, defaultModeExpression, attachedType)
		{
			this.modifiers = modifiers;
		}

		protected override MemberDeclarationSyntax[] GenerateMembers()
		{
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			return new MemberDeclarationSyntax[]
			{
				GenerateBindablePropertyDeclaration(modifiers, bindablePropertyField, nameBindableProperty, nameCreate),
				GenerateAttachedBindablePropertyGetMethod(modifiers, bindablePropertyField),
				GenerateAttachedBindablePropertySetMethod(modifiers, bindablePropertyField)
			};
		}
	}

	private sealed class ReadOnlyGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList getModifiers;
		private readonly SyntaxTokenList setModifiers;

		internal ReadOnlyGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(propertyName, propertyType, declaringType, defaultModeExpression, attachedType)
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
				GenerateBindablePropertyDeclaration(setModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateReadOnly),
				GenerateReadOnlyBindablePropertyDeclaration(getModifiers, bindablePropertyField, bindablePropertyKeyField),
				GenerateAttachedBindablePropertyGetMethod(getModifiers, bindablePropertyField),
				GenerateAttachedBindablePropertySetMethod(setModifiers, bindablePropertyKeyField)
			};
		}
	}

	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttached");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly");

	public static BindableAttachedPropertySyntaxGenerator Create(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(propertyName, propertyType, declaringType, defaultModeExpression, attachedType, getModifiers)
			: new ReadOnlyGenerator(propertyName, propertyType, declaringType, defaultModeExpression, attachedType, getModifiers, setModifiers);
	}

	private readonly TypeSyntax attachedType;

	private BindableAttachedPropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType) : base(propertyName, propertyType, declaringType, defaultModeExpression)
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

		return MethodDeclaration(IdentifierName("void"), "Set" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}
}