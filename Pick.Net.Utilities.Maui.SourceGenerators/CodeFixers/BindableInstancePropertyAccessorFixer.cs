using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyAutoPropertyFixer : CodeFixProvider
{
	public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyInstanceAutoProperty.Id);

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
				var action = CodeAction.Create("Use BindableProperty in accessors", token => DoFix(document, root, prop, token));
				context.RegisterCodeFix(action, diagnostic);
			}
		}
	}

	private static async Task<Document> DoFix(Document document, SyntaxNode root, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		var model = await document.GetSemanticModelAsync(token);
		var newProp = BindableInstancePropertyFixes.GeneratePropertyAccessors(model, prop, token);

		root = root.ReplaceNode(prop, newProp);
		return document.WithSyntaxRoot(root);
	}
}
