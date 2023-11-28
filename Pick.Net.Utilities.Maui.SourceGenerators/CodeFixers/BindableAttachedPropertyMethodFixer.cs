using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableAttachedPropertyMethodFixer : BaseCodeFixProvider<MethodDeclarationSyntax>
{
	public BindableAttachedPropertyMethodFixer() : base(DiagnosticDescriptors.BindablePropertyAttachedMethodToPartial, DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, MethodDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("To partial method", token => DoFix(document, root, node));

	private static Task<Document> DoFix(Document document, SyntaxNode root, MethodDeclarationSyntax node)
	{
		var semicolon = node.SemicolonToken.IsKind(SyntaxKind.SemicolonToken) ? node.SemicolonToken.WithLeadingTrivia(SyntaxTriviaList.Empty) : SyntaxHelper.Semicolon;
		if (node.Body != null)
			semicolon = semicolon.WithTrailingTrivia(node.Body.GetTrailingTrivia());

		var parameterList = node.ParameterList.WithoutTrailingTrivia();
		var fixedNode = node
			.WithParameterList(parameterList)
			.WithBody(null)
			.WithExpressionBody(null)
			.WithSemicolonToken(semicolon);

		if (fixedNode.Modifiers.IndexOf(SyntaxKind.PartialKeyword) < 0)
			fixedNode = fixedNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space));

		root = root.ReplaceNode(node, fixedNode);
		document = document.WithSyntaxRoot(root);

		return Task.FromResult(document);
	}
}
