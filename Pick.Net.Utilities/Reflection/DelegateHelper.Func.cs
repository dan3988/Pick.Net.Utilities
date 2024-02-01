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

	public static Type CreateFuncType(Type returnType, params Type[] argumentTypes)
	{
		if (FuncTypes.Length < argumentTypes.Length)
			throw new ArgumentException($"Func<> class with {argumentTypes.Length} parameters does not exist");

		var count = argumentTypes.Length;
		Array.Resize(ref argumentTypes, count + 1);
		argumentTypes[count] = returnType;
		return FuncTypes[argumentTypes.Length].MakeGenericType(argumentTypes);
	}

	#region CreateFunc Overloads

	/// <summary>
	/// Creates an <see cref="Func{TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{TResult}"/>
	public static Func<TResult> CreateFunc<TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T, TResult}"/>
	public static Func<T, TResult> CreateFunc<T, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, TResult}"/>
	public static Func<T1, T2, TResult> CreateFunc<T1, T2, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, TResult}"/>
	public static Func<T1, T2, T3, TResult> CreateFunc<T1, T2, T3, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, TResult}"/>
	public static Func<T1, T2, T3, T4, TResult> CreateFunc<T1, T2, T3, T4, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, TResult> CreateFunc<T1, T2, T3, T4, T5, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}()"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this MethodInfo method)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>();

	/// <summary>
	/// Creates an <see cref="Func{TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{TResult}"/>
	public static Func<TResult> CreateFunc<TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T, TResult}"/>
	public static Func<T, TResult> CreateFunc<T, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, TResult}"/>
	public static Func<T1, T2, TResult> CreateFunc<T1, T2, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, TResult}"/>
	public static Func<T1, T2, T3, TResult> CreateFunc<T1, T2, T3, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, TResult}"/>
	public static Func<T1, T2, T3, T4, TResult> CreateFunc<T1, T2, T3, T4, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, TResult> CreateFunc<T1, T2, T3, T4, T5, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(target);

	/// <summary>
	/// Creates an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/> delegate to represent <paramref name="method"/>
	/// </summary>
	/// <inheritdoc cref="MethodInfo.CreateDelegate{T}(object?)"/>
	/// <inheritdoc cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult}"/>
	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> CreateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this MethodInfo method, object? target)
		=> method.CreateDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>(target);

	#endregion CreateFunc Overloads

	#region CreateFuncType Overloads

	public static Type CreateFuncType(Type result)
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
