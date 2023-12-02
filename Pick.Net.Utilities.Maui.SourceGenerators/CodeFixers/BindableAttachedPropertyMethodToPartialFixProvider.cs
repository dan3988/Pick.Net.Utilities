using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableAttachedPropertyMethodToPartialFixProvider() : BaseCodeFixProvider<MethodDeclarationSyntax>("To partial method", DiagnosticDescriptors.BindablePropertyAttachedMethodToPartial, DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed)
{
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

	protected override bool Fix(DocumentEditor editor, MethodDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var propertyName = Identifiers.GetAttachedPropertyName(node.Identifier.Text);
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

		return true;
	}
}
