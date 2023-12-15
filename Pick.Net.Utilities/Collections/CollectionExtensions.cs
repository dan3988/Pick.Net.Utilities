using System.Collections;
using System.Collections.Specialized;

namespace Pick.Net.Utilities.Collections;

using CCEventHandler = NotifyCollectionChangedEventHandler;
using CCEventArgs = NotifyCollectionChangedEventArgs;
using CCAction = NotifyCollectionChangedAction;

public static class CollectionExtensions
{
	public static void InvokeAdded(this CCEventHandler handler, INotifyCollectionChanged sender, object item, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Add, item, index));

	public static void InvokeAdded(this CCEventHandler handler, INotifyCollectionChanged sender, IList items, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Add, items, index));

	public static void InvokeReplaced(this CCEventHandler handler, INotifyCollectionChanged sender, object oldItem, object newItem, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Replace, newItem, oldItem, index));

	public static void InvokeRemoved(this CCEventHandler handler, INotifyCollectionChanged sender, object item, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Remove, item, index));

	public static void InvokeReset(this CCEventHandler handler, INotifyCollectionChanged sender)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Reset));

	private static bool IsNotNull<T>(T? item) where T : class
		=> item != null;

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
		=> source.Where(IsNotNull)!;

	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
	{
		if (collection is List<T> list)
		{
			list.AddRange(values);
			return;
		}

		foreach (var value in values)
			collection.Add(value);
	}

	public static void Deconstruct<T, TKey, TValue>(this T pair, out TKey key, out TValue value)
		where TKey : notnull
		where TValue : class
		where T : IReadOnlyMapEntry<TKey, TValue>
	{
		key = pair.Key;
		value = pair.Value;
	}

	public static void Deconstruct<TKey, TValue>(this IReadOnlyMapEntry<TKey, TValue> entry, out TKey key, out TValue value)
		where TKey : notnull
		where TValue : class
	{
		key = entry.Key;
		value = entry.Value;
	}

	public static StringDictionary<TIn> ToStringDictionary<TIn>(this IEnumerable<TIn> source, Func<TIn, string> keySelector, StringComparison comparison = StringComparison.Ordinal)
	{
		return new(source.Select(Selector), comparison);

		KeyValuePair<string, TIn> Selector(TIn item)
		{
			var key = keySelector.Invoke(item);
			return new(key, item);
		}
	}

	public static StringDictionary<TOut> ToStringDictionary<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, string> keySelector, Func<TIn, TOut> valueSelector, StringComparison comparison = StringComparison.Ordinal)
	{
		return new(source.Select(Selector), comparison);

		KeyValuePair<string, TOut> Selector(TIn item)
		{
			var key = keySelector.Invoke(item);
			var value = valueSelector.Invoke(item);
			return new(key, value);
		}
	}

	public static Map<TKey, TValue> ToMap<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		where TValue : class
	{
		return new(source.Select(Selector), comparer);

		KeyValuePair<TKey, TValue> Selector(TValue item)
		{
			var key = keySelector.Invoke(item);
			return new(key, item);
		}
	}

	public static Map<TKey, TValue> ToMap<TIn, TKey, TValue>(this IEnumerable<TIn> source, Func<TIn, TKey> keySelector, Func<TIn, TValue> valueSelector, IEqualityComparer<TKey>? comparer = null)
		where TKey : notnull
		where TValue : class
	{
		return new(source.Select(Selector), comparer);

		KeyValuePair<TKey, TValue> Selector(TIn item)
		{
			var key = keySelector.Invoke(item);
			var value = valueSelector.Invoke(item);
			return new(key, value);
		}
	}

	public static void Insert<T1, T2>(this IList<ValueTuple<T1, T2>> @this, int index, T1 item1, T2 item2)
		=> @this.Insert(index, (item1, item2));

	public static void Insert<T1, T2, T3>(this IList<ValueTuple<T1, T2, T3>> @this, int index, T1 item1, T2 item2, T3 item3)
		=> @this.Insert(index, (item1, item2, item3));

	public static void Insert<T1, T2, T3, T4>(this IList<ValueTuple<T1, T2, T3, T4>> @this, int index, T1 item1, T2 item2, T3 item3, T4 item4)
		=> @this.Insert(index, (item1, item2, item3, item4));

	public static void Insert<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> @this, int index, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		=> @this.Insert(index, (item1, item2, item3, item4, item5));

	public static void Insert<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> @this, int index, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		=> @this.Insert(index, (item1, item2, item3, item4, item5, item6));

	public static void Insert<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> @this, int index, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		=> @this.Insert(index, (item1, item2, item3, item4, item5, item6, item7));

	public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> @this, TKey key, TValue value)
		=> @this.Add(new KeyValuePair<TKey, TValue>(key, value));

	public static void Add<T1, T2>(this ICollection<ValueTuple<T1, T2>> @this, T1 item1, T2 item2)
		=> @this.Add((item1, item2));

	public static void Add<T1, T2, T3>(this ICollection<ValueTuple<T1, T2, T3>> @this, T1 item1, T2 item2, T3 item3)
		=> @this.Add((item1, item2, item3));

	public static void Add<T1, T2, T3, T4>(this ICollection<ValueTuple<T1, T2, T3, T4>> @this, T1 item1, T2 item2, T3 item3, T4 item4)
		=> @this.Add((item1, item2, item3, item4));

	public static void Add<T1, T2, T3, T4, T5>(this ICollection<ValueTuple<T1, T2, T3, T4, T5>> @this, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		=> @this.Add((item1, item2, item3, item4, item5));

	public static void Add<T1, T2, T3, T4, T5, T6>(this ICollection<ValueTuple<T1, T2, T3, T4, T5, T6>> @this, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		=> @this.Add((item1, item2, item3, item4, item5, item6));

	public static void Add<T1, T2, T3, T4, T5, T6, T7>(this ICollection<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> @this, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		=> @this.Add((item1, item2, item3, item4, item5, item6, item7));
}
