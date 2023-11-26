using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyGetterFixer : BaseCodeFixProvider<AccessorDeclarationSyntax>
{
	public BindableInstancePropertyGetterFixer() : base(DiagnosticDescriptors.BindablePropertyNotReferencedInGetter)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, AccessorDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("Generate BindableProperty getter", _ => DoFix(document, root, node));

	private static Task<Document> DoFix(Document document, SyntaxNode root, AccessorDeclarationSyntax node)
	{
		var newNode = BindableInstancePropertyFixes.GeneratePropertyGetter(node);
		root = root.ReplaceNode(node, newNode);
		document = document.WithSyntaxRoot(root);
		return Task.FromResult(document);
	}
}
