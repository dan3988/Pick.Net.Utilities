namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class ModifierLists
{
	public static readonly SyntaxTokenList PrivateStaticPartial = new(Keywords.Private, Keywords.Static, Keywords.Partial);
	public static readonly SyntaxTokenList PrivateStaticReadOnly = new(Keywords.Private, Keywords.Static, Keywords.ReadOnly);
	public static readonly SyntaxTokenList PrivatePartial = new(Keywords.Private, Keywords.Partial);
	public static readonly SyntaxTokenList PublicStaticPartial = new(Keywords.Public, Keywords.Static, Keywords.Partial);
	public static readonly SyntaxTokenList Public = new(Keywords.Public);
	public static readonly SyntaxTokenList Internal = new(Keywords.Internal);
	public static readonly SyntaxTokenList ProtectedInternal = new(Keywords.Protected, Keywords.Internal);
	public static readonly SyntaxTokenList Protected = new(Keywords.Protected);
	public static readonly SyntaxTokenList PrivateProtected = new(Keywords.Private, Keywords.Protected);
	public static readonly SyntaxTokenList Private = new(Keywords.Private);
	public static readonly SyntaxTokenList Static = new(Keywords.Static);
	public static readonly SyntaxTokenList StaticPartial = new(Keywords.Static, Keywords.Partial);
	public static readonly SyntaxTokenList StaticReadOnly = new(Keywords.Static, Keywords.ReadOnly);
	public static readonly SyntaxTokenList Partial = new(Keywords.Partial);
}
