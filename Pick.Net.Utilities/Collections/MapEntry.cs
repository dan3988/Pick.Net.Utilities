namespace Pick.Net.Utilities.Collections;

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
