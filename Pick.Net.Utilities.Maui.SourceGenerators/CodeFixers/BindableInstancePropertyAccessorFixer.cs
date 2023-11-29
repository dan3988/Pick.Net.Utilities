using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyAutoPropertyFixer() : BaseCodeFixProvider<PropertyDeclarationSyntax>(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed)
{
	protected override CodeAction? CreateAction(Document document, SyntaxNode root, PropertyDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("Use BindableProperty in accessors", token => DoFix(document, root, node, token));

	private static async Task<Document> DoFix(Document document, SyntaxNode root, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		var model = await document.GetSemanticModelAsync(token);
		var newProp = BindableInstancePropertyFixes.GeneratePropertyAccessors(model, prop, token);

		root = root.ReplaceNode(prop, newProp);
		return document.WithSyntaxRoot(root);
	}
}
