using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class BindableInstancePropertyAccessorFixer : CodeFixProvider
{
	public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyInstanceAccessorBody.Id);

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
			if (root!.FindNode(span) is PropertyDeclarationSyntax prop)
			{
				var action = CodeAction.Create("Use BindableProperty", token => UseBindableProperty(document, root, prop, token));
				context.RegisterCodeFix(action, diagnostic);
			}
		}
	}

	private static async Task<Document> UseBindableProperty(Document document, SyntaxNode root, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		var model = await document.GetSemanticModelAsync(token);
		var newProp = BindableInstancePropertyAccessorFixerExecute.FixProperty(model, prop, token);

		root = root.ReplaceNode(prop, newProp);
		return document.WithSyntaxRoot(root);
	}
}

