﻿namespace Pick.Net.Utilities.Collections;

public static class CollectionExtensions
{
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
