namespace DotNetUtilities.Collections;

public interface IReadOnlyMapEntry
{
	object Key { get; }

	object Value { get; }
}

public interface IReadOnlyMapEntry<out TKey, out TValue> : IReadOnlyMapEntry
	where TKey : notnull
	where TValue : class
{
	new TKey Key { get; }

	new TValue Value { get; }
}
