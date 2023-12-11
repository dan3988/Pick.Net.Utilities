using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Collections;

public sealed class ReadOnlyStringDictionary<T>(IReadOnlyStringDictionary<T> values) : AbstractReadOnlyDictionary<string, T>, IStringDictionary<T>
{
	private static ReadOnlyStringDictionary<T>? _empty;

	public static ReadOnlyStringDictionary<T> Empty => _empty ??= new(new StringDictionary<T>());

	private KeyCollection? _keys;

	public new KeyCollection Keys => _keys ??= new(values);

	public override int Count => values.Count;

	public StringComparison Comparison => values.Comparison;

	public T this[ReadOnlySpan<char> key] => values[key];

	IReadOnlyStringCollection IReadOnlyStringDictionary<T>.Keys => Keys;

	IReadOnlyCollection<T> IReadOnlyStringDictionary<T>.Values => Values;

	protected override AbstractReadOnlyCollection<string> CreateKeys()
		=> Keys;

	public override bool TryGetValue(string key, [MaybeNullWhen(false)] out T value)
		=> values.TryGetValue(key, out value);

	public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value)
		=> values.TryGetValue(key, out value);

	public override bool ContainsKey(string key)
		=> values.ContainsKey(key);

	public bool ContainsKey(ReadOnlySpan<char> key)
		=> values.ContainsKey(key);

	public override IDictionaryEnumerator<string, T> GetEnumerator()
		=> CreateDictionaryEnumerator(values);

	#region Unsuppoorted Methods

	bool IStringDictionary<T>.TryAdd(string key, T value)
		=> throw Exception();

	bool IStringDictionary<T>.TryAdd(string key, T value, out T existing)
		=> throw Exception();

	bool IStringDictionary<T>.Remove(string key, out T value)
		=> throw Exception();

	bool IStringDictionary<T>.Remove(ReadOnlySpan<char> key)
		=> throw Exception();

	bool IStringDictionary<T>.Remove(ReadOnlySpan<char> key, out T value)
		=> throw Exception();

	#endregion Unsuppoorted Methods

	public sealed class KeyCollection(IReadOnlyStringDictionary<T> values) : AbstractReadOnlyCollection<string>, IReadOnlyStringCollection
	{
		public override int Count => values.Count;

		public override bool Contains(string item)
			=> values.ContainsKey(item);

		public bool Contains(ReadOnlySpan<char> value)
			=> values.ContainsKey(value);

		public override IEnumerator<string> GetEnumerator()
			=> values.Keys.GetEnumerator();
	}
}
