using System.Collections;

namespace Pick.Net.Utilities.Collections;

public abstract class AbstractReadOnlyList<T> : IList<T>, IList, IReadOnlyList<T>
{
	private static NotSupportedException Exception() => new("Collection is read-only");

	public abstract T this[int index] { get; }

	public abstract int Count { get; }

	#region Explicit properties

	object? IList.this[int index]
	{
		get => this[index];
		set => throw Exception();
	}

	T IList<T>.this[int index]
	{
		get => this[index];
		set => throw Exception();
	}

	bool IList.IsFixedSize => true;

	bool IList.IsReadOnly => true;

	bool ICollection<T>.IsReadOnly => true;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => throw new NotSupportedException();

	#endregion

	public abstract IEnumerator<T> GetEnumerator();

	public virtual bool Contains(T item)
		=> IndexOf(item) >= 0;

	public virtual int IndexOf(T item)
	{
		for (int i = 0; i < Count; i++)
			if (EqualityComparer<T>.Default.Equals(item, this[i]))
				return i;

		return -1;
	}

	public virtual void CopyTo(T[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public virtual void CopyTo(Array array, int index)
	{
		for (int i = 0; i < Count; i++)
			array.SetValue(this[i], index++);
	}

	int IList.IndexOf(object? value) => value is T t ? IndexOf(t) : -1;

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	#region unsupported methods

	int IList.Add(object? value) => throw Exception();

	void ICollection<T>.Add(T item) => throw Exception();

	void IList.Clear() => throw Exception();

	void ICollection<T>.Clear() => throw Exception();

	bool IList.Contains(object? value) => throw Exception();

	void IList.Insert(int index, object? value) => throw Exception();

	void IList<T>.Insert(int index, T item) => throw Exception();

	void IList.Remove(object? value) => throw Exception();

	bool ICollection<T>.Remove(T item) => throw Exception();

	void IList.RemoveAt(int index) => throw Exception();

	void IList<T>.RemoveAt(int index) => throw Exception();

	#endregion
}
