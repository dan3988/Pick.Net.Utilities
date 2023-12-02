namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

internal sealed class CastReplacingRewriter(TypeSyntax input, TypeSyntax output) : CSharpSyntaxRewriter
{
	public override SyntaxNode? VisitCastExpression(CastExpressionSyntax node)
	{
		if (node.Type.WithoutTrivia().IsEquivalentTo(input))
			node = node.WithType(output);

		return base.VisitCastExpression(node);
	}
}