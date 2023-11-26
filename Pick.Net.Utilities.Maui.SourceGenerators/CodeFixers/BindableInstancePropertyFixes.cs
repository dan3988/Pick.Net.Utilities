﻿using System.Reflection;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

public static class BindableInstancePropertyFixes
{
	private static readonly IdentifierNameSyntax NameValue = IdentifierName("value");
	private static readonly IdentifierNameSyntax NameGetValue = IdentifierName("GetValue");
	private static readonly IdentifierNameSyntax NameSetValue = IdentifierName("SetValue");

	private static readonly SymbolDisplayFormat FullNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

	private static AccessorDeclarationSyntax GenerateGetter(AccessorDeclarationSyntax node, TypeSyntax propertyType, TypeSyntax bindablePropertyField)
	{
		var expression = CastExpression(propertyType, InvocationExpression(NameGetValue).AddArgumentListArguments(Argument(bindablePropertyField)));
		return node.WithExpressionBody(ArrowExpressionClause(expression));
	}

	private static AccessorDeclarationSyntax GenerateSetter(AccessorDeclarationSyntax node, TypeSyntax bindablePropertyField)
	{
		var expression = InvocationExpression(NameSetValue)
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(NameValue));

		return node.WithExpressionBody(ArrowExpressionClause(expression));
	}

	internal static AccessorDeclarationSyntax GeneratePropertyGetter(AccessorDeclarationSyntax node)
	{
		if (node.Parent is AccessorListSyntax { Parent: PropertyDeclarationSyntax prop })
		{
			var field = IdentifierName(prop.Identifier.Text + "Property");
			node = GenerateGetter(node, prop.Type, field);
		}

		return node;
	}

	internal static AccessorDeclarationSyntax GeneratePropertySetter(AccessorDeclarationSyntax node)
	{
		if (node.Parent is AccessorListSyntax { Parent: PropertyDeclarationSyntax prop })
		{
			var suffix = node.Modifiers.Count == 0 ? "Property" : "PropertyKey";
			var field = IdentifierName(prop.Identifier.Text + suffix);
			node = GenerateSetter(node, field);
		}

		return node;
	}

	internal static PropertyDeclarationSyntax GeneratePropertyAccessors(SemanticModel? model, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		var field = IdentifierName(prop.Identifier.Text + "Property");
		var accessors = new List<AccessorDeclarationSyntax>(2);

		var getter = prop.AccessorList?.Accessors.FirstOrDefault(v => v.IsKind(SyntaxKind.GetAccessorDeclaration));
		if (getter != null)
		{
			var accessor = GenerateGetter(getter, prop.Type, field);
			accessors.Add(accessor);
		}

		var setter = prop.AccessorList?.Accessors.FirstOrDefault(v => v.IsKind(SyntaxKind.SetAccessorDeclaration));
		if (setter != null)
		{
			var keyField = setter.Modifiers.Count == 0 ? field : IdentifierName(prop.Identifier.Text + "PropertyKey");
			var accessor = GenerateSetter(setter, keyField);
			accessors.Add(accessor);
		}

		MoveInitializerToAttribute(ref prop, model, token);

		return prop.WithAccessorList(AccessorList(new(accessors))).WithTrailingLineBreak();
	}

	private static void MoveInitializerToAttribute(ref PropertyDeclarationSyntax prop, SemanticModel? model, CancellationToken token)
	{
		if (prop.Initializer == null)
			return;

		var bp = GetAttribute(model, prop, token);
		if (bp == null)
			return;

		var (list, attr) = bp.Value;
		var nameEquals = NameEquals(nameof(BindablePropertyAttribute.DefaultValue));
		var argument = AttributeArgument(nameEquals, null, prop.Initializer.Value);
		var argList = attr.ArgumentList;
		if (argList == null)
		{
			argList = AttributeArgumentList(SingletonSeparatedList(argument));
		}
		else
		{
			var arguments = argList.Arguments;
			var existingArg = arguments.FirstOrDefault(v => v.NameEquals?.Name.Identifier.Text == nameof(BindablePropertyAttribute.DefaultValue));

			arguments = existingArg != null ? arguments.Replace(existingArg, argument) : arguments.Insert(0, argument);
			argList = argList.WithArguments(arguments);
		}

		prop = prop
			.WithAttributeLists(prop.AttributeLists.Replace(list, list.ReplaceNode(attr, attr.WithArgumentList(argList))))
			.WithInitializer(null)
			.WithSemicolonToken(default);
	}

	private static (AttributeListSyntax, AttributeSyntax)? GetAttribute(SemanticModel? model, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		if (model == null)
			return null;

		foreach (var attributeList in prop.AttributeLists)
		{
			foreach (var attribute in attributeList.Attributes)
			{
				var type = model.GetTypeInfo(attribute, token);
				var name = type.Type?.ToDisplayString(FullNameFormat);
				if (name == "Pick.Net.Utilities.Maui.Helpers.BindablePropertyAttribute")
					return (attributeList, attribute);
			}
		}

		return null;
	}
}

