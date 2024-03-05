using System.Diagnostics;

namespace Pick.Net.Utilities.Collections;

internal sealed class IStringDictionaryDebugView<T>(IReadOnlyStringDictionary<T> dict)
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public DebugViewDictionaryItem<string, T>[] Items => dict.Select(v => new DebugViewDictionaryItem<string, T>(v)).ToArray();
}
