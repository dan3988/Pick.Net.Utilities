using System.Reflection;

namespace Pick.Net.Utilities.Reflection;

partial class DelegateHelper
{
	private static readonly Type[] FuncTypes =
	[
		typeof(Func<>),
		typeof(Func<,>),
		typeof(Func<,,>),
		typeof(Func<,,,>),
		typeof(Func<,,,,>),
		typeof(Func<,,,,,>),
		typeof(Func<,,,,,,>),
		typeof(Func<,,,,,,,>),
		typeof(Func<,,,,,,,,>),
		typeof(Func<,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,,,,,>),
		typeof(Func<,,,,,,,,,,,,,,,,>),
	];

	public static Type GetFuncType(Type returnType, params Type[] argumentTypes)
	{
		if (FuncTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Func<> class with {argumentTypes.Length} parameters does not exist");

		var count = argumentTypes.Length;
		Array.Resize(ref argumentTypes, count + 1);
		argumentTypes[count] = returnType;
		return FuncTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	#region CreateFunc Overloads

	public static Func<TResult> CreateFunc<TResult>(MethodInfo method)
		=> CreateDelegate<Func<TResult>>(method);

	public static Func<T1, TResult> CreateFunc<T1, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, TResult>>(method);

	public static Func<T1, T2, TResult> CreateFunc<T1, T2, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, TResult>>(method);

	public static Func<T1, T2, T3, TResult> CreateFunc<T1, T2, T3, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, TResult>>(method);

	public static Func<T1, T2, T3, T4, TResult> CreateFunc<T1, T2, T3, T4, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, TResult> CreateFunc<T1, T2, T3, T4, T5, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(method);

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodInfo method)
		=> CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(method);

	#endregion CreateFunc Overloads

	#region CreateFuncType Overloads

	public static Type CreateFuncType<TResult>(Type result)
		=> typeof(Func<>).MakeGenericType(result);

	public static Type CreateFuncType(Type arg1, Type result)
		=> typeof(Func<,>).MakeGenericType(arg1, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type result)
		=> typeof(Func<,,>).MakeGenericType(arg1, arg2, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type result)
		=> typeof(Func<,,,>).MakeGenericType(arg1, arg2, arg3, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type result)
		=> typeof(Func<,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type result)
		=> typeof(Func<,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type result)
		=> typeof(Func<,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type result)
		=> typeof(Func<,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type result)
		=> typeof(Func<,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type result)
		=> typeof(Func<,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type result)
		=> typeof(Func<,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type result)
		=> typeof(Func<,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type result)
		=> typeof(Func<,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type result)
		=> typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14, Type result)
		=> typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14, Type arg15, Type result)
		=> typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, result);

	public static Type CreateFuncType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14, Type arg15, Type arg16, Type result)
		=> typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, result);

	#endregion CreateFuncType Overloads
}
