using System.Text;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class SyntaxHelper
{
	public static readonly PredefinedTypeSyntax TypeVoid = PredefinedType(Token(SyntaxKind.VoidKeyword));
	public static readonly PredefinedTypeSyntax TypeObject = PredefinedType(Token(SyntaxKind.ObjectKeyword));
	public static readonly PredefinedTypeSyntax TypeString = PredefinedType(Token(SyntaxKind.StringKeyword));
	public static readonly PredefinedTypeSyntax TypeBoolean = PredefinedType(Token(SyntaxKind.BoolKeyword));
	public static readonly PredefinedTypeSyntax TypeChar = PredefinedType(Token(SyntaxKind.CharKeyword));
	public static readonly PredefinedTypeSyntax TypeByte = PredefinedType(Token(SyntaxKind.ByteKeyword));
	public static readonly PredefinedTypeSyntax TypeSByte = PredefinedType(Token(SyntaxKind.SByteKeyword));
	public static readonly PredefinedTypeSyntax TypeInt16 = PredefinedType(Token(SyntaxKind.ShortKeyword));
	public static readonly PredefinedTypeSyntax TypeUInt16 = PredefinedType(Token(SyntaxKind.UShortKeyword));
	public static readonly PredefinedTypeSyntax TypeInt32 = PredefinedType(Token(SyntaxKind.IntKeyword));
	public static readonly PredefinedTypeSyntax TypeUInt32 = PredefinedType(Token(SyntaxKind.UIntKeyword));
	public static readonly PredefinedTypeSyntax TypeInt64 = PredefinedType(Token(SyntaxKind.LongKeyword));
	public static readonly PredefinedTypeSyntax TypeUInt64 = PredefinedType(Token(SyntaxKind.ULongKeyword));
	public static readonly PredefinedTypeSyntax TypeSingle = PredefinedType(Token(SyntaxKind.FloatKeyword));
	public static readonly PredefinedTypeSyntax TypeDouble = PredefinedType(Token(SyntaxKind.DoubleKeyword));
	public static readonly PredefinedTypeSyntax TypeDecimal = PredefinedType(Token(SyntaxKind.DecimalKeyword));

	public static readonly LiteralExpressionSyntax Null = LiteralExpression(SyntaxKind.NullLiteralExpression);
	public static readonly LiteralExpressionSyntax Default = LiteralExpression(SyntaxKind.DefaultLiteralExpression);
	public static readonly LiteralExpressionSyntax True = LiteralExpression(SyntaxKind.TrueLiteralExpression);
	public static readonly LiteralExpressionSyntax False = LiteralExpression(SyntaxKind.FalseLiteralExpression);
	public static readonly SyntaxTriviaList EmptyTriviaList = TriviaList();

	private static readonly SymbolDisplayFormat fullTypeNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
	private static readonly SymbolDisplayFormat fullNamespaceFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
	
	private static readonly SyntaxToken semicolon = Token(SyntaxKind.SemicolonToken);
	private static readonly SyntaxToken nameof = Identifier(EmptyTriviaList, SyntaxKind.NameOfKeyword, "nameof", "nameof", EmptyTriviaList);
	private static readonly IdentifierNameSyntax nameofSyntax = IdentifierName(nameof);

	private static readonly Dictionary<SpecialType, TypeCode> specialTypesMap = new()
	{
		[SpecialType.System_Object] = TypeCode.Object,
		[SpecialType.System_Boolean] = TypeCode.Boolean,
		[SpecialType.System_Char] = TypeCode.Char,
		[SpecialType.System_SByte] = TypeCode.SByte,
		[SpecialType.System_Byte] = TypeCode.Byte,
		[SpecialType.System_Int16] = TypeCode.Int16,
		[SpecialType.System_UInt16] = TypeCode.UInt16,
		[SpecialType.System_Int32] = TypeCode.Int32,
		[SpecialType.System_UInt32] = TypeCode.UInt32,
		[SpecialType.System_Int64] = TypeCode.Int64,
		[SpecialType.System_UInt64] = TypeCode.UInt64,
		[SpecialType.System_Single] = TypeCode.Single,
		[SpecialType.System_Double] = TypeCode.Double,
		[SpecialType.System_Decimal] = TypeCode.Decimal,
		[SpecialType.System_String] = TypeCode.String,
		[SpecialType.System_DateTime] = TypeCode.DateTime
	};

	public static bool TryGetTypeCode(this SpecialType type, out TypeCode typeCode)
		=> specialTypesMap.TryGetValue(type, out typeCode);

	public static IdentifierNameSyntax ToIdentifier(this ITypeSymbol type)
	{
		var name = GetFullTypeName(type);
		return IdentifierName(name);
	}

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
	{
		key = pair.Key;
		value = pair.Value;
	}

	public static T AddFormatting<T>(this T syntax) where T : CSharpSyntaxNode
	{
		syntax = syntax.NormalizeWhitespace("\t");
		var visitor = new WhitespaceSyntaxReWriter();
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
		=> (T)syntax.AddModifiers(Token(kind));

	public static T AddModifiers<T>(this T syntax, params SyntaxKind[] kind) where T : MemberDeclarationSyntax
		=> (T)syntax.AddModifiers(kind.Select(Token).ToArray());

	public static string GetFullTypeName(this ITypeSymbol symbol)
		=> symbol.ToDisplayString(fullTypeNameFormat);

	public static string GetFullName(this INamespaceSymbol symbol)
		=> symbol.ToDisplayString(fullNamespaceFormat);

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

		array[i] = CarriageReturnLineFeed;

		while (++i < array.Length)
			array[i] = Tab;

		return array;
	}

	public static T WithLeadingLineBreak<T>(this T syntax) where T : SyntaxNode
		=> syntax.WithLeadingTrivia(CarriageReturnLineFeed);

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
		=> syntax.WithTrailingTrivia(CarriageReturnLineFeed);

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
		=> ParenthesizedExpression(syntax);

	public static InvocationExpressionSyntax NameOf(ExpressionSyntax expression)
		=> InvocationExpression(nameofSyntax, ArgumentList(expression));

	public static TypeOfExpressionSyntax TypeOf(TypeSyntax type)
		=> TypeOfExpression(type);

	public static SyntaxToken LiteralToken(bool value)
		=> Literal(value).Token;

	public static LiteralExpressionSyntax Literal(bool value)
		=> value ? True : False;

	public static LiteralExpressionSyntax Literal(string value)
		=> LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));

	public static LiteralExpressionSyntax Literal(int value)
		=> LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));

	public static ExpressionSyntax Literal(object? value)
	{
		if (value == null)
			return Null;

		var typeCode = Convert.GetTypeCode(value);
		var typeInfo = PrimitiveTypeInfo.ForTypeCode(typeCode);
		return typeInfo.CreateSyntax(value);
	}

	public static ExpressionSyntax Literal(object? value, TypeCode typeCode)
	{
		if (value == null)
			return Null;

		if (typeCode == TypeCode.Object)
		{
			typeCode = Convert.GetTypeCode(value);
		}
		else
		{
			value = Convert.ChangeType(value, typeCode);
		}

		var typeInfo = PrimitiveTypeInfo.ForTypeCode(typeCode);
		return typeInfo.CreateSyntax(value);
	}

	public static ArgumentListSyntax ArgumentList(params ExpressionSyntax[] arguments)
	{
		var list = SeparatedList(arguments.Select(Argument));
		return SyntaxFactory.ArgumentList(list);
	}

	public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
	{
		var list = SeparatedList(arguments);
		return SyntaxFactory.ArgumentList(list);
	}
}
