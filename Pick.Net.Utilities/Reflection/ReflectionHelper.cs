using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;

using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Reflection;

public static class ReflectionHelper
{
	public const BindingFlags Declared = DeclaredPublic | BindingFlags.NonPublic;
	public const BindingFlags DeclaredInstance = DeclaredPublicInstance | BindingFlags.NonPublic;
	public const BindingFlags DeclaredStatic = DeclaredPublicStatic | BindingFlags.NonPublic;

	public const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
	public const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

	public const BindingFlags DeclaredPublic = Public | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredPublicInstance = PublicInstance | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredPublicStatic = PublicStatic | BindingFlags.DeclaredOnly;

	public const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
	public const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
	public const BindingFlags NonPublicStatic = BindingFlags.NonPublic | BindingFlags.Static;

	public const BindingFlags DeclaredNonPublic = NonPublic | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredNonPublicInstance = NonPublicInstance | BindingFlags.DeclaredOnly;
	public const BindingFlags DeclaredNonPublicStatic = NonPublicStatic | BindingFlags.DeclaredOnly;
	
	private static readonly Type[] TypeCodeTypes =
	[
		/* TypeCode.Empty		*/ null,
		/* TypeCode.Object		*/ typeof(object),
		/* TypeCode.DBNull		*/ typeof(DBNull),
		/* TypeCode.Boolean		*/ typeof(bool),
		/* TypeCode.Char		*/ typeof(char),
		/* TypeCode.SByte		*/ typeof(sbyte),
		/* TypeCode.Byte		*/ typeof(byte),
		/* TypeCode.Int16		*/ typeof(short),
		/* TypeCode.UInt16		*/ typeof(ushort),
		/* TypeCode.Int32		*/ typeof(int),
		/* TypeCode.UInt32		*/ typeof(uint),
		/* TypeCode.Int64		*/ typeof(long),
		/* TypeCode.UInt64		*/ typeof(ulong),
		/* TypeCode.Single		*/ typeof(float),
		/* TypeCode.Double		*/ typeof(double),
		/* TypeCode.Decimal		*/ typeof(decimal),
		/* TypeCode.DateTime	*/ typeof(DateTime),
		/* TypeCode.String		*/ typeof(string),
	];

	private static readonly ImmutableHashSet<Type> KnownEnumerableInterfaces = ImmutableHashSet.Create([
		typeof(IEnumerable<>),
		typeof(IReadOnlyCollection<>),
		typeof(IReadOnlyList<>),
		typeof(IReadOnlySet<>),
		typeof(ICollection<>),
		typeof(IList<>),
		typeof(ISet<>),
		typeof(IImmutableList<>),
		typeof(IImmutableSet<>),
		typeof(IImmutableQueue<>),
		typeof(IImmutableStack<>),
	]);

	private static readonly ImmutableHashSet<Type> KnownEnumerableTypes = ImmutableHashSet.Create([
		.. KnownEnumerableInterfaces,
		typeof(ArraySegment<>),
		typeof(List<>),
		typeof(Stack<>),
		typeof(Queue<>),
		typeof(HashSet<>),
		typeof(ReadOnlyCollection<>),
		typeof(ReadOnlyObservableCollection<>),
		typeof(ObservableCollection<>),
		typeof(ObservableList<>),
		typeof(ConcurrentBag<>),
		typeof(ConcurrentStack<>),
		typeof(ConcurrentQueue<>),
		typeof(BlockingCollection<>),
		typeof(ImmutableArray<>),
		typeof(ImmutableHashSet<>),
		typeof(ImmutableQueue<>),
		typeof(ImmutableStack<>),
		typeof(ImmutableSortedSet<>)
	]);

	private static readonly ImmutableHashSet<Type> KnownDictionaryTypes = ImmutableHashSet.Create([
		typeof(IReadOnlyDictionary<,>),
		typeof(IDictionary<,>),
		typeof(IImmutableDictionary<,>),
		typeof(Dictionary<,>),
		typeof(ReadOnlyDictionary<,>),
		typeof(ImmutableDictionary<,>),
		typeof(ImmutableSortedDictionary<,>),
		typeof(ConcurrentDictionary<,>),
		typeof(SortedDictionary<,>),
		typeof(SortedList<,>)
	]);

	private static readonly ConcurrentDictionary<Type, Type?> EnumerableTypeCache = [];

	public static Type? TryGetCollectionType(this Type type)
	{
		if (type.IsArray)
			return type.GetElementType()!;

		if (type == typeof(string))
			return typeof(char);

		return EnumerableTypeCache.GetOrAdd(type, GetCollectionTypeImpl);
	}

	public static Type? GetCollectionType(this Type type)
		=> TryGetCollectionType(type) ?? throw new ArgumentException($"Type '{type}' does not implement IEnumerable<T>.", nameof(type));

	private static Type? GetCollectionTypeImpl(Type arg)
	{
		if (arg.IsGenericType)
		{
			var generic = arg.GetGenericTypeDefinition();
			if (KnownEnumerableTypes.Contains(generic))
				return arg.GetGenericArguments()[0];

			if (KnownDictionaryTypes.Contains(generic))
				return typeof(KeyValuePair<,>).MakeGenericType(arg.GetGenericArguments());
		}

		foreach (var i in arg.GetInterfaces())
			if (i.IsGenericType && KnownEnumerableInterfaces.Contains(i.GetGenericTypeDefinition()))
				return i.GetGenericArguments()[0];

		return null;
	}

	public static Type ToType(this TypeCode typeCode)
		=> (uint)typeCode <= (uint)TypeCode.String ? TypeCodeTypes[(int)typeCode] : throw new ArgumentException($"TypeCode '{typeCode}' is not defined.", nameof(typeCode));

	public static IEnumerable<Type> GetBaseTypes(this Type type, bool includeSelf)
		=> GetBaseTypes(type, null, includeSelf);

	public static IEnumerable<Type> GetBaseTypes(this Type type, Type? until = null, bool includeSelf = false)
	{
		ArgumentNullException.ThrowIfNull(type);

		if (until != null && !type.IsAssignableTo(until))
			throw new ArgumentException("End type is not assignable to input type.", nameof(until));

		if (includeSelf)
			yield return type;

		while ((type = type.BaseType!) != until)
			yield return type;
	}

	public static IEnumerable<TMember> WhereDefined<TMember>(this IEnumerable<TMember> members, Type attributeType, bool inherit = false) where TMember : MemberInfo
		=> members.Where(v => v.IsDefined(attributeType, inherit));

	public static IEnumerable<(TMember Member, TAttribute Attribute)> WithAttribute<TMember, TAttribute>(this IEnumerable<TMember> members, bool inherit = true)
		where TMember : MemberInfo
		where TAttribute : Attribute
	{
		return from member in members let attr = (TAttribute?)Attribute.GetCustomAttribute(member, typeof(TAttribute), inherit) where attr != null select (member, attr);
	}

	public static IEnumerable<(TMember Member, TAttribute[] Attributes)> WithAttributes<TMember, TAttribute>(this IEnumerable<TMember> members, bool inherit = true)
		where TMember : MemberInfo
		where TAttribute : Attribute
	{
		return from member in members let attr = (TAttribute[])Attribute.GetCustomAttributes(member, typeof(TAttribute), inherit) select (member, attr);
	}
}
