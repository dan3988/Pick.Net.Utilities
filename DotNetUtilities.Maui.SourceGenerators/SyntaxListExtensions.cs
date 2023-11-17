namespace DotNetUtilities.Maui.SourceGenerators;

internal static class SyntaxListExtensions
{
	public static SyntaxList<V> Select<T, V>(this SyntaxList<T> list, Func<T, V> selector)
		where T : SyntaxNode
		where V : SyntaxNode
	{
		var selected = Enumerable.Select(list, selector);
		return new(selected);
	}

	public static SeparatedSyntaxList<V> Select<T, V>(this SeparatedSyntaxList<T> list, Func<T, V> selector)
		where T : SyntaxNode
		where V : SyntaxNode
	{
		var selected = Enumerable.Select(list, selector);
		return SyntaxFactory.SeparatedList(selected);
	}
}
