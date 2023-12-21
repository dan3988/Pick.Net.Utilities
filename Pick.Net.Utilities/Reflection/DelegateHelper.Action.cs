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

	public static Type CreateAction(params Type[] argumentTypes)
	{
		if (ActionTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Action<> class with {argumentTypes.Length} parameters does not exist");

		return ActionTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	#region CreateAction Overloads

	/// <summary>
	/// Creates an <see cref="Action"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	public static Action CreateAction(this MethodInfo method)
		=> method.CreateDelegate<Action>();

	/// <summary>
	/// Creates an <see cref="Action{T1}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T}"/>
	public static Action<T> CreateAction<T>(this MethodInfo method)
		=> method.CreateDelegate<Action<T>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2}"/>
	public static Action<T1, T2> CreateAction<T1, T2>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3}"/>
	public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4}"/>
	public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5}"/>
	public static Action<T1, T2, T3, T4, T5> CreateAction<T1, T2, T3, T4, T5>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6}"/>
	public static Action<T1, T2, T3, T4, T5, T6> CreateAction<T1, T2, T3, T4, T5, T6>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7> CreateAction<T1, T2, T3, T4, T5, T6, T7>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>();

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this MethodInfo method)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>();

	/// <summary>
	/// Creates an <see cref="Action"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	public static Action CreateAction(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T}"/>
	public static Action<T> CreateAction<T>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2}"/>
	public static Action<T1, T2> CreateAction<T1, T2>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3}"/>
	public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4}"/>
	public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5}"/>
	public static Action<T1, T2, T3, T4, T5> CreateAction<T1, T2, T3, T4, T5>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6}"/>
	public static Action<T1, T2, T3, T4, T5, T6> CreateAction<T1, T2, T3, T4, T5, T6>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7> CreateAction<T1, T2, T3, T4, T5, T6, T7>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(target);

	/// <summary>
	/// Creates an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>
	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> CreateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>(target);

	#endregion CreateAction Overloads

	#region CreateFuncType Overloads

	public static Type CreateActionType(Type arg1)
		=> typeof(Action<>).MakeGenericType(arg1);

	public static Type CreateActionType(Type arg1, Type arg2)
		=> typeof(Action<,>).MakeGenericType(arg1, arg2);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3)
		=> typeof(Action<,,>).MakeGenericType(arg1, arg2, arg3);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4)
		=> typeof(Action<,,,>).MakeGenericType(arg1, arg2, arg3, arg4);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5)
		=> typeof(Action<,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6)
		=> typeof(Action<,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7)
		=> typeof(Action<,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8)
		=> typeof(Action<,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9)
		=> typeof(Action<,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10)
		=> typeof(Action<,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11)
		=> typeof(Action<,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12)
		=> typeof(Action<,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13)
		=> typeof(Action<,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14)
		=> typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14, Type arg15)
		=> typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

	public static Type CreateActionType(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8, Type arg9, Type arg10, Type arg11, Type arg12, Type arg13, Type arg14, Type arg15, Type arg16)
		=> typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

	#endregion CreateActionType Overloads
}
