using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Collections;

/// <summary>
/// Abstract dictionary implementation that implements <see cref="IReadOnlyDictionary{TKey, TValue}"/>, and the methods of <see cref="IDictionary{TKey, TValue}"/> and <see cref="IDictionary"/> that do not modify the collection, throwing a <see cref="NotSupportedException"/> for those that do.
/// </summary>
/// <typeparam name="TKey">The type of key to use to look up values</typeparam>
/// <typeparam name="TValue">The type of value stored by this dictionary</typeparam>
public abstract class AbstractReadOnlyDictionary<TKey, TValue> : AbstractReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
	where TKey : notnull
{
	protected static IDictionaryEnumerator<TKey, TValue> CreateDictionaryEnumerator(IEnumerable<KeyValuePair<TKey, TValue>> source)
	{
		var en = source.GetEnumerator();
		return en as IDictionaryEnumerator<TKey, TValue> ?? new DictionaryEnumerator(en);
	}

	public virtual TValue this[TKey key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

	private AbstractReadOnlyCollection<TKey>? _keys;
	public AbstractReadOnlyCollection<TKey> Keys => _keys ??= CreateKeys();

	private AbstractReadOnlyCollection<TValue>? _values;
	public AbstractReadOnlyCollection<TValue> Values => _values ??= CreateValues();

	#region Explicit Properties

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

	ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

	ICollection IDictionary.Keys => Keys;

	ICollection IDictionary.Values => Values;

	bool IDictionary.IsFixedSize => true;

	bool IDictionary.IsReadOnly => true;

	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get => this[key];
		set => throw Exception();
	}

	object? IDictionary.this[object key]
	{
		get => key is TKey k ? this[k] : throw new KeyNotFoundException();
		set => throw Exception();
	}

	#endregion Explicit Properties

	protected virtual AbstractReadOnlyCollection<TKey> CreateKeys()
		=> new KeyCollection(this);

	protected virtual AbstractReadOnlyCollection<TValue> CreateValues()
		=> new ValueCollection(this);

	bool IDictionary.Contains(object key)
		=> key is TKey k && ContainsKey(k);

	public override abstract IDictionaryEnumerator<TKey, TValue> GetEnumerator();

	IDictionaryEnumerator IDictionary.GetEnumerator()
		=> GetEnumerator();

	public virtual bool ContainsKey(TKey key)
		=> TryGetValue(key, out _);

	public abstract bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

	#region Unsuppoorted Methods

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		=> throw Exception();

	void IDictionary.Add(object key, object? value)
		=> throw Exception();

	bool IDictionary<TKey, TValue>.Remove(TKey key)
		=> throw Exception();

	void IDictionary.Remove(object key)
		=> throw Exception();

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		=> throw Exception();

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		=> throw Exception();

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		=> throw Exception();

	void IDictionary.Clear()
		=> throw Exception();

	#endregion Unsuppoorted Methods

	private sealed class DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> source) : IDictionaryEnumerator<TKey, TValue>
	{
		public KeyValuePair<TKey, TValue> Current => source.Current;

		object IEnumerator.Current => source.Current;

		public void Dispose()
			=> source.Dispose();

		public bool MoveNext()
			=> source.MoveNext();

		public void Reset()
			=> source.Reset();
	}

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
