namespace Pick.Net.Utilities.Maui.SourceGenerators;

using System.Collections.Immutable;

using static SyntaxFactory;

internal static class SymbolHelper
{
	private static readonly SymbolDisplayFormat FullTypeNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
	private static readonly SymbolDisplayFormat FullNamespaceFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

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

	public static readonly SyntaxTokenList TokensPublic = CreateTokenList(SyntaxKind.PublicKeyword);
	public static readonly SyntaxTokenList TokensProtected = CreateTokenList(SyntaxKind.ProtectedKeyword);
	public static readonly SyntaxTokenList TokensInternal = CreateTokenList(SyntaxKind.InternalKeyword);
	public static readonly SyntaxTokenList TokensPrivate = CreateTokenList(SyntaxKind.PrivateKeyword);
	public static readonly SyntaxTokenList TokensProtectedInternal = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword);
	public static readonly SyntaxTokenList TokensProtectedPrivate = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword);

	private static readonly Dictionary<Accessibility, SyntaxTokenList> VisibilityTokens = new()
	{
		[Accessibility.Public] = TokensPublic,
		[Accessibility.Protected] = TokensProtected,
		[Accessibility.Internal] = TokensInternal,
		[Accessibility.Private] = TokensPrivate,
		[Accessibility.ProtectedOrInternal] = TokensProtectedInternal,
		[Accessibility.ProtectedAndInternal] = TokensProtectedPrivate,
	};

	private static SyntaxTokenList CreateTokenList(SyntaxKind kind)
		=> new(Token(kind));

	private static SyntaxTokenList CreateTokenList(params SyntaxKind[] kinds)
		=> new(kinds.Select(Token));

	public static bool TryGetTypeCode(this SpecialType type, out TypeCode typeCode)
		=> specialTypesMap.TryGetValue(type, out typeCode);

	public static string GetFullTypeName(this ITypeSymbol symbol)
		=> symbol.ToDisplayString(FullTypeNameFormat);

	public static string GetFullName(this INamespaceSymbol symbol)
		=> symbol.ToDisplayString(FullNamespaceFormat);

	public static IdentifierNameSyntax ToIdentifier(this ITypeSymbol type)
	{
		var name = GetFullTypeName(type);
		return IdentifierName(name);
	}

	public static SyntaxTokenList ToSyntaxList(this Accessibility accessibility)
	{
		VisibilityTokens.TryGetValue(accessibility, out var list);
		return list;
	}

	public static ImmutableArray<string> GetContainingTypeNames(this ISymbol type)
	{
		var parent = type.ContainingType;
		if (parent == null)
		{
			return ImmutableArray<string>.Empty;
		}
		else
		{
			var builder = ImmutableArray.CreateBuilder<string>();

			do
			{
				builder.Add(parent.MetadataName);
				parent = parent.ContainingType;
			}
			while (parent != null);

			return builder.ToImmutable();
		}
	}
}