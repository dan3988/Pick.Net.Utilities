using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

public sealed class BindableAttachedPropertyToNullableFixerProvider() : BaseCodeFixProvider<MethodDeclarationSyntax>("Make property type nullable", DiagnosticDescriptors.BindablePropertyDefaultValueNull)
{
	protected override bool Fix(DocumentEditor editor, MethodDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var typeAsNullable = SyntaxFactory.NullableType(node.ReturnType);
		var rewriter = new CastReplacingRewriter(node.ReturnType.WithoutTrivia(), typeAsNullable);

		if (node.ExpressionBody != null)
			editor.RewriteNode(rewriter, node.ExpressionBody);

		if (node.Body != null)
			editor.RewriteNode(rewriter, node.Body);

		editor.ReplaceNode(node.ReturnType, typeAsNullable);

		var symbol = editor.SemanticModel.GetDeclaredSymbol(node);
		if (symbol is not { Parameters.Length: > 0 })
			return true;

		var propertyName = Identifiers.GetAttachedPropertyName(symbol.Name);
		var setter = symbol.ContainingType.GetAttachedSetMethod(symbol.ReturnType, symbol.Parameters[0].Type, propertyName);
		if (setter == null)
			return true;

		var location = setter.Locations.First();
		var setterNode = (MethodDeclarationSyntax?)node.Parent?.FindNode(location.SourceSpan);
		if (setterNode != null)
		{
			var valueParameter = setterNode.ParameterList.Parameters[1];
			editor.ReplaceNode(valueParameter, valueParameter.WithType(typeAsNullable));
		}

		return true;
	}
}
