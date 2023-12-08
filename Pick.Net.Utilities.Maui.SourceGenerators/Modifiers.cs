namespace Pick.Net.Utilities.Maui.SourceGenerators;

using static SyntaxFactory;

internal static class Modifiers
{
	public static readonly SyntaxToken Public = Token(SyntaxKind.PublicKeyword);
	public static readonly SyntaxToken Internal = Token(SyntaxKind.InternalKeyword);
	public static readonly SyntaxToken Protected = Token(SyntaxKind.ProtectedKeyword);
	public static readonly SyntaxToken Private = Token(SyntaxKind.PrivateKeyword);
	public static readonly SyntaxToken Static = Token(SyntaxKind.StaticKeyword);
	public static readonly SyntaxToken Partial = Token(SyntaxKind.PartialKeyword);
	public static readonly SyntaxToken ReadOnly = Token(SyntaxKind.ReadOnlyKeyword);
}
