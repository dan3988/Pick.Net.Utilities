using System.Collections;

namespace DotNetUtilities.Collections;

public class ReadOnlyMap<TKey, TValue> : IReadOnlyMap<TKey, TValue>, IReadOnlyMap, ICollection
	where TKey : notnull
	where TValue : class
{
	private readonly IReadOnlyMap<TKey, TValue> _map;

	public ReadOnlyMap(IReadOnlyMap<TKey, TValue> map)
	{
		_map = map;
	}

	public int Count => _map.Count;

	public TValue? this[TKey key] => _map[key];

	public IReadOnlyCollection<TKey> Keys => _map.Keys;

	public IReadOnlyCollection<TValue> Values => _map.Values;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IReadOnlyMap.this[object key] => key is TKey k ? _map[k] : null;

	public bool ContainsKey(TKey key)
		=> _map.ContainsKey(key);

	public void CopyTo(Array array, int index)
	{
		foreach (var item in _map)
			array.SetValue(item, index++);
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IMapEntry> IEnumerable<IMapEntry>.GetEnumerator()
		=> GetEnumerator();

	public IEnumerator<IMapEntry<TKey, TValue>> GetEnumerator()
		=> _map.GetEnumerator();
}
