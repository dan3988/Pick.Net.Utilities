namespace Pick.Net.Utilities.Maui.SourceGenerators;

using System.Collections.Immutable;

using static SyntaxFactory;

internal static class SymbolHelper
{
	private static readonly SymbolDisplayFormat FullyQualifiedNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);
	private static readonly SymbolDisplayFormat FullTypeNameFormat = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
	private static readonly SymbolDisplayFormat FullNamespaceFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

	private readonly record struct SpecialTypeInfo(TypeCode TypeCode, SyntaxToken? PredefinedType)
	{
		public SpecialTypeInfo(TypeCode typeCode) : this(typeCode, new SyntaxToken?())
		{
		}

		public SpecialTypeInfo(TypeCode typeCode, SyntaxKind predefinedTypeKind) : this(typeCode, Token(predefinedTypeKind))
		{
		}
	}

	private static readonly Dictionary<SpecialType, SpecialTypeInfo> specialTypesMap = new()
	{
		[SpecialType.System_Object]			= new(TypeCode.Object, SyntaxKind.ObjectKeyword),
		[SpecialType.System_Boolean]		= new(TypeCode.Boolean, SyntaxKind.BoolKeyword),
		[SpecialType.System_Char]			= new(TypeCode.Char, SyntaxKind.CharKeyword),
		[SpecialType.System_SByte]			= new(TypeCode.SByte, SyntaxKind.SByteKeyword),
		[SpecialType.System_Byte]			= new(TypeCode.Byte, SyntaxKind.ByteKeyword),
		[SpecialType.System_Int16]			= new(TypeCode.Int16, SyntaxKind.ShortKeyword),
		[SpecialType.System_UInt16]			= new(TypeCode.UInt16, SyntaxKind.UShortKeyword),
		[SpecialType.System_Int32]			= new(TypeCode.Int32, SyntaxKind.IntKeyword),
		[SpecialType.System_UInt32]			= new(TypeCode.UInt32, SyntaxKind.UIntKeyword),
		[SpecialType.System_Int64]			= new(TypeCode.Int64, SyntaxKind.LongKeyword),
		[SpecialType.System_UInt64]			= new(TypeCode.UInt64, SyntaxKind.ULongKeyword),
		[SpecialType.System_Single]			= new(TypeCode.Single, SyntaxKind.FloatKeyword),
		[SpecialType.System_Double]			= new(TypeCode.Double, SyntaxKind.DoubleKeyword),
		[SpecialType.System_Decimal]		= new(TypeCode.Decimal, SyntaxKind.DecimalKeyword),
		[SpecialType.System_String]			= new(TypeCode.String, SyntaxKind.StringKeyword),
		[SpecialType.System_DateTime]		= new(TypeCode.DateTime),
	};

	private static readonly Dictionary<Accessibility, SyntaxTokenList> VisibilityTokens = new()
	{
		[Accessibility.Private]					= ModifierLists.Private,
		[Accessibility.ProtectedAndInternal]	= ModifierLists.PrivateProtected,
		[Accessibility.Protected]				= ModifierLists.Protected,
		[Accessibility.Internal]				= ModifierLists.Internal,
		[Accessibility.ProtectedOrInternal]		= ModifierLists.ProtectedInternal,
		[Accessibility.Public]					= ModifierLists.Public,
	};

	public static bool TryGetTypeCode(this SpecialType type, out TypeCode typeCode)
	{
		if (specialTypesMap.TryGetValue(type, out var info))
		{
			typeCode = info.TypeCode;
			return true;
		}
		else
		{
			typeCode = default;
			return false;
		}
	}

	public static bool IsPrimitiveType(this INamedTypeSymbol type)
		=> IsPrimitiveType(type, out _);

	public static bool IsPrimitiveType(this INamedTypeSymbol type, out INamedTypeSymbol nullableUnderlyingType)
	{
		nullableUnderlyingType = type;

		if (!type.IsValueType)
			return type.SpecialType == SpecialType.System_String;

		if (type.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
		{
			if (type.TypeArguments[0] is not INamedTypeSymbol arg)
				return false;

			nullableUnderlyingType = arg;
			type = arg;
		}

		if (type.SpecialType == SpecialType.System_Enum)
		{
			if (type.EnumUnderlyingType == null)
				return false;

			type = type.EnumUnderlyingType;
		}

		return type.SpecialType != SpecialType.System_DateTime && specialTypesMap.ContainsKey(type.SpecialType);
	}

	public static string GetFullyQualifiedName(this ITypeSymbol symbol)
		=> symbol.ToDisplayString(FullyQualifiedNameFormat);

	public static string GetFullTypeName(this ITypeSymbol symbol, bool incluceNullableAnnotation)
		=> symbol.ToDisplayString(incluceNullableAnnotation ? FullTypeNameFormat : SymbolDisplayFormat.FullyQualifiedFormat);

	public static string GetFullName(this INamespaceSymbol symbol)
		=> symbol.ToDisplayString(FullNamespaceFormat);

	public static bool IsAutoProperty(this IPropertySymbol symbol)
		=> symbol.ContainingType.GetMembers().Any(v => v is IFieldSymbol f && SymbolEqualityComparer.Default.Equals(symbol, f.AssociatedSymbol));

	public static TypeSyntax ToIdentifier(this ITypeSymbol type, bool incluceNullableAnnotation = false)
		=> ToIdentifier((INamedTypeSymbol)type, incluceNullableAnnotation);

	public static TypeSyntax ToIdentifier(this INamedTypeSymbol type, bool incluceNullableAnnotation = false)
	{
		static TypeSyntax ToIdentifierCore(ITypeSymbol type)
		{
			return specialTypesMap.TryGetValue(type.SpecialType, out var info) && info.PredefinedType.HasValue
				? PredefinedType((SyntaxToken)info.PredefinedType)
				: IdentifierName(GetFullTypeName(type, false));
		}

		if (type.SpecialType == SpecialType.System_Nullable_T)
		{
			var identifier = ToIdentifierCore(type.TypeArguments[0]);
			return NullableType(identifier);
		}
		else
		{
			var identifier = ToIdentifierCore(type);
			if (incluceNullableAnnotation && type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
				identifier = NullableType(identifier);

			return identifier;
		}
	}

	public static bool Equals(this ITypeSymbol left, ITypeSymbol right, bool includeNullability)
		=> left.Equals(right, includeNullability ? SymbolEqualityComparer.IncludeNullability : SymbolEqualityComparer.Default);

	public static SyntaxTokenList GetModifiers(this IMethodSymbol symbol)
	{
		var list = symbol.DeclaredAccessibility.ToSyntaxList();
		if (symbol.IsStatic)
			list = list.Add(Keywords.Static);

		if (symbol.IsVirtual)
			list = list.Add(Keywords.Virtual);

		if (symbol.IsAbstract)
			list = list.Add(Keywords.Abstract);

		if (symbol.IsSealed)
			list = list.Add(Keywords.Sealed);

		if (symbol.IsOverride)
			list = list.Add(Keywords.Override);

		if (symbol.IsReadOnly)
			list = list.Add(Keywords.ReadOnly);

		if (symbol.IsAsync)
			list = list.Add(Keywords.Async);

		if (symbol.IsPartialDefinition)
			list = list.Add(Keywords.Partial);

		return list;
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

	public static IEnumerable<IMethodSymbol> SelectMethods(this IEnumerable<ISymbol> source)
		=> OfType<ISymbol, IMethodSymbol>(source, SymbolKind.Method);

	public static IEnumerable<IPropertySymbol> SelectProperties(this IEnumerable<ISymbol> source)
		=> OfType<ISymbol, IPropertySymbol>(source, SymbolKind.Property);

	private static IEnumerable<TOut> OfType<TIn, TOut>(IEnumerable<TIn> source, SymbolKind kind)
		where TIn : class, ISymbol
		where TOut : TIn
	{
		return source.Where(v => v.Kind == kind).Cast<TOut>();
	}
}