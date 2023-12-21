using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Reflection;

public static partial class DelegateHelper
{
	private static readonly Dictionary<Type, DelegateInfo> DelegateInfos = [];

	/// <summary>
	/// Thread safe delegate concatenation of <paramref name="value"/> to the field <paramref name="eventHandler"/>.
	/// </summary>
	/// <remarks>
	/// This is the same implementation that the compiler generates when an event without accessors is declared.
	/// </remarks>
	/// <typeparam name="T">The delegate type</typeparam>
	/// <param name="eventHandler">A refernece to the field for the event handler</param>
	/// <param name="value">The delegate to add to the event handler</param>
	public static void SafeSubscribe<T>(ref T? eventHandler, T? value) where T : Delegate
	{
		T? last = eventHandler;
		T? expected, current;
		do
		{
			expected = last;
			current = (T?)Delegate.Combine(expected, value);
			last = Interlocked.CompareExchange(ref eventHandler, current, expected);
		}
		while (last != expected);
	}

	/// <summary>
	/// Thread safe delegate removal of <paramref name="value"/> from the field <paramref name="eventHandler"/>.
	/// </summary>
	/// <remarks>
	/// This is the same implementation that the compiler generates when an event without accessors is declared.
	/// </remarks>
	/// <typeparam name="T">The delegate type</typeparam>
	/// <param name="eventHandler">A refernece to the field for the event handler</param>
	/// <param name="value">The delegate to remove from the event handler</param>
	public static void SafeUnsubscribe<T>(ref T? eventHandler, T? value) where T : Delegate
	{
		T? last = eventHandler;
		T? expected, current;
		do
		{
			expected = last;
			current = (T?)Delegate.Remove(expected, value);
			last = Interlocked.CompareExchange(ref eventHandler, current, expected);
		}
		while (last != expected);
	}

	/// <inheritdoc cref="Delegate.Combine(Delegate?, Delegate?)"/>
	[return: NotNullIfNotNull(nameof(a))]
	[return: NotNullIfNotNull(nameof(b))]
	public static T? Combine<T>(T? a, T? b) where T : Delegate
		=> (T?)Delegate.Combine(a, b);

	/// <inheritdoc cref="Delegate.Combine(Delegate?[])"/>
	public static T? Combine<T>(params T?[] delegates) where T : Delegate
		=> (T?)Delegate.Combine(delegates);

	/// <inheritdoc cref="Delegate.Remove(Delegate?, Delegate?)"/>
	public static T? Remove<T>(T? source, T? value) where T : Delegate
		=> (T?)Delegate.Remove(source, value);

	/// <inheritdoc cref="Delegate.RemoveAll(Delegate?, Delegate?)"/>
	public static T? RemoveAll<T>(T? source, T? value) where T : Delegate
		=> (T?)Delegate.RemoveAll(source, value);

	/// <summary>
	/// Gets the types of the parameters for the delegate <typeparamref name="T"/>
	/// </summary>
	/// <exception cref="ArgumentException"><typeparamref name="T"/> is <see cref="Delegate"/> or <see cref="MulticastDelegate"/></exception>
	public static ImmutableArray<Type> GetParameterTypes<T>() where T : Delegate
		=> GetDelegateInfo<T>().ArgumentTypes;

	/// <summary>
	/// Gets the types of the parameters for the delegate <paramref name="type"/>
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="type"/> is not a delegate type</exception>
	public static ImmutableArray<Type> GetParameterTypes(Type type)
		=> GetDelegateInfo(type).ArgumentTypes;

	/// <summary>
	/// Gets the return type of the delegate <typeparamref name="T"/>
	/// </summary>
	/// <exception cref="ArgumentException"><typeparamref name="T"/> is <see cref="Delegate"/> or <see cref="MulticastDelegate"/></exception>
	public static Type GetReturnType<T>() where T : Delegate
		=> GetDelegateInfo<T>().ReturnType;

	/// <summary>
	/// Gets the return type of the delegate <paramref name="type"/>
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="type"/> is not a delegate type</exception>
	public static Type GetReturnType(Type type)
		=> GetDelegateInfo(type).ReturnType;

	private static DelegateInfo GetDelegateInfo<T>() where T : Delegate
	{
		if (typeof(T).IsAbstract)
			throw new ArgumentException($"Type {typeof(T)} is not a delegate type.");

		return DelegateInfo<T>.Instance;
	}

	private static DelegateInfo GetDelegateInfo(Type type, [CallerMemberName] string caller = null!)
	{
		if (type.IsAbstract || !type.IsAssignableTo(typeof(Delegate)))
			throw new ArgumentException($"Type {type} is not a delegate type.");

		if (type.IsGenericTypeDefinition)
			throw new ArgumentException($"Cannot call {caller} on a generic type definition.");

		if (!DelegateInfos.TryGetValue(type, out var info))
		{
			var infoType = typeof(DelegateInfo<>).MakeGenericType(type);
			RuntimeHelpers.RunClassConstructor(infoType.TypeHandle);
			info = DelegateInfos[type];
		}

		return info;
	}

	private static DelegateInfo CreateDelegateInfo(Type type)
	{
		var invokeMethod = type.GetMethod("Invoke", ReflectionHelper.DeclaredPublicInstance) ?? throw new MissingMethodException(type.FullName, "Invoke");
		var argumentTypes = invokeMethod.GetParameters().Select(v => v.ParameterType).ToImmutableArray();
		var instance = new DelegateInfo(invokeMethod, invokeMethod.ReturnType, argumentTypes);
		DelegateInfos[type] = instance;
		return instance;
	}

	private record DelegateInfo(MethodInfo InvokeMethod, Type ReturnType, ImmutableArray<Type> ArgumentTypes);

	private static class DelegateInfo<T> where T : Delegate
	{
		public static readonly DelegateInfo Instance = CreateDelegateInfo(typeof(T));
	}
}
