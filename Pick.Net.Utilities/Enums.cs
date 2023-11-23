using System.Collections.ObjectModel;

namespace Pick.Net.Utilities;

public static class Enums
{
	public static ReadOnlyCollection<T> GetValues<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyValues;

	public static ReadOnlyCollection<string> GetNames<T>() where T : unmanaged, Enum
		=> Enums<T>.ReadOnlyNames;

	public static TypeCode GetTypeCode<T>() where T : unmanaged, Enum
		=> Enums<T>.TypeCode;

	public static unsafe bool HasFlagsFast<T>(this T value, T flag) where T : unmanaged, Enum
		=> Enums<T>.Helper.HasFlag(&value, &flag);
}

internal static unsafe class Enums<T> where T : unmanaged, Enum
{
	internal static readonly Type UnderlyingType = typeof(T).GetEnumUnderlyingType();
	internal static readonly TypeCode TypeCode = Conversion.GetTypeCodeFast(UnderlyingType);
	internal static readonly int Size = sizeof(T);
	internal static readonly EnumHelper Helper = EnumHelper.ForTypeCode(TypeCode);

	internal static readonly T[] Values = Enum.GetValues<T>();
	internal static readonly ReadOnlyCollection<T> ReadOnlyValues = new(Values);

	internal static readonly string[] Names = Enum.GetNames<T>();
	internal static readonly ReadOnlyCollection<string> ReadOnlyNames = new(Names);
}