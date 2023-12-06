using System.Reflection;

namespace Pick.Net.Utilities.Reflection;

partial class DelegateHelper
{
	private static readonly Type[] ActionTypes =
	[
		typeof(Action),
		typeof(Action<>),
		typeof(Action<,>),
		typeof(Action<,,>),
		typeof(Action<,,,>),
		typeof(Action<,,,,>),
		typeof(Action<,,,,,>),
		typeof(Action<,,,,,,>),
		typeof(Action<,,,,,,,>),
		typeof(Action<,,,,,,,,>),
		typeof(Action<,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,,,,,>),
		typeof(Action<,,,,,,,,,,,,,,,>),
	];

	public static Type GetActionType(params Type[] argumentTypes)
	{
		if (ActionTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Action<> class with {argumentTypes.Length} parameters does not exist");

		return ActionTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	#region CreateAction Overloads

	public static Action CreateAction(MethodInfo method)
		=> CreateDelegate<Action>(method);

	public static Action<T1> CreateAction<T1>(MethodInfo method)
		=> CreateDelegate<Action<T1>>(method);

	public static Action<T1, T2> CreateAction<T1, T2>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2>>(method);

	public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3>>(method);

	public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4>>(method);

	public static Action<T1, T2, T3, T4, T5> CreateAction<T1, T2, T3, T4, T5>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5>>(method);

	public static Action<T1, T2, T3, T4, T5, T6> CreateAction<T1, T2, T3, T4, T5, T6>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7> CreateAction<T1, T2, T3, T4, T5, T6, T7>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(method);

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodInfo method)
		=> CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(method);

	#endregion CreateAction Overloads
}
