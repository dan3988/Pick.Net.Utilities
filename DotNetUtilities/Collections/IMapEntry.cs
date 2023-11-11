namespace DotNetUtilities.Collections;

public interface IMapEntry<out TKey, TValue> : IReadOnlyMapEntry<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	new TValue Value { get; set; }
}
