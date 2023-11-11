namespace DotNetUtilities.Collections;

public record ReadOnlyMapEntry<TKey, TValue>(TKey Key, TValue Value) : IReadOnlyMapEntry<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	public ReadOnlyMapEntry(KeyValuePair<TKey, TValue> pair) : this(pair.Key, pair.Value)
	{
	}

	object IReadOnlyMapEntry.Key => Key;

	object IReadOnlyMapEntry.Value => Value;

	public TKey Key { get; } = Key;

	public TValue Value { get; private protected set; } = Value;

	public static implicit operator KeyValuePair<TKey, TValue>(ReadOnlyMapEntry<TKey, TValue> v) => new(v.Key, v.Value);
}

public record MapEntry<TKey, TValue>(TKey Key, TValue Value) : ReadOnlyMapEntry<TKey, TValue>(Key, Value), IMapEntry<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	public new TValue Value
	{
		get => base.Value;
		set => base.Value = value;
	}

	public MapEntry(KeyValuePair<TKey, TValue> pair) : this(pair.Key, pair.Value)
	{
	}
}
