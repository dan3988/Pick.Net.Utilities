using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableAttachedPropertyMethodToPartialFixProvider() : BaseCodeFixProvider<MethodDeclarationSyntax>(DiagnosticDescriptors.BindablePropertyAttachedMethodToPartial, DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed)
{
	protected override CodeAction? CreateAction(Document document, SyntaxNode root, MethodDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("To partial method", token => DoFix(document, root, node, token));

	private static MethodDeclarationSyntax ToPartial(MethodDeclarationSyntax node)
	{
		var semicolon = node.SemicolonToken.IsKind(SyntaxKind.SemicolonToken) ? node.SemicolonToken.WithLeadingTrivia(SyntaxTriviaList.Empty) : SyntaxHelper.Semicolon;
		if (node.Body != null)
			semicolon = semicolon.WithTrailingTrivia(node.Body.GetTrailingTrivia());

		var parameterList = node.ParameterList.WithoutTrailingTrivia();
		node = node
			.WithParameterList(parameterList)
			.WithBody(null)
			.WithExpressionBody(null)
			.WithSemicolonToken(semicolon);

		if (node.Modifiers.IndexOf(SyntaxKind.PartialKeyword) < 0)
			node = node.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space));

		return node;
	}

	private static async Task<Document> DoFix(Document document, SyntaxNode root, MethodDeclarationSyntax node, CancellationToken token)
	{
		var propertyName = Identifiers.GetAttachedPropertyName(node.Identifier.Text);
		var editor = await DocumentEditor.CreateAsync(document, token);

		editor.ReplaceNode(node, ToPartial(node));

		var getterSymbol = editor.SemanticModel.GetDeclaredSymbol(node);
		var setter = getterSymbol?.ContainingType.GetAttachedSetMethod(getterSymbol.ReturnType, getterSymbol.Parameters[0].Type, propertyName);
		if (setter != null)
		{
			var location = setter.Locations.First();
			var setterNode = (MethodDeclarationSyntax?)node.Parent?.FindNode(location.SourceSpan);
			if (setterNode != null)
				editor.ReplaceNode(setterNode, ToPartial(setterNode));
		}

		return editor.GetChangedDocument();
	}
}
