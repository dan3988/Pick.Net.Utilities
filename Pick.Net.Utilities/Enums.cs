using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Pick.Net.Utilities;

public static unsafe partial class Enums
{
	/// <summary>
	/// Gets a cached <see cref="ImmutableArray{T}"/> containing all the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableArray<T> GetValues<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyValues;

	/// <summary>
	/// Gets a cached <see cref="ReadOnlyCollection{T}"/> containing all the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableList<T> GetValueList<T>() where T : unmanaged, Enum
		=> Enums<T>.BoxedValues;

	/// <summary>
	/// Gets a cached <see cref="ImmutableArray{T}"/> containing the names of the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableArray<string> GetNames<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyNames;

	/// <summary>
	/// Gets a cached <see cref="ReadOnlyCollection{T}"/> containing the names of the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableList<string> GetNameList<T>() where T : unmanaged, Enum
		=> Enums<T>.BoxedNames;

	/// <summary>
	/// Gets the underlying TypeCode of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static TypeCode GetTypeCode<T>() where T : unmanaged, Enum
		=> Enums<T>.TypeCode;

	/// <summary>
	/// The same as <see cref="Enum.HasFlag(Enum)"/> but does not box the values.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static bool HasFlagsFast<T>(this T value, T flag) where T : unmanaged, Enum
		=> Enums<T>.Helper.HasFlag(&value, &flag);

	/// <summary>
	/// Get the smallest and largest values declared in the enum <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static (T Min, T Max) GetMinAndMaxValues<T>() where T : unmanaged, Enum
		=> Enums<T>.MinMax;

	/// <summary>
	/// Get the smallest value declared in the enum <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static T GetMinValue<T>() where T : unmanaged, Enum
		=> Enums<T>.MinMax.Min;

	/// <summary>
	/// Get the largest value declared in the enum <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static T GetMaxValue<T>() where T : unmanaged, Enum
		=> Enums<T>.MinMax.Max;

	public static bool CopyTo<T>(this T value, Span<byte> span) where T : unmanaged, Enum
	{
		var size = Enums<T>.Size;
		if (size > span.Length)
			return false;

		new Span<byte>(&value, size).CopyTo(span);
		return true;
	}

	public static T Add<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.Add(&x, &y, &result);
		return result;
	}

	public static T Subtract<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.Subtract(&x, &y, &result);
		return result;
	}

	public static T Multiply<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.Multiply(&x, &y, &result);
		return result;
	}

	public static T Divide<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.Divide(&x, &y, &result);
		return result;
	}

	public static T BitwiseAnd<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.BitwiseAnd(&x, &y, &result);
		return result;
	}

	public static T BitwiseOr<T>(this T x, T y) where T : unmanaged, Enum
	{
		T result;
		Enums<T>.Helper.BitwiseOr(&x, &y, &result);
		return result;
	}
}

/// <summary>
/// Stores values relating to the enum <typeparamref name="T"/>.
/// </summary>
// ReSharper disable StaticMemberInGenericType
internal static unsafe class Enums<T> where T : unmanaged, Enum
{
	internal static readonly Type UnderlyingType = typeof(T).GetEnumUnderlyingType();
	internal static readonly TypeCode TypeCode = Conversion.GetTypeCodeFast(UnderlyingType);
	internal static readonly int Size = sizeof(T);
	internal static readonly EnumHelper Helper = EnumHelper.ForTypeCode(TypeCode);

	internal static readonly T[] Values = Enum.GetValues<T>();
	internal static readonly ImmutableArray<T> ReadOnlyValues = ImmutableArray.Create(Values);
	private static ImmutableList<T>? _boxedValues;
	internal static ImmutableList<T> BoxedValues => _boxedValues ??= ImmutableList.CreateRange(Values);

	internal static readonly string[] Names = Enum.GetNames<T>();
	internal static readonly ImmutableArray<string> ReadOnlyNames = ImmutableArray.Create(Names);
	private static ImmutableList<string>? _boxedNames;
	internal static ImmutableList<string> BoxedNames => _boxedNames ??= ImmutableList.CreateRange(Names);

	private static (T Min, T Max)? _minMax;
	internal static (T Min, T Max) MinMax => _minMax ??= CalculateMinMax();

	private static (T Min, T Max) CalculateMinMax()
	{
		var min = default(T);
		var max = default(T);

		foreach (var value in Values)
		{
			if (Helper.Compare(&value, &min) < 0)
				min = value;

			if (Helper.Compare(&value, &max) > 0)
				max = value;
		}

		return (min, max);
	}
}