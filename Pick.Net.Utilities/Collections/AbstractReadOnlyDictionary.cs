using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Collections;

public abstract class AbstractReadOnlyDictionary<TKey, TValue> : AbstractReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
{
	public virtual TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

	private KeyCollection? _keys;
	public AbstractReadOnlyCollection<TKey> Keys => _keys ??= new(this);

	private ValueCollection? _values;
	public AbstractReadOnlyCollection<TValue> Values => _values ??= new(this);

	#region Explicit Properties

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

	ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get => this[key];
		set => throw Exception();
	}

	#endregion Explicit Properties

	public virtual bool ContainsKey(TKey key)
		=> TryGetValue(key, out _);

	public abstract bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

	#region Unsuppoorted Methods

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		=> throw Exception();

	bool IDictionary<TKey, TValue>.Remove(TKey key)
		=> throw Exception();

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		=> throw Exception();

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		=> throw Exception();

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		=> throw Exception();

	#endregion Unsuppoorted Methods

	private sealed class KeyCollection(AbstractReadOnlyDictionary<TKey, TValue> owner) : AbstractReadOnlyCollection<TKey>
	{
		public override int Count => owner.Count;

		public override IEnumerator<TKey> GetEnumerator()
		{
			foreach (var pair in owner)
				yield return pair.Key;
		}
	}

	private sealed class ValueCollection(AbstractReadOnlyDictionary<TKey, TValue> owner) : AbstractReadOnlyCollection<TValue>
	{
		public override int Count => owner.Count;

		public override IEnumerator<TValue> GetEnumerator()
		{
			foreach (var pair in owner)
				yield return pair.Value;
		}
	}
}
