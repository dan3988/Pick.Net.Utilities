using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DotNetUtilities.Collections;

public class ObservableList<T> : ObservableCollection<T>
{
	private ReadOnlyObservableCollection<T>? readOnly;

	public ReadOnlyObservableCollection<T> ReadOnly => readOnly ??= new(this);

	public ObservableList()
	{
	}

	public ObservableList(IEnumerable<T> collection) : base(collection)
	{
	}

	public ObservableList(List<T> list) : base(list)
	{
	}

	public void AddRange(IEnumerable<T> items)
	{
		CheckReentrancy();

		var range = items.ToList();
		if (range.Count == 0)
			return;

		var startIndex = Items.Count;

		foreach (var item in range)
			Items.Add(item);

		OnPropertyChanged(new("Count"));
		OnPropertyChanged(new("Items[]"));
		OnCollectionChanged(new(NotifyCollectionChangedAction.Add, range, startIndex));
	}

	public void Reset(IEnumerable<T> items)
	{
		CheckReentrancy();

		var oldCount = Items.Count;

		Items.Clear();

		using var en = items.GetEnumerator();

		if (!en.MoveNext())
		{
			if (oldCount == 0)
				return;

			OnPropertyChanged(new("Count"));
			OnPropertyChanged(new("Items[]"));
			OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
			return;
		}

		do
		{
			Items.Add(en.Current);
		}
		while (en.MoveNext());

		if (Items.Count != oldCount)
			OnPropertyChanged(new("Count"));

		OnPropertyChanged(new("Items[]"));
		OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
	}
}