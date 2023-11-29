using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyToNullableFixerProvider() : BaseCodeFixProvider(DiagnosticDescriptors.BindablePropertyDefaultValueNull)
{
	protected override CodeAction? CreateAction(Document document, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic)
	{
		return node.Kind() switch
		{
			SyntaxKind.PropertyDeclaration => CodeAction.Create("Make property type nullable", _ => MakePropertyNullable(document, root, (PropertyDeclarationSyntax)node)),
			SyntaxKind.MethodDeclaration => CodeAction.Create("Make property type nullable", token => MakeMethodNullable(document, (MethodDeclarationSyntax)node, token)),
			_ => null,
		};
	}

	private static async Task<Document> MakeMethodNullable(Document document, MethodDeclarationSyntax node, CancellationToken token)
	{
		var editor = await DocumentEditor.CreateAsync(document, token);
		var typeAsNullable = SyntaxFactory.NullableType(node.ReturnType);
		var rewriter = new CastReplacingRewriter(node.ReturnType.WithoutTrivia(), typeAsNullable);

		if (node.ExpressionBody != null)
			editor.RewriteNode(rewriter, node.ExpressionBody);

		if (node.Body != null)
			editor.RewriteNode(rewriter, node.Body);

		editor.ReplaceNode(node.ReturnType, typeAsNullable);

		var symbol = editor.SemanticModel.GetDeclaredSymbol(node);
		if (symbol != null && symbol.Parameters.Length > 0)
		{
			var propertyName = Identifiers.GetAttachedPropertyName(symbol.Name);
			var setter = symbol.ContainingType.GetAttachedSetMethod(symbol.ReturnType, symbol.Parameters[0].Type, propertyName);
			if (setter != null)
			{
				var location = setter.Locations.First();
				var setterNode = (MethodDeclarationSyntax?)node.Parent?.FindNode(location.SourceSpan);
				if (setterNode != null)
				{
					var valueParameter = setterNode.ParameterList.Parameters[1];
					editor.ReplaceNode(valueParameter, valueParameter.WithType(typeAsNullable));
				}
			}
		}

		return editor.GetChangedDocument();
	}

	private static Task<Document> MakePropertyNullable(Document document, SyntaxNode root, PropertyDeclarationSyntax node)
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

		root = root.ReplaceNode(ogNode, node.WithType(typeAsNullable));
		document = document.WithSyntaxRoot(root);

		return Task.FromResult(document);
	}

	private sealed class CastReplacingRewriter(TypeSyntax input, TypeSyntax output) : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitCastExpression(CastExpressionSyntax node)
		{
			if (node.Type.WithoutTrivia().IsEquivalentTo(input))
				node = node.WithType(output);

			return base.VisitCastExpression(node);
		}
	}
}
