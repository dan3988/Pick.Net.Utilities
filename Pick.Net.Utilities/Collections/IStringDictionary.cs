using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Collections;

public interface IStringDictionary<T> : IDictionary<string, T>, IReadOnlyStringDictionary<T>
{
	bool TryAdd(string key, T value);

	bool TryAdd(string key, T value, [MaybeNullWhen(true)] out T existing);

	bool Remove(string key, [MaybeNullWhen(false)] out T value);

	bool Remove(ReadOnlySpan<char> key);

	bool Remove(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value);
}