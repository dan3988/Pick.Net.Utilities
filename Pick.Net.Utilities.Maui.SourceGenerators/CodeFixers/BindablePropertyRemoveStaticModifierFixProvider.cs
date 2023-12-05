using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyRemoveStaticModifierFixProvider() : BaseCodeFixProvider<PropertyDeclarationSyntax>("Remove static modifier", DiagnosticDescriptors.BindablePropertyStaticProperty)
{
	protected override bool Fix(DocumentEditor editor, PropertyDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var modifiers = node.Modifiers.Remove(SyntaxKind.StaticKeyword);
		var fixedNode = node.WithModifiers(modifiers);

		editor.ReplaceNode(node, fixedNode);
		return true;
	}
}