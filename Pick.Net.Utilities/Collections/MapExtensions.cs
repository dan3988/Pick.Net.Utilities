namespace Pick.Net.Utilities.Collections;

public static class MapExtensions
{
	public static void Deconstruct<TKey, TValue>(this IReadOnlyMapEntry<TKey, TValue> entry, out TKey key, out TValue value)
		where TKey : notnull
		where TValue : class
	{
		key = entry.Key;
		value = entry.Value;
	}
}
