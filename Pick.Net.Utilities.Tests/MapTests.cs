using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class MapTests
{
	private static void TestEntry<TKey, TValue>(Map<TKey, TValue> values, TKey key, TValue value)
		where TKey : notnull
		where TValue : class
	{
		Assert.IsTrue(values.ContainsKey(key), "Expected ContainsKey to return true for key '{0}'.", key);
		Assert.IsTrue(values.Keys.Contains(key), "Expected dictionary key collection to contain '{0}'.", key);
		Assert.AreEqual(value, (object?)values[key], "Expected indexer value for key '{0}' to be '{1}'.", key, value);

		var entry = values.GetEntry(key);
		Assert.IsNotNull(entry);
		Assert.AreEqual(value, entry.Value);
		CollectionAssert.Contains(values.Keys, key, "Expected key collection to contain '{0}'.", key);
		CollectionAssert.Contains(values.Values, value, "Expected value collection to contain '{0}'.", value);
	}

	private static void AssertEqualIgnoreOrder<T>(IReadOnlyCollection<T> expected, IReadOnlyCollection<T> actual, IEqualityComparer<T>? comparer = null)
	{
		Assert.AreEqual(actual.Count, expected.Count, "Expected both collections to be the same size.");

		comparer ??= EqualityComparer<T>.Default;
		var remaining = new List<T>(expected);

		foreach (var item in expected)
			Assert.IsTrue(Check(item), "Expected collection to contain '{0}'", item);

		bool Check(T item)
		{
			for (var i = 0; i < remaining.Count; i++)
			{
				if (comparer.Equals(remaining[i], item))
				{
					remaining.RemoveAt(i);
					return true;
				}
			}

			return false;
		}
	}

	private static void CheckDictionaryValues<TKey, TValue>(Map<TKey, TValue> values, params (TKey Key, TValue Value)[] expected)
		where TKey : notnull
		where TValue : class
	{
		AssertEqualIgnoreOrder(values, expected.Select(v => new MapEntry<TKey, TValue>(v.Key, v.Value)).ToArray(), MapEntryComparer<TKey, TValue>.Default);
		AssertEqualIgnoreOrder(values.Keys, expected.Select(v => v.Key).ToArray());
		AssertEqualIgnoreOrder(values.Values, expected.Select(v => v.Value).ToArray());
	}

	[TestMethod]
	public void AddingValues()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		Assert.AreEqual(5, map.Count);
		TestEntry(map, "key1", "value1");
		TestEntry(map, "key2", "value2");
		TestEntry(map, "key3", "value3");
		TestEntry(map, "key4", "value4");
		TestEntry(map, "key5", "value5");
		Assert.IsNull(map["key6"]);
		CheckDictionaryValues(map, ("key1", "value1"), ("key2", "value2"), ("key3", "value3"), ("key4", "value4"), ("key5", "value5"));
	}

	[TestMethod]
	public void RemovingValues()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		map.Remove("key3");

		Assert.AreEqual(4, map.Count);
		Assert.IsNull(map["key3"]);
		Assert.IsNull(map.GetEntry("key3"));
		TestEntry(map, "key1", "value1");
		TestEntry(map, "key2", "value2");
		TestEntry(map, "key4", "value4");
		TestEntry(map, "key5", "value5");
		CheckDictionaryValues(map, ("key1", "value1"), ("key2", "value2"), ("key4", "value4"), ("key5", "value5"));
	}

	[TestMethod]
	public void UpdatingValues()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		map["key3"] = "new value";

		Assert.AreEqual(5, map.Count);
		TestEntry(map, "key1", "value1");
		TestEntry(map, "key2", "value2");
		TestEntry(map, "key3", "new value");
		TestEntry(map, "key4", "value4");
		TestEntry(map, "key5", "value5");
		CheckDictionaryValues(map, ("key1", "value1"), ("key2", "value2"), ("key3", "new value"), ("key4", "value4"), ("key5", "value5"));
	}

	[TestMethod]
	public void UpdatingEntries()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		var entry = map.GetEntry("key2");
		Assert.IsNotNull(entry);
		entry.Value = "new value";

		Assert.AreEqual(5, map.Count);
		TestEntry(map, "key1", "value1");
		TestEntry(map, "key2", "new value");
		TestEntry(map, "key3", "value3");
		TestEntry(map, "key4", "value4");
		TestEntry(map, "key5", "value5");
		CheckDictionaryValues(map, ("key1", "value1"), ("key2", "new value"), ("key3", "value3"), ("key4", "value4"), ("key5", "value5"));
	}

	[TestMethod]
	public void Clearing()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		map.Clear();
		Assert.AreEqual(0, map.Count);
		CollectionAssert.AreEqual(Array.Empty<string>(), map.Keys);
		CollectionAssert.AreEqual(Array.Empty<string>(), map.Values);
	}

	[TestMethod]
	public void ThrowsOnDuplicateKey()
	{
		var map = new Map<string, string>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		Assert.ThrowsException<ArgumentException>(() => map.Add("key3", ""));
	}

	private sealed class MapEntryComparer<TKey, TValue>(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) : IEqualityComparer<IReadOnlyMapEntry<TKey, TValue>>
		where TKey : notnull
		where TValue : class
	{
		public static readonly MapEntryComparer<TKey, TValue> Default = new(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default);

		public bool Equals(IReadOnlyMapEntry<TKey, TValue>? x, IReadOnlyMapEntry<TKey, TValue>? y)
		{
			if (x == null)
				return y == null;

			if (y == null)
				return false;

			return keyComparer.Equals(x.Key, y.Key) && valueComparer.Equals(x.Value, y.Value);
		}

		public int GetHashCode(IReadOnlyMapEntry<TKey, TValue> obj)
			=> keyComparer.GetHashCode(obj.Key) ^ valueComparer.GetHashCode(obj.Value);
	}
}
