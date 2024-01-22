using System.Collections.Concurrent;
using System.Reflection;

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

	private static readonly ConcurrentDictionary<Type, TypeMembers> Members = new()
	{
		[typeof(object)] = TypeMembers.Object,
		[typeof(ValueType)] = TypeMembers.ValueType,
		[typeof(Enum)] = TypeMembers.Enum
	};

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

	public static TypeMembers<T> GetMembers<T>()
		=> (TypeMembers<T>)Members.GetOrAdd(typeof(T), CreateMembers);

	public static TypeMembers GetMembers(Type type)
		=> Members.GetOrAdd(type, CreateMembers);

	private static TypeMembers CreateMembers(Type arg)
	{
		var type = typeof(TypeMembers<>).MakeGenericType(arg);
		return (TypeMembers)Activator.CreateInstance(type, true)!;
	}
}
