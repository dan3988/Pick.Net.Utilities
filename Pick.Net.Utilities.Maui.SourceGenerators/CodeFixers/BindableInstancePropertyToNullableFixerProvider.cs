using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyToNullableFixerProvider() : BaseCodeFixProvider<PropertyDeclarationSyntax>("Make property type nullable", DiagnosticDescriptors.BindablePropertyDefaultValueNull)
{
	protected override bool Fix(DocumentEditor editor, PropertyDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var ogNode = node;
		var typeAsNullable = SyntaxFactory.NullableType(node.Type);
		var getter = node.AccessorList?.Accessors.FirstOrDefault(v => v.IsKind(SyntaxKind.GetAccessorDeclaration));
		if (getter != null)
		{
			var rewriter = new CastReplacingRewriter(node.Type.WithoutTrivia(), typeAsNullable);
			var replaced = rewriter.Visit(getter);
			node = node.ReplaceNode(getter, replaced);
		}

		editor.ReplaceNode(ogNode, node.WithType(typeAsNullable));
		return true;
	}
}
