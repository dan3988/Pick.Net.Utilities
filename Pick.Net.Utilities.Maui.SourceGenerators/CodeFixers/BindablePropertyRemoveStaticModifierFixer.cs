using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyRemoveStaticModifierFixer : BaseCodeFixProvider<PropertyDeclarationSyntax>
{
	public BindablePropertyRemoveStaticModifierFixer() : base(DiagnosticDescriptors.BindablePropertyStaticProperty)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, PropertyDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("Remove static modifier", _ => DoFix(document, root, node));

	private static Task<Document> DoFix(Document document, SyntaxNode root, PropertyDeclarationSyntax node)
	{
		var modifiers = node.Modifiers.Remove(SyntaxKind.StaticKeyword);
		var fixedNode = node.WithModifiers(modifiers);

		root = root.ReplaceNode(node, fixedNode);
		document = document.WithSyntaxRoot(root);

		return Task.FromResult(document);
	}
}