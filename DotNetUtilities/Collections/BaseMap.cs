using System.Collections;

namespace DotNetUtilities.Collections;

public abstract class BaseMap<TKey, TValue> : IReadOnlyMap, IReadOnlyMap<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	private sealed class ValueCollection : IReadOnlyCollection<TValue>
	{
		private readonly BaseMap<TKey, TValue> owner;

		public int Count => owner.Count;

		public ValueCollection(BaseMap<TKey, TValue> owner)
		{
			this.owner = owner;
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		public IEnumerator<TValue> GetEnumerator()
			=> owner.dictionary.Values.Select(v => v.Value).GetEnumerator();
	}

	private static KeyValuePair<TKey, MapEntry<TKey, TValue>> ToMapEntry(KeyValuePair<TKey, TValue> pair)
		=> new(pair.Key, new(pair));

	private protected readonly Dictionary<TKey, MapEntry<TKey, TValue>> dictionary;
	private ValueCollection? values;

	public int Count => dictionary.Count;

	public abstract bool IsReadOnly { get; }

	public IReadOnlyCollection<TKey> Keys => dictionary.Keys;

	public IReadOnlyCollection<TValue> Values => values ??= new(this);

	object? IReadOnlyMap.this[object key] => key is TKey k ? this[k] : null;

	public TValue? this[TKey key] => dictionary.TryGetValue(key, out var value) ? value.Value : null;

	private protected BaseMap(int capacity, IEqualityComparer<TKey>? comparer)
	{
		dictionary = new(capacity, comparer);
	}

	private protected BaseMap(Dictionary<TKey, TValue> values)
	{
		dictionary = new(values.Select(ToMapEntry), values.Comparer);
	}

	public void CopyTo(IMapEntry[] array, int arrayIndex)
	{
		foreach (var entry in dictionary.Values)
			array[arrayIndex++] = entry;
	}

	public void CopyTo(IMapEntry<TKey, TValue>[] array, int arrayIndex)
	{
		foreach (var entry in dictionary.Values)
			array[arrayIndex++] = entry;
	}

	public bool ContainsKey(TKey key)
		=> dictionary.ContainsKey(key);

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IMapEntry> IEnumerable<IMapEntry>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IMapEntry<TKey, TValue>> IEnumerable<IMapEntry<TKey, TValue>>.GetEnumerator()
		=> GetEnumerator();

	public IEnumerator<MapEntry<TKey, TValue>> GetEnumerator()
		=> dictionary.Values.GetEnumerator();
}
