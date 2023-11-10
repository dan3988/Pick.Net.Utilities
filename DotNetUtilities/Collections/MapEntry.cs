namespace DotNetUtilities.Collections;

public sealed record MapEntry<TKey, TValue>(TKey Key, TValue Value) : IMapEntry<TKey, TValue>
	where TKey : notnull
{
	object IMapEntry.Key => Key;

	object? IMapEntry.Value => Value;

	public MapEntry(KeyValuePair<TKey, TValue> pair) : this(pair.Key, pair.Value)
	{
	}

	public static implicit operator KeyValuePair<TKey, TValue>(MapEntry<TKey, TValue> v) => new(v.Key, v.Value);
}