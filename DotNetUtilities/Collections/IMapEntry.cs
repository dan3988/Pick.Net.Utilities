namespace DotNetUtilities.Collections;

public interface IMapEntry

{
	object Key { get; }

	object? Value { get; }
}

public interface IMapEntry<out TKey, out TValue> : IMapEntry
	where TKey : notnull
{
	new TKey Key { get; }

	new TValue Value { get; }
}
