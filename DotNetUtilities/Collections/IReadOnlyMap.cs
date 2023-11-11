namespace DotNetUtilities.Collections;

/// <summary>
/// Non-generic implementation of <see cref="IReadOnlyMap{TKey, TValue}"/>.
/// </summary>
public interface IReadOnlyMap : IReadOnlyCollection<IReadOnlyMapEntry>
{
	object? this[object key] { get; }

	IReadOnlyMapEntry? GetEntry(object key);
}

/// <summary>
/// Similar API to <see cref="Dictionary{TKey, TValue}"/> but supports covariance. I.E. <br/><c>IReadOnlyMap&lt;string, object&gt; map = new Map&lt;string, string&gt;();</c>
/// </summary>
public interface IReadOnlyMap<TKey, out TValue> : IReadOnlyCollection<IReadOnlyMapEntry<TKey, TValue>>
	where TKey : notnull
	where TValue : class
{
	TValue? this[TKey key] { get; }

	IReadOnlyCollection<TKey> Keys { get; }

	IReadOnlyCollection<TValue> Values { get; }

	IReadOnlyMapEntry<TKey, TValue>? GetEntry(TKey key);

	bool ContainsKey(TKey key);
}
