using System.Diagnostics;

namespace Pick.Net.Utilities.Collections;

internal sealed class IMapDebugView<TKey, TValue>(IReadOnlyMap<TKey, TValue> dict)
	where TKey : notnull
	where TValue : class
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public DebugViewDictionaryItem<TKey, TValue>[] Items => dict.Select(v => new DebugViewDictionaryItem<TKey, TValue>(v.Key, v.Value)).ToArray();
}
