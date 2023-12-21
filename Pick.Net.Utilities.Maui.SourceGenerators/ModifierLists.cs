namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class ModifierLists
{
	public static readonly SyntaxTokenList PrivateStaticPartial = new(Modifiers.Private, Modifiers.Static, Modifiers.Partial);
	public static readonly SyntaxTokenList PrivateStaticReadOnly = new(Modifiers.Private, Modifiers.Static, Modifiers.ReadOnly);
	public static readonly SyntaxTokenList PrivatePartial = new(Modifiers.Private, Modifiers.Partial);
	public static readonly SyntaxTokenList PublicStaticPartial = new(Modifiers.Public, Modifiers.Static, Modifiers.Partial);
	public static readonly SyntaxTokenList Public = new(Modifiers.Public);
	public static readonly SyntaxTokenList Internal = new(Modifiers.Internal);
	public static readonly SyntaxTokenList ProtectedInternal = new(Modifiers.Protected, Modifiers.Internal);
	public static readonly SyntaxTokenList Protected = new(Modifiers.Protected);
	public static readonly SyntaxTokenList PrivateProtected = new(Modifiers.Private, Modifiers.Protected);
	public static readonly SyntaxTokenList Private = new(Modifiers.Private);
	public static readonly SyntaxTokenList Static = new(Modifiers.Static);
	public static readonly SyntaxTokenList StaticPartial = new(Modifiers.Static, Modifiers.Partial);
	public static readonly SyntaxTokenList StaticReadOnly = new(Modifiers.Static, Modifiers.ReadOnly);
	public static readonly SyntaxTokenList Partial = new(Modifiers.Partial);
}
