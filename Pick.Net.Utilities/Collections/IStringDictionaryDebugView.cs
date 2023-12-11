using System.Diagnostics;

namespace Pick.Net.Utilities.Collections;

internal sealed class IStringDictionaryDebugView<T>(IReadOnlyStringDictionary<T> _dict)
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public DebugViewDictionaryItem<string, T>[] Items => _dict.Select(v => new DebugViewDictionaryItem<string, T>(v)).ToArray();
}
