using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class SourceGenerationExtensions
{
	private static readonly SymbolDisplayFormat fullTypeNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
	private static readonly SyntaxToken semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit)
		=> AddSource(context, hintName, unit, Encoding.UTF8);

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit, Encoding encoding)
	{
		var text = unit.GetText(encoding);
#if DEBUG
		var test = unit.ToFullString();
#endif
		context.AddSource(hintName, text);
	}

	public static T AddModifier<T>(this T syntax, SyntaxKind kind) where T : MemberDeclarationSyntax
		=> (T)syntax.AddModifiers(SyntaxFactory.Token(kind));

	public static T AddModifiers<T>(this T syntax, params SyntaxKind[] kind) where T : MemberDeclarationSyntax
		=> (T)syntax.AddModifiers(kind.Select(SyntaxFactory.Token).ToArray());

	public static string GetFullTypeName(this ITypeSymbol symbol)
		=> symbol.ToDisplayString(fullTypeNameFormat);

	public static AccessorDeclarationSyntax WithSemicolonToken(this AccessorDeclarationSyntax syntax)
		=> syntax.WithSemicolonToken(semicolon);

	public static InvocationExpressionSyntax AddArgumentListTypeOfArgument(this InvocationExpressionSyntax syntax, TypeSyntax type)
		=> syntax.AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(type)));

	public static InvocationExpressionSyntax AddArgumentListLiteralArgument(this InvocationExpressionSyntax syntax, string literal)
		=> syntax.AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(literal))));

	public static InvocationExpressionSyntax AddArgumentListLiteralArgument(this InvocationExpressionSyntax syntax, int literal)
		=> syntax.AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(literal))));
}
