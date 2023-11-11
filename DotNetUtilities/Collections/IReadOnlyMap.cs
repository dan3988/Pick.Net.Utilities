namespace DotNetUtilities.Collections;

/// <summary>
/// Non-generic implementation of <see cref="IReadOnlyMap{TKey, TValue}"/>.
/// </summary>
public interface IReadOnlyMap : IReadOnlyCollection<IMapEntry>
{
	object? this[object key] { get; }
}

/// <summary>
/// Similar API to <see cref="Dictionary{TKey, TValue}"/> but supports covariance. I.E. <br/><c>IReadOnlyMap&lt;string, object&gt; map = new Map&lt;string, string&gt;();</c>
/// </summary>
public interface IReadOnlyMap<TKey, out TValue> : IReadOnlyCollection<IMapEntry<TKey, TValue>>
	where TKey : notnull
	where TValue : class
{
	TValue? this[TKey key] { get; }

	IReadOnlyCollection<TKey> Keys { get; }

	IReadOnlyCollection<TValue> Values { get; }

	bool ContainsKey(TKey key);
}
