namespace Pick.Net.Utilities;

internal static class TypeHelper<T>
{
	public static readonly Type? NullableUnderlyingType = Nullable.GetUnderlyingType(typeof(T));
	public static readonly bool IsNullable = NullableUnderlyingType !=null || !typeof(T).IsValueType;
}
