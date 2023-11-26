using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertySetterFixer : CodeFixProvider
{
	public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyNotReferencedInSetter.Id);

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
			if (root!.FindNode(span) is AccessorDeclarationSyntax prop)
			{
				var action = CodeAction.Create("Generate BindableProperty setter", _ => DoFix(document, root, prop));
				context.RegisterCodeFix(action, diagnostic);
			}
		}
	}

	private static Task<Document> DoFix(Document document, SyntaxNode root, AccessorDeclarationSyntax node)
	{
		var newNode = BindableInstancePropertyFixes.GeneratePropertySetter(node);
		root = root.ReplaceNode(node, newNode);
		document = document.WithSyntaxRoot(root);
		return Task.FromResult(document);
	}
}
