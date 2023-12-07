namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class ModifierLists
{
	public static readonly SyntaxTokenList PrivateStaticPartial = new(Modifiers.Private, Modifiers.Static, Modifiers.Partial);
	public static readonly SyntaxTokenList PrivatePartial = new(Modifiers.Private, Modifiers.Partial);
	public static readonly SyntaxTokenList StaticPartial = new(Modifiers.Static, Modifiers.Partial);
	public static readonly SyntaxTokenList StaticReadOnly = new(Modifiers.Static, Modifiers.ReadOnly);
	public static readonly SyntaxTokenList Partial = new(Modifiers.Partial);
}
