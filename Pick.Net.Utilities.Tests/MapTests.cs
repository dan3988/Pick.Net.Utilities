using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class MapTests
{
	private static void TestEntry(Map<string, string> map, string key, string value)
	{
		Assert.IsTrue(map.ContainsKey(key));
		Assert.AreEqual(value, map[key]);
		var entry = map.GetEntry(key);
		Assert.IsNotNull(entry);
		Assert.AreEqual(value, entry.Value);
		CollectionAssert.Contains(map.Keys, key);
		CollectionAssert.Contains(map.Values, value);
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
		CollectionAssert.AreEqual(new[] { "key1", "key2", "key3", "key4", "key5" }, map.Keys);
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
		CollectionAssert.AreEqual(new[] { "key1", "key2", "key4", "key5" }, map.Keys);
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
		CollectionAssert.AreEqual(new[] { "key1", "key2", "key3", "key4", "key5" }, map.Keys);
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
		CollectionAssert.AreEqual(new[] { "key1", "key2", "key3", "key4", "key5" }, map.Keys);
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
}
