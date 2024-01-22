using System.Collections;
using System.Diagnostics;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

[DebuggerDisplay("{DebugName,nq}<{DeclaringType.FullName,nq}> Count = {Count}")]
public abstract class ReflectedPropertyCollection : ICollection, IReflectedPropertyCollection
{
	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	public abstract int Count { get; }

	public abstract Type DeclaringType { get; }

	internal abstract string DebugName { get; }

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	public abstract IEnumerator<IReflectedProperty> GetEnumerator();

	public abstract void CopyTo(Array array, int index);

	public abstract IReflectedProperty GetProperty(ReadOnlySpan<char> key);

	public abstract IReflectedProperty? TryGetProperty(ReadOnlySpan<char> key);
}
