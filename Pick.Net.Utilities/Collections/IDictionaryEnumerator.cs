using System.Collections;

namespace Pick.Net.Utilities.Collections;

public interface IDictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	where TKey : notnull
{
	DictionaryEntry IDictionaryEnumerator.Entry
	{
		get
		{
			var (key, value) = Current;
			return new(key, value);
		}
	}

	object IDictionaryEnumerator.Key => Current.Key;

	object? IDictionaryEnumerator.Value => Current.Value;

	new TKey Key => Current.Key;

	new TValue Value => Current.Value;
}
