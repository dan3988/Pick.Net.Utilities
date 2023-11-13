using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindablePropertySyntaxGenerator
{
	protected static readonly IdentifierNameSyntax nameValue = IdentifierName("value");
	protected static readonly IdentifierNameSyntax nameGetValue = IdentifierName("GetValue");
	protected static readonly IdentifierNameSyntax nameSetValue = IdentifierName("SetValue");
	protected static readonly IdentifierNameSyntax nameBindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	protected static readonly IdentifierNameSyntax nameBindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	private static readonly IdentifierNameSyntax nameBindablePropertyKeyProperty = IdentifierName("BindableProperty");

	protected static FieldDeclarationSyntax GenerateReadOnlyBindablePropertyDeclaration(SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField)
	{
		var propertyInitializer = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, nameBindablePropertyKeyProperty);
		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

		return FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
	}

	protected readonly string propertyName;
	protected readonly TypeSyntax propertyType;
	protected readonly TypeSyntax declaringType;
	protected readonly ExpressionSyntax defaultModeExpression;

	protected BindablePropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultModeExpression)
	{
		this.propertyName = propertyName;
		this.propertyType = propertyType;
		this.declaringType = declaringType;
		this.defaultModeExpression = defaultModeExpression;
	}

	public TypeDeclarationSyntax Generate(TypeDeclarationSyntax syntax)
		=> syntax.AddMembers(GenerateMembers());

	protected abstract MemberDeclarationSyntax[] GenerateMembers();

	protected FieldDeclarationSyntax GenerateBindablePropertyDeclaration(SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, TypeSyntax createMethod)
	{
		var propertyInitializer = InvocationExpression(createMethod)
			.AddArgumentListLiteralArgument(propertyName)
			.AddArgumentListTypeOfArgument(propertyType)
			.AddArgumentListTypeOfArgument(declaringType)
			.AddArgumentListNullArgument()
			.AddArgumentListArguments(Argument(defaultModeExpression));

		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

		return FieldDeclaration(VariableDeclaration(fieldType).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
	}
}
