﻿using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Pick.Net.Utilities;

public static unsafe class Enums
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
	public static ReadOnlyCollection<T> GetValueList<T>() where T : unmanaged, Enum
		=> Enums<T>.BoxedValues ??= new(Enums<T>.Values);

	/// <summary>
	/// Gets a cached <see cref="ImmutableArray{string}"/> containing the names of the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableArray<string> GetNames<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyNames;

	/// <summary>
	/// Gets a cached <see cref="ReadOnlyCollection{string}"/> containing the names of the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ReadOnlyCollection<string> GetNameList<T>() where T : unmanaged, Enum
		=> Enums<T>.BoxedNames ??= new(Enums<T>.Names);

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
	public static unsafe bool HasFlagsFast<T>(this T value, T flag) where T : unmanaged, Enum
		=> Enums<T>.Helper.HasFlag(&value, &flag);

	/// <summary>
	/// Get the smallest value declared in the enum <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static T GetMinValue<T>() where T : unmanaged, Enum
		=> Enums<T>.MinValue;

	/// <summary>
	/// Get the largest value declared in the enum <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static T GetMaxValue<T>() where T : unmanaged, Enum
		=> Enums<T>.MaxValue;

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
internal static unsafe class Enums<T> where T : unmanaged, Enum
{
	internal static readonly Type UnderlyingType = typeof(T).GetEnumUnderlyingType();
	internal static readonly TypeCode TypeCode = Conversion.GetTypeCodeFast(UnderlyingType);
	internal static readonly int Size = sizeof(T);
	internal static readonly EnumHelper Helper = EnumHelper.ForTypeCode(TypeCode);

	internal static readonly T[] Values = Enum.GetValues<T>();
	internal static readonly ImmutableArray<T> ReadOnlyValues = ImmutableArray.Create(Values);
	internal static ReadOnlyCollection<T>? BoxedValues;

	internal static readonly string[] Names = Enum.GetNames<T>();
	internal static readonly ImmutableArray<string> ReadOnlyNames = ImmutableArray.Create(Names);
	internal static ReadOnlyCollection<string>? BoxedNames;

	internal static readonly T MinValue;
	internal static readonly T MaxValue;

	static Enums()
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

		MinValue = min;
		MaxValue = max;
	}
}