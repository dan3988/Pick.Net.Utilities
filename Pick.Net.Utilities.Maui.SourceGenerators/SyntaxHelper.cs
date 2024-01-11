using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

using static SyntaxFactory;

internal static class SyntaxHelper
{
	public static readonly PredefinedTypeSyntax TypeVoid = PredefinedType(Keywords.Void);
	public static readonly PredefinedTypeSyntax TypeObject = PredefinedType(Keywords.Object);
	public static readonly PredefinedTypeSyntax TypeString = PredefinedType(Keywords.String);
	public static readonly PredefinedTypeSyntax TypeBoolean = PredefinedType(Keywords.Bool);
	public static readonly PredefinedTypeSyntax TypeChar = PredefinedType(Keywords.Char);
	public static readonly PredefinedTypeSyntax TypeByte = PredefinedType(Keywords.Byte);
	public static readonly PredefinedTypeSyntax TypeSByte = PredefinedType(Keywords.SByte);
	public static readonly PredefinedTypeSyntax TypeInt16 = PredefinedType(Keywords.Short);
	public static readonly PredefinedTypeSyntax TypeUInt16 = PredefinedType(Keywords.UShort);
	public static readonly PredefinedTypeSyntax TypeInt32 = PredefinedType(Keywords.Int);
	public static readonly PredefinedTypeSyntax TypeUInt32 = PredefinedType(Keywords.UInt);
	public static readonly PredefinedTypeSyntax TypeInt64 = PredefinedType(Keywords.Long);
	public static readonly PredefinedTypeSyntax TypeUInt64 = PredefinedType(Keywords.ULong);
	public static readonly PredefinedTypeSyntax TypeSingle = PredefinedType(Keywords.Float);
	public static readonly PredefinedTypeSyntax TypeDouble = PredefinedType(Keywords.Double);
	public static readonly PredefinedTypeSyntax TypeDecimal = PredefinedType(Keywords.Decimal);

	public static readonly LiteralExpressionSyntax Null = LiteralExpression(SyntaxKind.NullLiteralExpression);
	public static readonly LiteralExpressionSyntax Default = LiteralExpression(SyntaxKind.DefaultLiteralExpression);
	public static readonly LiteralExpressionSyntax True = LiteralExpression(SyntaxKind.TrueLiteralExpression);
	public static readonly LiteralExpressionSyntax False = LiteralExpression(SyntaxKind.FalseLiteralExpression);
	public static readonly SyntaxTriviaList EmptyTriviaList = TriviaList();

	public static readonly SyntaxToken Semicolon = Token(SyntaxKind.SemicolonToken);

	private static readonly SyntaxToken nameof = Identifier(EmptyTriviaList, SyntaxKind.NameOfKeyword, "nameof", "nameof", EmptyTriviaList);
	private static readonly IdentifierNameSyntax nameofSyntax = IdentifierName(nameof);

	public static readonly AttributeListSyntax GeneratedCodeAttributeList = CreateAttributeList();

	private static AttributeListSyntax CreateAttributeList()
	{
		var attributeArgs = SeparatedList(
		[
			AttributeArgument(Literal(AssemblyInfo.FullName)),
			AttributeArgument(Literal(AssemblyInfo.Version))
		]);

		var attribute = Attribute(Identifiers.GeneratedCodeAttribute, AttributeArgumentList(attributeArgs));
		return AttributeList(SingletonSeparatedList(attribute));
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

	public static bool SearchRecursive<T>(this SyntaxNode node, Func<T, bool> predicate, CancellationToken token = default) where T : SyntaxNode
		=> SearchRecursive(node, v => v is T t && predicate.Invoke(t), token);

	public static bool SearchRecursive(this SyntaxNode node, Func<SyntaxNode, bool> predicate, CancellationToken token = default)
	{
		var en = node.ChildNodes().GetEnumerator();
		if (!en.MoveNext())
		{
			en.Dispose();
			return false;
		}

		var stack = new Stack<IEnumerator<SyntaxNode>>();

		try
		{
			while (true)
			{
				if (predicate.Invoke(en.Current))
					return true;

				var next = en.Current.ChildNodes().GetEnumerator();
				if (next.MoveNext())
				{
					token.ThrowIfCancellationRequested();
					stack.Push(en);
					en = next;
					continue;
				}

				while (!en.MoveNext())
				{
					if (stack.Count == 0)
						return false;

					en.Dispose();
					en = stack.Pop();
				}
			}
		}
		finally
		{
			en.Dispose();

			foreach (var enumerator in stack)
				enumerator.Dispose();
		}
	}

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit)
		=> AddSource(context, hintName, unit, Encoding.UTF8);

	public static void AddSource(this SourceProductionContext context, string hintName, CompilationUnitSyntax unit, Encoding encoding)
	{
		var text = unit.GetText(encoding);
		context.AddSource(hintName, text);
	}
	public static bool Contains(this SyntaxTokenList list, SyntaxKind kind)
		=> list.IndexOf(kind) >= 0;

	public static SyntaxTokenList Remove(this SyntaxTokenList list, SyntaxKind kind)
	{
		var index = list.IndexOf(kind);
		return index < 0 ? list : list.RemoveAt(index);
	}

	public static NullableTypeSyntax AsNullable(this TypeSyntax type)
		=> NullableType(type);

	public static AccessorDeclarationSyntax WithSemicolonToken(this AccessorDeclarationSyntax syntax)
		=> syntax.WithSemicolonToken(Semicolon);

	public static MethodDeclarationSyntax WithSemicolonToken(this MethodDeclarationSyntax syntax)
		=> syntax.WithSemicolonToken(Semicolon);

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

	public static InvocationExpressionSyntax NameOf(string expression)
		=> InvocationExpression(nameofSyntax, ArgumentList(IdentifierName(expression)));

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
		=> ArgumentList((IEnumerable<ExpressionSyntax>)arguments);

	public static ArgumentListSyntax ArgumentList(IEnumerable<ExpressionSyntax> arguments)
	{
		var list = SeparatedList(arguments.Select(Argument));
		return SyntaxFactory.ArgumentList(list);
	}

	public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
		=> ArgumentList((IEnumerable<ArgumentSyntax>)arguments);

	public static ArgumentListSyntax ArgumentList(IEnumerable<ArgumentSyntax> arguments)
	{
		var list = SeparatedList(arguments);
		return SyntaxFactory.ArgumentList(list);
	}
}
