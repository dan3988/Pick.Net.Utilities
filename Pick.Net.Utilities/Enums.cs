using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Pick.Net.Utilities;

public static class Enums
{
	/// <summary>
	/// Gets a cached <see cref="ImmutableArray{T}"/> containing all the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableArray<T> GetValues<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyValues;

	/// <summary>
	/// Gets a cached <see cref="ImmutableArray{string}"/> containing the names of the constant values defined in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	public static ImmutableArray<string> GetNames<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyNames;

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

	internal static readonly string[] Names = Enum.GetNames<T>();
	internal static readonly ImmutableArray<string> ReadOnlyNames = ImmutableArray.Create(Names);
}