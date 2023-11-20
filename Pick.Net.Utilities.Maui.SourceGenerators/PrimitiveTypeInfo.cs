namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal abstract record PrimitiveTypeInfo
{
	public static readonly PrimitiveTypeInfo Boolean	= new BooleanTypeInfo();
	public static readonly PrimitiveTypeInfo Char		= new SimpleTypeInfo<char>(TypeCode.Char, SyntaxHelper.TypeChar, SyntaxKind.CharKeyword, SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo SByte		= new CastNumericTypeInfo<sbyte>(TypeCode.SByte, SyntaxHelper.TypeSByte, SyntaxKind.SByteKeyword);
	public static readonly PrimitiveTypeInfo Byte		= new CastNumericTypeInfo<byte>(TypeCode.Byte, SyntaxHelper.TypeByte, SyntaxKind.ByteKeyword);
	public static readonly PrimitiveTypeInfo Int16		= new CastNumericTypeInfo<short>(TypeCode.Int16, SyntaxHelper.TypeInt16, SyntaxKind.ShortKeyword);
	public static readonly PrimitiveTypeInfo UInt16		= new CastNumericTypeInfo<ushort>(TypeCode.UInt16, SyntaxHelper.TypeUInt16, SyntaxKind.UShortKeyword);
	public static readonly PrimitiveTypeInfo Int32		= new SimpleTypeInfo<int>(TypeCode.Int32, SyntaxHelper.TypeInt32, SyntaxKind.IntKeyword, SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo UInt32		= new SimpleTypeInfo<uint>(TypeCode.UInt32, SyntaxHelper.TypeUInt32, SyntaxKind.UIntKeyword, SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo Int64		= new SimpleTypeInfo<long>(TypeCode.Int64, SyntaxHelper.TypeInt64, SyntaxKind.LongKeyword, SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo UInt64		= new SimpleTypeInfo<ulong>(TypeCode.UInt64, SyntaxHelper.TypeUInt64, SyntaxKind.ULongKeyword, SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo Single		= new SimpleTypeInfo<float>(TypeCode.Single, SyntaxHelper.TypeSingle, SyntaxKind.FloatKeyword, SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal);
	public static readonly PrimitiveTypeInfo Double		= new SimpleTypeInfo<double>(TypeCode.Double, SyntaxHelper.TypeDouble, SyntaxKind.DoubleKeyword, SyntaxKind.NumericLiteralExpression, DoubleLiteral);
	public static readonly PrimitiveTypeInfo Decimal	= new SimpleTypeInfo<decimal>(TypeCode.Decimal, SyntaxHelper.TypeDecimal, SyntaxKind.DecimalKeyword, SyntaxKind.NumericLiteralExpression, DecimalLiteral);
	public static readonly PrimitiveTypeInfo String		= new SimpleTypeInfo<string>(TypeCode.String, SyntaxHelper.TypeString, SyntaxKind.StringKeyword, SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal);

	private static readonly PrimitiveTypeInfo?[] typeCodeLookup =
	{
		/* TypeCode.Empty = 0		*/ null,
		/* TypeCode.Object = 1		*/ null,
		/* TypeCode.DBNull = 2		*/ null,
		/* TypeCode.Boolean = 3		*/ Boolean,
		/* TypeCode.Char = 4		*/ Char,
		/* TypeCode.SByte = 5		*/ SByte,
		/* TypeCode.Byte = 6		*/ Byte,
		/* TypeCode.Int16 = 7		*/ Int16,
		/* TypeCode.UInt16 = 8		*/ UInt16,
		/* TypeCode.Int32 = 9		*/ Int32,
		/* TypeCode.UInt32 = 10		*/ UInt32,
		/* TypeCode.Int64 = 11		*/ Int64,
		/* TypeCode.UInt64 = 12		*/ UInt64,
		/* TypeCode.Single = 13		*/ Single,
		/* TypeCode.Double = 14		*/ Double,
		/* TypeCode.Decimal = 15	*/ Decimal,
		/* TypeCode.DateTime = 16	*/ null,
		/* gap						*/ null,
		/* TypeCode.String = 18		*/ String,
	};

	private static SyntaxToken DoubleLiteral(double value)
		=> SyntaxFactory.Literal($"{value}D", value);

	private static SyntaxToken DecimalLiteral(decimal value)
		=> SyntaxFactory.Literal($"{value}M", value);

	public static PrimitiveTypeInfo ForTypeCode(TypeCode type)
	{
		if (unchecked((uint)type >= (uint)typeCodeLookup.Length))
			throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid TypeCode");

		return typeCodeLookup[(int)type] ?? throw new ArgumentException(type + " is not a primitive type.", nameof(type));
	}

	public TypeCode TypeCode { get; }

	public PredefinedTypeSyntax TypeSyntax { get; }

	public SyntaxKind KeywordKind { get; }

	private PrimitiveTypeInfo(TypeCode typeCode, PredefinedTypeSyntax typeSyntax, SyntaxKind keywordKind)
	{
		TypeCode = typeCode;
		TypeSyntax = typeSyntax;
		KeywordKind = keywordKind;
	}

	public abstract SyntaxToken CreateLiteral(object value);

	public abstract ExpressionSyntax CreateSyntax(object value);

	private sealed record BooleanTypeInfo() : PrimitiveTypeInfo(TypeCode.Boolean, SyntaxHelper.TypeBoolean, SyntaxKind.BoolKeyword)
	{
		public override SyntaxToken CreateLiteral(object value)
			=> SyntaxHelper.LiteralToken((bool)value);

		public override ExpressionSyntax CreateSyntax(object value)
			=> SyntaxHelper.Literal((bool)value);
	}

	private abstract record DynamicTypeInfo<T>(TypeCode TypeCode, PredefinedTypeSyntax TypeSyntax, SyntaxKind KeywordKind, SyntaxKind ExpressionKind) : PrimitiveTypeInfo(TypeCode, TypeSyntax, KeywordKind)
		where T : notnull, IConvertible
	{
		public sealed override ExpressionSyntax CreateSyntax(object value)
		{
			var token = CreateLiteral(value);
			return SyntaxFactory.LiteralExpression(ExpressionKind, token);
		}
	}

	private sealed record CastNumericTypeInfo<T>(TypeCode TypeCode, PredefinedTypeSyntax TypeSyntax, SyntaxKind KeywordKind) : DynamicTypeInfo<T>(TypeCode, TypeSyntax, KeywordKind, SyntaxKind.NumericLiteralExpression)
		where T : unmanaged, IConvertible
	{
		public override SyntaxToken CreateLiteral(object value)
		{
			var intValue = Convert.ToInt32(value);
			return SyntaxFactory.Literal("(" + TypeSyntax.Keyword.Text + ")" + intValue, intValue);
		}
	}

	private sealed record SimpleTypeInfo<T>(TypeCode TypeCode, PredefinedTypeSyntax TypeSyntax, SyntaxKind KeywordKind, SyntaxKind ExpressionKind, Func<T, SyntaxToken> LiteralFactory) : DynamicTypeInfo<T>(TypeCode, TypeSyntax, KeywordKind, ExpressionKind)
		where T : notnull, IConvertible
	{
		public override SyntaxToken CreateLiteral(object value)
			=> LiteralFactory.Invoke((T)value);
	}
}
