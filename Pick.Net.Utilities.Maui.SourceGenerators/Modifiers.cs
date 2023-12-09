namespace Pick.Net.Utilities.Maui.SourceGenerators;

using static SyntaxFactory;

internal static class Modifiers
{
	public static readonly SyntaxToken Public = Token(SyntaxKind.PublicKeyword);
	public static readonly SyntaxToken Internal = Token(SyntaxKind.InternalKeyword);
	public static readonly SyntaxToken Protected = Token(SyntaxKind.ProtectedKeyword);
	public static readonly SyntaxToken Private = Token(SyntaxKind.PrivateKeyword);
	public static readonly SyntaxToken Static = Token(SyntaxKind.StaticKeyword);
	public static readonly SyntaxToken Virtual = Token(SyntaxKind.VirtualKeyword);
	public static readonly SyntaxToken Abstract = Token(SyntaxKind.AbstractKeyword);
	public static readonly SyntaxToken Sealed = Token(SyntaxKind.SealedKeyword);
	public static readonly SyntaxToken Override = Token(SyntaxKind.OverrideKeyword);
	public static readonly SyntaxToken ReadOnly = Token(SyntaxKind.ReadOnlyKeyword);
	public static readonly SyntaxToken Required = Token(SyntaxKind.RequiredKeyword);
	public static readonly SyntaxToken Volatile = Token(SyntaxKind.VolatileKeyword);
	public static readonly SyntaxToken Async = Token(SyntaxKind.AsyncKeyword);
	public static readonly SyntaxToken Partial = Token(SyntaxKind.PartialKeyword);
}
