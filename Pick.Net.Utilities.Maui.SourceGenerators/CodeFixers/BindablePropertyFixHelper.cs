using System.Reflection;

using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

internal static class BindablePropertyFixHelper
{
	private static readonly SymbolDisplayFormat FullNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

	public static TypeSyntax GetTypeIdentifier(this SemanticModel? model, SyntaxGenerator generator, string fullName)
	{
		if (model == null)
			return IdentifierName("global::" + fullName);

		var typeInfo = model.Compilation.GetTypeByMetadataName(fullName);
		if (typeInfo == null)
			return IdentifierName("global::" + fullName);

		return (TypeSyntax)generator.TypeExpression(typeInfo).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
	}

	public static AttributeSyntax? GetAttribute(SemanticModel model, MethodDeclarationSyntax method, CancellationToken token)
		=> GetAttribute(model, method.AttributeLists, token);

	public static AttributeSyntax? GetAttribute(SemanticModel model, PropertyDeclarationSyntax prop, CancellationToken token)
		=> GetAttribute(model, prop.AttributeLists, token);

	private static AttributeSyntax? GetAttribute(SemanticModel model, SyntaxList<AttributeListSyntax> attributes, CancellationToken token)
		=> GetAttribute(model, attributes, out int index, token)?.Attributes[index];

	public static AttributeListSyntax? GetAttribute(SemanticModel model, MethodDeclarationSyntax method, out int index, CancellationToken token)
		=> GetAttribute(model, method.AttributeLists, out index, token);

	public static AttributeListSyntax? GetAttribute(SemanticModel model, PropertyDeclarationSyntax prop, out int index, CancellationToken token)
		=> GetAttribute(model, prop.AttributeLists, out index, token);

	private static AttributeListSyntax? GetAttribute(SemanticModel model, SyntaxList<AttributeListSyntax> attributes, out int index, CancellationToken token)
	{
		index = -1;

		if (model == null)
			return null;

		foreach (var attributeList in attributes)
		{
			for (int i = 0; i < attributeList.Attributes.Count; i++)
			{
				var attribute = attributeList.Attributes[i];
				var type = model.GetTypeInfo(attribute, token);
				var name = type.Type?.ToDisplayString(FullNameFormat);
				if (name == "Pick.Net.Utilities.Maui.BindablePropertyAttribute")
				{
					index = i;
					return attributeList;
				}
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

