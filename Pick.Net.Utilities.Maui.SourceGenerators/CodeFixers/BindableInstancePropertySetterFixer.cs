using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertySetterFixer : BaseCodeFixProvider<AccessorDeclarationSyntax>
{
	public BindableInstancePropertySetterFixer() : base(DiagnosticDescriptors.BindablePropertyNotReferencedInSetter)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, AccessorDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("Generate BindableProperty setter", _ => DoFix(document, root, node));

	private static Task<Document> DoFix(Document document, SyntaxNode root, AccessorDeclarationSyntax node)
	{
		var newNode = BindableInstancePropertyFixes.GeneratePropertySetter(node);
		root = root.ReplaceNode(node, newNode);
		document = document.WithSyntaxRoot(root);
		return Task.FromResult(document);
	}
}
