using System.Collections;

namespace Pick.Net.Utilities.Collections;

/// <summary>
/// Abstract collection implementation that implements <see cref="ICollection"/>, <see cref="IReadOnlyList{T}"/>, and the methods of <see cref="IList{T}"/> and <see cref="IList"/> that do not modify the collection, throwing a <see cref="NotSupportedException"/> for those that do.
/// </summary>
/// <typeparam name="T">The type of element in this list</typeparam>
public abstract class AbstractReadOnlyList<T> : AbstractReadOnlyCollection<T>, IList<T>, IList, IReadOnlyList<T>
{
	public abstract T this[int index] { get; }

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

	#endregion

	public override bool Contains(T item)
		=> IndexOf(item) >= 0;

	public virtual int IndexOf(T item)
	{
		for (int i = 0; i < Count; i++)
			if (EqualityComparer<T>.Default.Equals(item, this[i]))
				return i;

		return -1;
	}

	int IList.IndexOf(object? value) => value is T t ? IndexOf(t) : -1;

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	#region unsupported methods

	int IList.Add(object? value) => throw Exception();

	void IList.Clear() => throw Exception();

	bool IList.Contains(object? value) => throw Exception();

	void IList.Insert(int index, object? value) => throw Exception();

	void IList<T>.Insert(int index, T item) => throw Exception();

	void IList.Remove(object? value) => throw Exception();

	void IList.RemoveAt(int index) => throw Exception();

	void IList<T>.RemoveAt(int index) => throw Exception();

	#endregion
}
