using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Reflection;

public static partial class DelegateHelper
{
	private static readonly Dictionary<Type, DelegateInfo> DelegateInfos = [];

	public static T CreateDelegate<T>(MethodInfo method) where T : Delegate
		=> (T)Delegate.CreateDelegate(typeof(T), method);

	public static ImmutableArray<Type> GetArgumentTypes<T>() where T : Delegate
		=> GetDelegateInfo<T>().ArgumentTypes;

	public static ImmutableArray<Type> GetArgumentTypes(Type type)
		=> GetDelegateInfo(type).ArgumentTypes;

	public static Type GetReturnType<T>() where T : Delegate
		=> GetDelegateInfo<T>().ReturnType;

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
