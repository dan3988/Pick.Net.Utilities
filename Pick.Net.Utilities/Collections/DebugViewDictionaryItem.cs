using System.Diagnostics;

namespace Pick.Net.Utilities.Collections;

[DebuggerDisplay("{Value}", Name = "[{Key}]")]
internal readonly record struct DebugViewDictionaryItem<TKey, TValue>([property: DebuggerBrowsable(DebuggerBrowsableState.Collapsed)] TKey Key, [property: DebuggerBrowsable(DebuggerBrowsableState.Collapsed)] TValue Value)
{
	public DebugViewDictionaryItem(KeyValuePair<TKey, TValue> keyValue) : this(keyValue.Key, keyValue.Value)
	{
	}
}
