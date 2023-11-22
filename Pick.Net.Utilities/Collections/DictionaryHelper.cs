using System.Collections.ObjectModel;

namespace Pick.Net.Utilities.Collections;

public static class DictionaryHelper
{
	public static ReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull
	{
#if NET8_0_OR_GREATER
		return ReadOnlyDictionary<TKey, TValue>.Empty;
#else
		return EmptyDictionary<TKey, TValue>.Value;
	}

	private static class EmptyDictionary<TKey, TValue> where TKey : notnull
	{
		internal static readonly ReadOnlyDictionary<TKey, TValue> Value = new(new Dictionary<TKey, TValue>());
#endif
	}

	public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> v, TKey key, ref TValue value)
		where TKey : notnull
	{
		if (v.TryGetValue(key, out var val))
		{
			value = val;
			return true;
		}

		return false;
	}
}