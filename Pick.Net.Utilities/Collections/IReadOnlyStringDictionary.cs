using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Collections;

public interface IReadOnlyStringDictionary<T> : IReadOnlyDictionary<string, T>
{
	IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => Keys;

	IEnumerable<T> IReadOnlyDictionary<string, T>.Values => Values;

	StringComparison Comparison { get; }

	new IReadOnlyStringCollection Keys { get; }

	new IReadOnlyCollection<T> Values { get; }

	T this[ReadOnlySpan<char> key] { get; }

	bool ContainsKey(ReadOnlySpan<char> key);

	bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value);
}
