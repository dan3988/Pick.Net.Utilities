using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetUtilities.Collections;

public static class DictionaryHelper
{
	public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
		where TKey : notnull
	{
		return EmptyDictionary<TKey, TValue>.Instance;
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

	internal sealed class EmptyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
		where TKey : notnull
	{
		public readonly static EmptyDictionary<TKey, TValue> Instance = new();

		private readonly static IEnumerable<KeyValuePair<TKey, TValue>> en = Array.Empty<KeyValuePair<TKey, TValue>>();

		public TValue this[TKey key] => throw new KeyNotFoundException("EmptyDictionary instance will never have values.");

		public int Count => 0;

		public IEnumerable<TKey> Keys => Array.Empty<TKey>();

		public IEnumerable<TValue> Values => Array.Empty<TValue>();

		public bool ContainsKey(TKey key) => false;

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) { value = default!; return false; }

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => en.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => en.GetEnumerator();
	}
}