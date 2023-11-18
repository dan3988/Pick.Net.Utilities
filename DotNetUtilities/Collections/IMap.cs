using System.Diagnostics.CodeAnalysis;

namespace DotNetUtilities.Collections;

/// <summary>
/// Mutable extension of <see cref="IReadOnlyMap"/>
/// </summary>
public interface IMap : IReadOnlyMap
{
	new object? this[object key] { get; set; }

	void Add(object key, object? value);

	bool Remove(object key);
}

/// <summary>
/// Mutable extension of <see cref="IReadOnlyMap{TKey, TValue}"/>
/// </summary>
public interface IMap<TKey, TValue> : IReadOnlyMap<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	new TValue? this[TKey key] { get; set; }

	new IMapEntry<TKey, TValue>? GetEntry(TKey key);

	void Add(TKey key, TValue value);

	bool TryAdd(TKey key, TValue value);

	bool Remove(TKey key);

	bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);
}
