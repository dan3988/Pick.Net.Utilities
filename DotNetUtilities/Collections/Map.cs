using System.Diagnostics.CodeAnalysis;

namespace DotNetUtilities.Collections;

public sealed class Map<TKey, TValue> : BaseMap<TKey, TValue>, IMap, IMap<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	object? IMap.this[object key]
	{
		get => key is TKey k ? base[k] : null;
		set
		{
			if (key is not TKey k)
				throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

			if (value is not TValue v)
				throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

			this[k] = v;
		}
	}

	public new TValue? this[TKey key]
	{
		get => base[key];
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

	public override bool IsReadOnly => false;

	public Map() : this(0, null) { }

	public Map(int capacity) : this(capacity, null) { }

	public Map(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

	public Map(int capacity, IEqualityComparer<TKey>? comparer) : base(capacity, comparer) { }

	public Map(Dictionary<TKey, TValue> values) : base(new(values, values.Comparer)) { }

	public void Add(IMapEntry<TKey, TValue> item)
		=> Add(item.Key, item.Value);

	public void Add(TKey key, TValue value)
		=> dictionary.Add(key, new(key, value));

	public void Clear()
		=> dictionary.Clear();

	public bool TryAdd(TKey key, TValue value)
		=> dictionary.TryAdd(key, new(key, value));

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

	private bool Contains(TKey key, TValue value, IMapEntry entry)
		=> dictionary.TryGetValue(key, out var found) && (ReferenceEquals(entry, found) || EqualityComparer<TValue>.Default.Equals(found.Value, value));

	private bool Remove(TKey key, TValue value, IMapEntry entry)
	{
		if (Contains(key, value, entry))
		{
			dictionary.Remove(key);
			return true;
		}

		return false;
	}

	bool ICollection<IMapEntry>.Contains(IMapEntry item)
		=> item.Key is TKey key && item.Value is TValue value && Contains(key, value, item);

	bool ICollection<IMapEntry<TKey, TValue>>.Contains(IMapEntry<TKey, TValue> item)
		=> Contains(item.Key, item.Value, item);

	bool IMap.Remove(object key)
		=> key is TKey k && Remove(k);

	bool ICollection<IMapEntry<TKey, TValue>>.Remove(IMapEntry<TKey, TValue> item)
		=> Remove(item.Key, item.Value, item);

	bool ICollection<IMapEntry>.Remove(IMapEntry item)
		=> item.Key is TKey key && item.Value is TValue value && Contains(key, value, item);

	void ICollection<IMapEntry>.Add(IMapEntry item)
		=> ((IMap)this).Add(item.Key, item.Value);

	void IMap.Add(object key, object? value)
	{
		if (key is not TKey k)
			throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

		if (value is not TValue v)
			throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

		dictionary.Add(k, new(k, v));
	}
}