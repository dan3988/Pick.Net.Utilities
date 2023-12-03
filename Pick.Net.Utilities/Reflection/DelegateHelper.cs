using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Reflection;

public static partial class DelegateHelper
{
	private static readonly Dictionary<Type, DelegateInfo> DelegateInfos = [];

	public static T CreateDelegate<T>(MethodInfo method) where T : Delegate
		=> (T)Delegate.CreateDelegate(typeof(T), method);

	public static Type GetActionType(params Type[] argumentTypes)
	{
		if (ActionTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Action<> class with {argumentTypes.Length} parameters does not exist");

		return ActionTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	public static Type GetFuncType(Type returnType, params Type[] argumentTypes)
	{
		if (FuncTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Func<> class with {argumentTypes.Length} parameters does not exist");

		var count = argumentTypes.Length;
		Array.Resize(ref argumentTypes, count + 1);
		argumentTypes[count] = returnType;
		return FuncTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	public static Type[] GetArgumentTypes<T>() where T : Delegate
		=> (Type[])DelegateInfo<T>.Instance.ArgumentTypes.Clone();

	public static Type GetReturnType<T>() where T : Delegate
		=> DelegateInfo<T>.Instance.ReturnType;

	public static Type[] GetArgumentTypes(Type type)
		=> (Type[])GetDelegateInfo(type).ArgumentTypes.Clone();

	public static Type GetReturnType<T>(Type type) where T : Delegate
		=> GetDelegateInfo(type).ReturnType;

	private static DelegateInfo GetDelegateInfo(Type type)
	{
		if (!type.IsAssignableTo(typeof(Delegate)))
			throw new ArgumentException($"Type {type} is not a delegate type.");

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
		var argumentTypes = invokeMethod.GetParameters().Select(v => v.ParameterType).ToArray();
		var instance = new DelegateInfo(invokeMethod, invokeMethod.ReturnType, argumentTypes);
		DelegateInfos[type] = instance;
		return instance;
	}

	private record DelegateInfo(MethodInfo InvokeMethod, Type ReturnType, Type[] ArgumentTypes);

	private static class DelegateInfo<T> where T : Delegate
	{
		public static readonly DelegateInfo Instance = CreateDelegateInfo(typeof(T));
	}
}
