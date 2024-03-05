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
	
	private static readonly Type?[] TypeCodeTypes =
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

	/// <summary>
	/// Gets the type parameter of <see cref="IEnumerable{T}"/> if <paramref name="type"/> implements it, otherwise null.
	/// </summary>
	public static Type? TryGetCollectionType(this Type type)
	{
		if (type.IsArray)
			return type.GetElementType()!;

		if (type == typeof(string))
			return typeof(char);

		return EnumerableTypeCache.GetOrAdd(type, GetCollectionTypeImpl);
	}

	/// <summary>
	/// Gets the type parameter of <see cref="IEnumerable{T}"/> if <paramref name="type"/> implements it, otherwise a <see cref="ArgumentException"/> is thrown.
	/// </summary>
	public static Type GetCollectionType(this Type type)
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

	public static Type? ToType(this TypeCode typeCode)
		=> (uint)typeCode <= (uint)TypeCode.String ? TypeCodeTypes[(int)typeCode] : throw new ArgumentException($"TypeCode '{typeCode}' is not defined.", nameof(typeCode));

	/// <inheritdoc cref="GetBaseTypes(Type, Type?, bool)"/>
	public static IEnumerable<Type> GetBaseTypes(this Type type, bool includeSelf)
		=> GetBaseTypes(type, null, includeSelf);

	/// <summary>
	/// Get a sequence of the base types of <paramref name="type"/>.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="until">The type to stop the sequence on, or null to include all base types including <c>typeof(<see cref="object"/>)</c>.</param>
	/// <param name="includeSelf">If <c>true</c>, <paramref name="type"/> will be the first item in the sequence.</param>
	/// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
	/// <exception cref="ArgumentException">If <paramref name="until"/> is not null and not a type assignable to <paramref name="type"/>.</exception>
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

	/// <summary>
	/// Filters a sequence of <typeparamref name="TMember"/> instances to ones that have an attribute of type <paramref name="attributeType"/> defined.
	/// </summary>
	/// <typeparam name="TMember">The member type of the sequence.</typeparam>
	/// <param name="members">The sequence to filter.</param>
	/// <param name="attributeType">The type of attribute to check.</param>
	/// <param name="inherit">Whether to check for inherited attributes on each member.</param>
	/// <returns></returns>
	public static IEnumerable<TMember> WhereDefined<TMember>(this IEnumerable<TMember> members, Type attributeType, bool inherit = false) where TMember : MemberInfo
		=> members.Where(v => v.IsDefined(attributeType, inherit));

	/// <summary>
	/// Filters a sequence of <typeparamref name="TMember"/> instances to ones that have a single attribute of type <typeparamref name="TAttribute"/> declared and projects those members to a tuple containing the member and the attribute.
	/// </summary>
	/// <typeparam name="TMember">The member type of the sequence.</typeparam>
	/// <typeparam name="TAttribute">The type of attribute to look for.</typeparam>
	/// <param name="members">The sequence to filter.</param>
	/// <param name="inherit">Whether to check for inherited attributes on each member.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> instance containing the projected results.</returns>
	public static IEnumerable<(TMember Member, TAttribute Attribute)> WithAttribute<TMember, TAttribute>(this IEnumerable<TMember> members, bool inherit = true)
		where TMember : MemberInfo
		where TAttribute : Attribute
	{
		return from member in members let attr = (TAttribute?)Attribute.GetCustomAttribute(member, typeof(TAttribute), inherit) where attr != null select (member, attr);
	}

	/// <summary>
	/// Filters a sequence of <typeparamref name="TMember"/> instances to ones that have at least one attribute of type <typeparamref name="TAttribute"/> declared and projects those members to a tuple containing the member and the attributes.
	/// </summary>
	/// <typeparam name="TMember">The member type of the sequence.</typeparam>
	/// <typeparam name="TAttribute">The type of attribute to look for.</typeparam>
	/// <param name="members">The sequence to filter.</param>
	/// <param name="inherit">Whether to check for inherited attributes on each member.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> instance containing the projected results.</returns>
	public static IEnumerable<(TMember Member, TAttribute[] Attributes)> WithAttributes<TMember, TAttribute>(this IEnumerable<TMember> members, bool inherit = true)
		where TMember : MemberInfo
		where TAttribute : Attribute
	{
		return from member in members let attr = (TAttribute[])Attribute.GetCustomAttributes(member, typeof(TAttribute), inherit) select (member, attr);
	}
}
