using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class BindablePropertySyntaxFactory
{
	private static readonly IdentifierNameSyntax nameValue = IdentifierName("value");
	private static readonly IdentifierNameSyntax nameGetValue = IdentifierName("GetValue");
	private static readonly IdentifierNameSyntax nameSetValue = IdentifierName("SetValue");
	private static readonly IdentifierNameSyntax nameBindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	private static readonly IdentifierNameSyntax nameBindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");
	private static readonly IdentifierNameSyntax nameBindablePropertyKeyProperty = IdentifierName("BindableProperty");
	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");
	private static readonly IdentifierNameSyntax nameCreateAttached = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttached");
	private static readonly IdentifierNameSyntax nameCreateAttachedReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly");

	private static FieldDeclarationSyntax GenerateBindablePropertyDeclaration(string propertyName, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, TypeSyntax createMethod, TypeSyntax declaringType, IdentifierNameSyntax propertyType)
	{
		var propertyInitializer = InvocationExpression(createMethod)
			.AddArgumentListLiteralArgument(propertyName)
			.AddArgumentListTypeOfArgument(declaringType)
			.AddArgumentListTypeOfArgument(propertyType)
			.AddArgumentListNullArgument();

		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

		return FieldDeclaration(VariableDeclaration(fieldType).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
	}

	private static FieldDeclarationSyntax GenerateReadOnlyBindablePropertyDeclaration(BindablePropertyEntry entry, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField)
	{
		var propertyInitializer = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, nameBindablePropertyKeyProperty);
		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

		return FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
			.WithModifiers(entry.GetModifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
	}

	private static PropertyDeclarationSyntax GenerateReadOnlyBindablePropertyAccessors(BindablePropertyEntry entry, TypeSyntax propertyType, TypeSyntax bindablePropertyKeyField, TypeSyntax bindablePropertyField)
	{
		return PropertyDeclaration(propertyType, entry.PropertyName)
			.WithModifiers(entry.GetModifiers)
			.AddAccessorListAccessors(
				GenerateGetter(propertyType, bindablePropertyField),
				GenerateSetter(bindablePropertyKeyField, entry.SetModifiers));
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

	private static PropertyDeclarationSyntax GenerateBindablePropertyAccessors(BindablePropertyEntry entry, TypeSyntax propertyType, TypeSyntax bindablePropertyField)
	{
		return PropertyDeclaration(propertyType, entry.PropertyName)
			.WithModifiers(entry.GetModifiers)
			.AddAccessorListAccessors(
				GenerateGetter(propertyType, bindablePropertyField),
				GenerateSetter(bindablePropertyField, entry.SetModifiers));
	}

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(string propertyName, SyntaxTokenList modifiers, TypeSyntax propertyType, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
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

	private static MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(string propertyName, SyntaxTokenList modifiers, IdentifierNameSyntax propertyType, TypeSyntax attachedType, TypeSyntax bindablePropertyField)
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

	public static void GenerateBindablePropertyMembers(ref TypeDeclarationSyntax type, TypeSyntax declaringType, BindablePropertyEntry entry)
		=> type = GenerateBindablePropertyMembers(type, declaringType, entry);

	public static TypeDeclarationSyntax GenerateBindablePropertyMembers(TypeDeclarationSyntax type, TypeSyntax declaringType, BindablePropertyEntry entry)
	{
		var propertyType = IdentifierName(entry.PropertyType);

		if (entry.AttachedType == null)
		{
			if (entry.SetModifiers.Count == 0)
			{
				var	bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
				return type.AddMembers(
					GenerateBindablePropertyDeclaration(entry.PropertyName, entry.GetModifiers, bindablePropertyField, nameBindableProperty, nameCreate, declaringType, propertyType),
					GenerateBindablePropertyAccessors(entry, propertyType, bindablePropertyField));
			}
			else
			{
				var bindablePropertyKeyField = IdentifierName(entry.PropertyName + "PropertyKey");
				var bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
				return type.AddMembers(
					GenerateBindablePropertyDeclaration(entry.PropertyName, entry.SetModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateReadOnly, declaringType, propertyType),
					GenerateReadOnlyBindablePropertyDeclaration(entry, bindablePropertyField, bindablePropertyKeyField),
					GenerateReadOnlyBindablePropertyAccessors(entry, propertyType, bindablePropertyKeyField, bindablePropertyField));
			}
		}
		else
		{
			var attachedType = IdentifierName(entry.AttachedType);

			if (entry.SetModifiers.Count == 0)
			{
				var bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
				return type.AddMembers(
					GenerateBindablePropertyDeclaration(entry.PropertyName, entry.GetModifiers, bindablePropertyField, nameBindableProperty, nameCreateAttached, declaringType, propertyType),
					GenerateAttachedBindablePropertyGetMethod(entry.PropertyName, entry.GetModifiers, propertyType, attachedType, bindablePropertyField),
					GenerateAttachedBindablePropertySetMethod(entry.PropertyName, entry.GetModifiers, propertyType, attachedType, bindablePropertyField));
			}
			else
			{
				var bindablePropertyKeyField = IdentifierName(entry.PropertyName + "PropertyKey");
				var bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
				return type.AddMembers(
					GenerateBindablePropertyDeclaration(entry.PropertyName, entry.SetModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateAttachedReadOnly, declaringType, propertyType),
					GenerateReadOnlyBindablePropertyDeclaration(entry, bindablePropertyField, bindablePropertyKeyField),
					GenerateAttachedBindablePropertyGetMethod(entry.PropertyName, entry.GetModifiers, propertyType, attachedType, bindablePropertyField),
					GenerateAttachedBindablePropertySetMethod(entry.PropertyName, entry.SetModifiers, propertyType, attachedType, bindablePropertyKeyField));
			}
		}
	}
}
