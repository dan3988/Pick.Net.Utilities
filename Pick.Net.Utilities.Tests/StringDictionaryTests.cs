using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class StringDictionaryTests
{
	private static void CheckDictionaryValues<T>(StringDictionary<T> values, params (string Key, T Value)[] expected)
	{
		CollectionAssert.AreEqual(expected.Select(v => new KeyValuePair<string, T>(v.Key, v.Value)).ToArray(), values);
		CollectionAssert.AreEqual(expected.Select(v => v.Key).ToArray(), values.Keys);
		CollectionAssert.AreEqual(expected.Select(v => v.Value).ToArray(), values.Values);
	}

	private static void CheckValue<T>(StringDictionary<T> values, string key, T value)
	{
		Assert.IsTrue(values.ContainsKey(key), "Expected ContainsKey to return true for key '{0}'.", key);
		Assert.IsTrue(values.Keys.Contains(key), "Expected dictionary key collection to contain '{0}'.", key);
		Assert.IsTrue(values.TryGetValue(key, out var retrievedValue), "Expected TryGetValue to return true for key '{0}'.", key);
		Assert.AreEqual(value, (object?)retrievedValue, "Expected TryGetValue value for key '{0}' to be '{1}'.", key, value);
		Assert.AreEqual(value, (object?)values[key], "Expected indexer value for key '{0}' to be '{1}'.", key, value);

		var keySpan = key.AsSpan();
		Assert.IsTrue(values.ContainsKey(keySpan), "Expected ContainsKey to return true for key '{0}'.", key);
		Assert.IsTrue(values.Keys.Contains(keySpan), "Expected dictionary key collection to contain '{0}'.", key);
		Assert.IsTrue(values.TryGetValue(keySpan, out retrievedValue), "Expected TryGetValue to return true for key '{0}'.", key);
		Assert.AreEqual(value, (object?)retrievedValue, "Expected TryGetValue value for key '{0}' to be '{1}'.", key, value);
		Assert.AreEqual(value, (object?)values[keySpan], "Expected indexer value for key '{0}' to be '{1}'.", key, value);

		CollectionAssert.Contains(values.Values, value);
	}

	[TestMethod]
	public void Add()
	{
		var dictionary = new StringDictionary<object>();

		CheckDictionaryValues(dictionary);

		dictionary.Add("key1", "value1");
		dictionary.Add("key2", "value2");
		dictionary.Add("key3", "value3");

		CheckDictionaryValues(dictionary, ("key1", "value1"), ("key2", "value2"), ("key3", "value3"));
		Assert.ThrowsException<ArgumentException>(() => dictionary.Add("key1", "different value"));
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key0"]);
		CheckValue(dictionary, "key1", "value1");
		CheckValue(dictionary, "key2", "value2");
		CheckValue(dictionary, "key3", "value3");
	}

	[TestMethod]
	public void Copying()
	{
		var src = new Dictionary<string, object>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		var dictionary = new StringDictionary<object>(src);

		CheckDictionaryValues(dictionary, ("key1", "value1"), ("key2", "value2"), ("key3", "value3"), ("key4", "value4"), ("key5", "value5"));
		Assert.ThrowsException<ArgumentException>(() => dictionary.Add("key1", "different value"));
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key0"]);
		CheckValue(dictionary, "key1", "value1");
		CheckValue(dictionary, "key2", "value2");
		CheckValue(dictionary, "key3", "value3");
		CheckValue(dictionary, "key4", "value4");
		CheckValue(dictionary, "key5", "value5");
	}

	[TestMethod]
	public void Remove()
	{
		var dictionary = new StringDictionary<object>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" },
			{ "key4", "value4" },
			{ "key5", "value5" }
		};

		Assert.IsTrue(dictionary.Remove("key2".AsSpan(), out var value));
		Assert.AreEqual("value2", value);
		Assert.IsTrue(dictionary.Remove("key4".AsSpan(), out value));
		Assert.AreEqual("value4", value);
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key2"]);

		CheckDictionaryValues(dictionary, ("key1", "value1"), ("key3", "value3"), ("key5", "value5"));

		dictionary.Add("key7", "value7");
		dictionary.Add("key6", "value6");
		CheckDictionaryValues(dictionary, ("key1", "value1"), ("key6", "value6"), ("key3", "value3"), ("key7", "value7"), ("key5", "value5"));
	}

	[TestMethod]
	public void Clear()
	{
		var dictionary = new StringDictionary<object>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" }
		};

		dictionary.Clear();

		CheckDictionaryValues(dictionary);
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key1"]);
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key2"]);
		Assert.ThrowsException<KeyNotFoundException>(() => dictionary["key3"]);

		dictionary.Add("key1", "value1");
		dictionary.Add("key2", "value2");
		dictionary.Add("key3", "value3");

		CheckDictionaryValues(dictionary, ("key1", "value1"), ("key2", "value2"), ("key3", "value3"));
	}

	[TestMethod]
	public void Replace()
	{
		var dictionary = new StringDictionary<object>
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" }
		};

		dictionary["key2"] = "new value";

		CheckValue(dictionary, "key2", "new value");
	}

	[TestMethod]
	public void CaseInsensitive()
	{
		var dictionary = new StringDictionary<object>(StringComparison.OrdinalIgnoreCase)
		{
			{ "key1", "value1" },
			{ "key2", "value2" },
			{ "key3", "value3" }
		};

		CheckValue(dictionary, "key1", "value1");
		CheckValue(dictionary, "KEY1", "value1");
		CheckValue(dictionary, "Key1", "value1");

		dictionary["KEY2"] = "new value";

		CheckValue(dictionary, "KEY2", "new value");
		CheckValue(dictionary, "key2", "new value");
	}

	[TestMethod]
	public void EnumeratorIsInvalidatedByMutation()
	{
		void Test(string message, Action<StringDictionary<object>> mutate)
		{
			var dictionary = new StringDictionary<object>(StringComparison.OrdinalIgnoreCase)
			{
				{ "key1", "value1" },
				{ "key2", "value2" },
				{ "key3", "value3" }
			};

			var enumeratePairs = dictionary.GetEnumerator();
			var enumerateKeys = dictionary.Keys.GetEnumerator();
			var enumerateValues = dictionary.Values.GetEnumerator();

			mutate.Invoke(dictionary);

			Assert.ThrowsException<InvalidOperationException>(() => enumeratePairs.MoveNext(), "Dictionary enumerator was not invalidated when {0}", message);
			Assert.ThrowsException<InvalidOperationException>(() => enumerateKeys.MoveNext(), "Dictionary key collection enumerator was not invalidated when {0}", message);
			Assert.ThrowsException<InvalidOperationException>(() => enumerateValues.MoveNext(), "Dictionary value collection enumerator was not invalidated when {0}", message);
		}

		Test("replacing a value", d => d["key3"] = "new value");
		Test("adding a value", d => d.Add("key4", "new value"));
		Test("removing a value", d => d.Remove("key3"));
		Test("clearing the collection", d => d.Clear());
	}
}
