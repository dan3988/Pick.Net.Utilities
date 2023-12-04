using System.Collections;

namespace Pick.Net.Utilities.Collections;

/// <summary>
/// Abstract class that implements all collection interfaces, but only supports the read-only operations
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AbstractReadOnlyCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
{
	protected static NotSupportedException Exception() => new("Collection is read-only");

	public abstract int Count { get; }

	#region Explicit properties

	bool ICollection<T>.IsReadOnly => true;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	#endregion

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public abstract IEnumerator<T> GetEnumerator();

	public virtual bool Contains(T item)
	{
		foreach (var value in this)
			if (EqualityComparer<T>.Default.Equals(item, value))
				return true;

		return false;
	}

	public virtual void CopyTo(T[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public virtual void CopyTo(Array array, int index)
	{
		foreach (var value in this)
			array.SetValue(value, index++);
	}

	#region unsupported methods

	void ICollection<T>.Add(T item) => throw Exception();

	void ICollection<T>.Clear() => throw Exception();

	bool ICollection<T>.Remove(T item) => throw Exception();

	#endregion
}
