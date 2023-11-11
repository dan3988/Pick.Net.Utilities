using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetUtilities.Collections;

public sealed class Map<TKey, TValue> : IMap, IMap<TKey, TValue>, ICollection
	where TKey : notnull
	where TValue : class
{
	private sealed class ValueCollection : IReadOnlyCollection<TValue>
	{
		private readonly Map<TKey, TValue> owner;

		public int Count => owner.Count;

		public ValueCollection(Map<TKey, TValue> owner)
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

	public int Count => dictionary.Count;

	public IReadOnlyCollection<TKey> Keys => dictionary.Keys;

	public IReadOnlyCollection<TValue> Values => values ??= new(this);

	public TValue? this[TKey key]
	{
		get => dictionary.TryGetValue(key, out var entry) ? entry.Value : null;
		set
		{
			if (value == null)
			{
				dictionary.Remove(key);
			}
			else
			{
				dictionary[key] = new(key, value);
			}
		}
	}

	#region Explicit Property Implementations

	bool ICollection<IReadOnlyMapEntry>.IsReadOnly => false;

	bool ICollection<IMapEntry<TKey, TValue>>.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IReadOnlyMap.this[object key] => ((IMap)this)[key];

	object? IMap.this[object key]
	{
		get => key is TKey k ? this[k] : null;
		set
		{
			if (key is not TKey k)
				throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

			if (value is not TValue v)
				throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

			this[k] = v;
		}
	}

	#endregion

	private readonly Dictionary<TKey, MapEntry<TKey, TValue>> dictionary;
	private ValueCollection? values;

	public Map() : this(0, null)
	{
	}

	public Map(int capacity) : this(capacity, null)
	{
	}

	public Map(IEqualityComparer<TKey>? comparer) : this(0, comparer)
	{
	}

	public Map(int capacity, IEqualityComparer<TKey>? comparer)
	{
		dictionary = new(capacity, comparer);
	}

	public Map(Dictionary<TKey, TValue> values) : this(values, values.Comparer)
	{
	}

	public Map(IEnumerable<KeyValuePair<TKey, TValue>> values) : this(values, null)
	{
	}

	public Map(IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? comparer)
	{
		dictionary = new(values.Select(ToMapEntry), comparer);
	}

	public IMapEntry<TKey, TValue>? GetEntry(TKey key)
	{
		dictionary.TryGetValue(key, out var entry);
		return entry;
	}

	public void CopyTo(IReadOnlyMapEntry[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public void CopyTo(IMapEntry<TKey, TValue>[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public bool ContainsKey(TKey key)
		=> dictionary.ContainsKey(key);

	public void Add(IMapEntry<TKey, TValue> item)
		=> Add(item.Key, item.Value);

	public void Add(TKey key, TValue value)
		=> dictionary.Add(key, new(key, value));

	public bool TryAdd(TKey key, TValue value)
		=> dictionary.TryAdd(key, new(key, value));

	public void Clear()
		=> dictionary.Clear();

	public bool Remove(TKey key)
		=> dictionary.Remove(key);

	public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		if (dictionary.Remove(key, out var entry))
		{
			value = entry.Value;
			return true;
		}
		else
		{
			value = null;
			return false;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IReadOnlyMapEntry> IEnumerable<IReadOnlyMapEntry>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IReadOnlyMapEntry<TKey, TValue>> IEnumerable<IReadOnlyMapEntry<TKey, TValue>>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IMapEntry<TKey, TValue>> IEnumerable<IMapEntry<TKey, TValue>>.GetEnumerator()
		=> GetEnumerator();

	public IEnumerator<MapEntry<TKey, TValue>> GetEnumerator()
		=> dictionary.Values.GetEnumerator();

	private void CopyTo(Array array, int index)
	{
		foreach (var entry in dictionary.Values)
			array.SetValue(entry, index++);
	}

	private bool Contains(TKey key, TValue value, IReadOnlyMapEntry entry)
		=> dictionary.TryGetValue(key, out var found) && (ReferenceEquals(entry, found) || EqualityComparer<TValue>.Default.Equals(found.Value, value));

	private bool Remove(TKey key, TValue value, IReadOnlyMapEntry entry)
	{
		if (Contains(key, value, entry))
		{
			dictionary.Remove(key);
			return true;
		}

		return false;
	}

	#region Explicit Method Implementations

	void ICollection.CopyTo(Array array, int index)
		=> CopyTo(array, index);

	bool ICollection<IReadOnlyMapEntry>.Contains(IReadOnlyMapEntry item)
		=> item.Key is TKey key && item.Value is TValue value && Contains(key, value, item);

	bool ICollection<IMapEntry<TKey, TValue>>.Contains(IMapEntry<TKey, TValue> item)
		=> Contains(item.Key, item.Value, item);

	bool IMap.Remove(object key)
		=> key is TKey k && Remove(k);

	bool ICollection<IMapEntry<TKey, TValue>>.Remove(IMapEntry<TKey, TValue> item)
		=> Remove(item.Key, item.Value, item);

	bool ICollection<IReadOnlyMapEntry>.Remove(IReadOnlyMapEntry item)
		=> item.Key is TKey key && item.Value is TValue value && Contains(key, value, item);

	void ICollection<IReadOnlyMapEntry>.Add(IReadOnlyMapEntry item)
		=> ((IMap)this).Add(item.Key, item.Value);

	void IMap.Add(object key, object? value)
	{
		if (key is not TKey k)
			throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

		if (value is not TValue v)
			throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

		dictionary.Add(k, new(k, v));
	}

	IReadOnlyMapEntry<TKey, TValue>? IReadOnlyMap<TKey, TValue>.GetEntry(TKey key)
		=> GetEntry(key);

	IReadOnlyMapEntry? IReadOnlyMap.GetEntry(object key)
		=> key is TKey k ? GetEntry(k) : null;

	#endregion
}