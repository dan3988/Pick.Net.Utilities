﻿using System.Collections;

namespace Pick.Net.Utilities.Collections;

public class ReadOnlyMap<TKey, TValue>(IReadOnlyMap<TKey, TValue> map) : IReadOnlyMap<TKey, TValue>, IReadOnlyMap, ICollection
	where TKey : notnull
	where TValue : class
{
	private static ReadOnlyMap<TKey, TValue>? _empty;

	public static ReadOnlyMap<TKey, TValue> Empty => _empty ??= new(new Map<TKey, TValue>());

	public int Count => map.Count;

	public TValue? this[TKey key] => map[key];

	public IReadOnlyCollection<TKey> Keys => map.Keys;

	public IReadOnlyCollection<TValue> Values => map.Values;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IReadOnlyMap.this[object key] => key is TKey k ? map[k] : null;

	IReadOnlyMapEntry? IReadOnlyMap.GetEntry(object key)
		=> key is TKey k ? map.GetEntry(k) : null;

	public IReadOnlyMapEntry<TKey, TValue>? GetEntry(TKey key)
		=> map.GetEntry(key);

	public bool ContainsKey(TKey key)
		=> map.ContainsKey(key);

	public void CopyTo(Array array, int index)
	{
		foreach (var item in map)
			array.SetValue(item, index++);
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IReadOnlyMapEntry> IEnumerable<IReadOnlyMapEntry>.GetEnumerator()
		=> GetEnumerator();

	public IEnumerator<IReadOnlyMapEntry<TKey, TValue>> GetEnumerator()
		=> map.GetEnumerator();
}
