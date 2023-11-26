using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableAttachedPropertyMethodFixer : CodeFixProvider
{
	public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindableAttachedPropertyNotReferencedInGetMethod.Id, DiagnosticDescriptors.BindableAttachedPropertyNotReferencedInSetMethod.Id);

	public override FixAllProvider? GetFixAllProvider()
		=> WellKnownFixAllProviders.BatchFixer;

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var root = await document.GetSyntaxRootAsync(context.CancellationToken);
		if (root == null)
			return;

		foreach (var diagnostic in context.Diagnostics)
		{
			var span = diagnostic.Location.SourceSpan;
			if (root!.FindNode(span) is MethodDeclarationSyntax method)
			{
				var action = CodeAction.Create("Generate partial method", token => DoFix(document, root, method));
				context.RegisterCodeFix(action, diagnostic);
			}
		}
	}

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
