using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class SourceGenerationExtensions
{
	public static readonly PredefinedTypeSyntax VoidType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
	public static readonly LiteralExpressionSyntax Null = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
	public static readonly LiteralExpressionSyntax Default = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
	public static readonly LiteralExpressionSyntax True = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
	public static readonly LiteralExpressionSyntax False = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);

	private static readonly SymbolDisplayFormat fullTypeNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
	private static readonly SyntaxToken semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
	{
		key = pair.Key;
		value = pair.Value;
	}

	public static T AddFormatting<T>(this T syntax) where T : CSharpSyntaxNode
	{
		syntax = syntax.NormalizeWhitespace("\t");
		var visitor = new WhitespaceSyntaxRewriter();
		return (T)syntax.Accept(visitor)!;
	}

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit)
		=> AddSource(context, hintName, unit, Encoding.UTF8);

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit, Encoding encoding)
	{
		var text = unit.GetText(encoding);
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

	public static MethodDeclarationSyntax WithSemicolonToken(this MethodDeclarationSyntax syntax)
		=> syntax.WithSemicolonToken(semicolon);

	private static SyntaxTrivia[] CreateIndentTriviaList(int indentCount, SyntaxTriviaList keepTrivia = default)
	{
		var array = new SyntaxTrivia[keepTrivia.Count + indentCount + 1];
		var i = 0;
		for (; i < keepTrivia.Count; i++)
			array[i] = keepTrivia[i];

		array[i] = SyntaxFactory.CarriageReturnLineFeed;

		while (++i < array.Length)
			array[i] = SyntaxFactory.Tab;

		return array;
	}

	public static T WithLeadingLineBreak<T>(this T syntax) where T : SyntaxNode
		=> syntax.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

	public static T WithLeadingLineBreak<T>(this T syntax, int indentCount) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(indentCount);
		return syntax.WithLeadingTrivia(list);
	}

	public static T AddLeadingLineBreak<T>(this T syntax) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(0, syntax.GetLeadingTrivia());
		return syntax.WithLeadingTrivia(list);
	}

	public static T AddLeadingLineBreak<T>(this T syntax, int indentCount) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(indentCount, syntax.GetLeadingTrivia());
		return syntax.WithLeadingTrivia(list);
	}

	public static T WithTrailingLineBreak<T>(this T syntax) where T : SyntaxNode
		=> syntax.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

	public static T WithTrailingLineBreak<T>(this T syntax, int indentCount) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(indentCount);
		return syntax.WithTrailingTrivia(list);
	}

	public static T AddTrailingLineBreak<T>(this T syntax) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(0, syntax.GetTrailingTrivia());
		return syntax.WithTrailingTrivia(list);
	}

	public static T AddTrailingLineBreak<T>(this T syntax, int indentCount) where T : SyntaxNode
	{
		var list = CreateIndentTriviaList(indentCount, syntax.GetTrailingTrivia());
		return syntax.WithTrailingTrivia(list);
	}

	public static ParenthesizedExpressionSyntax WithSurroundingParenthesis(this ExpressionSyntax syntax)
		=> SyntaxFactory.ParenthesizedExpression(syntax);

	public static TypeOfExpressionSyntax TypeOf(TypeSyntax type)
		=> SyntaxFactory.TypeOfExpression(type);

	public static LiteralExpressionSyntax Literal(string literal)
		=> SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(literal));

	public static LiteralExpressionSyntax Literal(int literal)
		=> SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(literal));

	public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
	{
		var list = SyntaxFactory.SeparatedList(arguments);
		return SyntaxFactory.ArgumentList(list);
	}
}
