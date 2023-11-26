using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableAttachedPropertyMethodFixer : BaseCodeFixProvider<MethodDeclarationSyntax>
{
	public BindableAttachedPropertyMethodFixer() : base(DiagnosticDescriptors.BindableAttachedPropertyNotReferencedInGetMethod, DiagnosticDescriptors.BindableAttachedPropertyNotReferencedInSetMethod)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, MethodDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("Generate partial method", token => DoFix(document, root, node));

	private static Task<Document> DoFix(Document document, SyntaxNode root, MethodDeclarationSyntax node)
	{
		var fixedNode = node
			.WithBody(null)
			.WithExpressionBody(null)
			.WithSemicolonToken();

		if (fixedNode.Modifiers.IndexOf(SyntaxKind.PartialKeyword) < 0)
			fixedNode = fixedNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space));

		root = root.ReplaceNode(node, fixedNode);
		document = document.WithSyntaxRoot(root);

		return Task.FromResult(document);
	}
}
