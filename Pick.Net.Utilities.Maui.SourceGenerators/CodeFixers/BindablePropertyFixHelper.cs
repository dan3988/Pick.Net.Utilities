using System.Reflection;

using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

public static class BindablePropertyFixHelper
{
	private static readonly SymbolDisplayFormat FullNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

	public static TypeSyntax GetTypeIdentifier(SemanticModel? model, SyntaxGenerator generator, string fullName)
	{
		if (model == null)
			return IdentifierName("global::" + fullName);

		var typeInfo = model.Compilation.GetTypeByMetadataName(fullName);
		if (typeInfo == null)
			return IdentifierName("global::" + fullName);

		return (TypeSyntax)generator.TypeExpression(typeInfo).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
	}

	public static (AttributeListSyntax List, AttributeSyntax Attribute)? GetAttribute(SemanticModel? model, PropertyDeclarationSyntax prop, CancellationToken token)
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

	public static void RewriteNode(this DocumentEditor editor, CSharpSyntaxRewriter rewriter, SyntaxNode node)
	{
		var changed = rewriter.Visit(node);
		editor.ReplaceNode(node, changed);
	}
}

