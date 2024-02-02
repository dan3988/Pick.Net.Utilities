using Pick.Net.Utilities.Threading;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class AtomicValueTests
{
	private static void TestEquality<TSelf, TValue>(TValue value, TSelf atomic)
		where TValue : IEquatable<TValue>
		where TSelf : IAtomicValue<TSelf, TValue>
	{
		var areEqual = value.Equals(atomic.Value);
		Assert.AreEqual(areEqual, atomic == TSelf.Create(value), "Expected equality operator ({0}){2} == ({0}){3} to return {1}.", typeof(TSelf), areEqual, atomic.Value, value);
		Assert.AreEqual(!areEqual, atomic != TSelf.Create(value), "Expected equality operator ({0}){2} != ({0}){3} to return {1}.", typeof(TSelf), !areEqual, atomic.Value, value);
		Assert.AreEqual(areEqual, atomic == value, "Expected equality operator ({0}){2} == {3} to return {1}.", typeof(TSelf), areEqual, atomic.Value, value);
		Assert.AreEqual(!areEqual, atomic != value, "Expected equality operator ({0}){2} != {3} to return {1}.", typeof(TSelf), !areEqual, atomic.Value, value);
		Assert.AreEqual(areEqual, value == atomic, "Expected equality operator {3} == ({0}){2} to return {1}.", typeof(TSelf), areEqual, atomic.Value, value);
		Assert.AreEqual(!areEqual, value != atomic, "Expected equality operator {3} != ({0}){2} to return {1}.", typeof(TSelf), !areEqual, atomic.Value, value);
	}

	[TestMethod]
	public void TestAtomicBool()
	{
		var value = new AtomicBool();

		Assert.IsFalse(value.Value);
		Assert.IsFalse(value.TrySet(false));
		Assert.IsTrue(value.TrySet(true));
		Assert.IsTrue(value.Value);
		TestEquality(true, value);
		TestEquality(false, value);


		value = new AtomicBool(true);
		Assert.IsTrue(value.Value);
		Assert.IsFalse(value.TrySet(true));
		Assert.IsTrue(value.TrySet(false));
		Assert.IsFalse(value.Value);
		TestEquality(true, value);
		TestEquality(false, value);
	}

	[TestMethod]
	public void TestAtomicInt32()
	{
		var value = new AtomicInt32();

		Assert.AreEqual(0, value.Value);
		Assert.AreEqual(1, value.Increment());
		Assert.AreEqual(1, value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(499, value.Add(500));
		Assert.AreEqual(200, value.Value = 200);
		TestEquality(200, value);
		TestEquality(5, value);

		Assert.AreEqual(200, value.Set(1000));
		Assert.AreEqual(1000, value.Value);
		Assert.AreEqual(1000, value.Set(1000));
		Assert.AreEqual(1000, value.Set(1005, 1000));
		Assert.AreEqual(1005, value.Value);
		Assert.AreEqual(1005, value.Set(5000, 1000));
		Assert.AreEqual(1005, value.Value);
	}

	[TestMethod]
	public void TestAtomicUInt32()
	{
		var value = new AtomicUInt32();

		Assert.AreEqual(0u, value.Value);
		Assert.AreEqual(1u, value.Increment());
		Assert.AreEqual(1u, value.Set(0));
		Assert.AreEqual(uint.MaxValue, value.Decrement());
		Assert.AreEqual(499u, value.Add(500));
		Assert.AreEqual(200u, value.Value = 200);
		TestEquality(200u, value);
		TestEquality(5u, value);

		Assert.AreEqual(200u, value.Set(1000));
		Assert.AreEqual(1000u, value.Value);
		Assert.AreEqual(1000u, value.Set(1000));
		Assert.AreEqual(1000u, value.Set(1005, 1000));
		Assert.AreEqual(1005u, value.Value);
		Assert.AreEqual(1005u, value.Set(5000, 1000));
		Assert.AreEqual(1005u, value.Value);
	}

	[TestMethod]
	public void TestAtomicInt64()
	{
		var value = new AtomicInt64();

		Assert.AreEqual(0, value.Value);
		Assert.AreEqual(1, value.Increment());
		Assert.AreEqual(1, value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(499, value.Add(500));
		Assert.AreEqual(200, value.Value = 200);
		TestEquality(200L, value);
		TestEquality(5L, value);

		Assert.AreEqual(200, value.Set(1000));
		Assert.AreEqual(1000, value.Value);
		Assert.AreEqual(1000, value.Set(1000));
		Assert.AreEqual(1000, value.Set(1005, 1000));
		Assert.AreEqual(1005, value.Value);
		Assert.AreEqual(1005, value.Set(5000, 1000));
		Assert.AreEqual(1005, value.Value);
	}

	[TestMethod]
	public void TestAtomicUInt64()
	{
		var value = new AtomicUInt64();

		Assert.AreEqual(0u, value.Value);
		Assert.AreEqual(1u, value.Increment());
		Assert.AreEqual(1u, value.Set(0));
		Assert.AreEqual(ulong.MaxValue, value.Decrement());
		Assert.AreEqual(499u, value.Add(500));
		Assert.AreEqual(200u, value.Value = 200);
		TestEquality(200UL, value);
		TestEquality(5UL, value);

		Assert.AreEqual(200u, value.Set(1000));
		Assert.AreEqual(1000u, value.Value);
		Assert.AreEqual(1000u, value.Set(1000));
		Assert.AreEqual(1000u, value.Set(1005, 1000));
		Assert.AreEqual(1005u, value.Value);
		Assert.AreEqual(1005u, value.Set(5000, 1000));
		Assert.AreEqual(1005u, value.Value);
	}

	[TestMethod]
	public void TestAtomicSingle()
	{
		var value = new AtomicSingle();

		Assert.AreEqual(0, value.Value);
		Assert.AreEqual(1, value.Increment());
		Assert.AreEqual(1, value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(249.5f, value.Add(250.5f));
		Assert.AreEqual(33.3333f, value.Value = 33.3333f);
		TestEquality(33.3333f, value);
		TestEquality(5f, value);

		Assert.AreEqual(33.3333f, value.Set(1000));
		Assert.AreEqual(1000, value.Value);
		Assert.AreEqual(1000, value.Set(1000));
		Assert.AreEqual(1000, value.Set(1005, 1000));
		Assert.AreEqual(1005, value.Value);
		Assert.AreEqual(1005, value.Set(5000, 1000));
		Assert.AreEqual(1005, value.Value);
	}

	[TestMethod]
	public void TestAtomicDouble()
	{
		var value = new AtomicDouble();

		Assert.AreEqual(0, value.Value);
		Assert.AreEqual(1, value.Increment());
		Assert.AreEqual(1, value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(249.5, value.Add(250.5));
		Assert.AreEqual(33.3333, value.Value = 33.3333);
		TestEquality(33.3333, value);
		TestEquality(5D, value);

		Assert.AreEqual(33.3333, value.Set(1000));
		Assert.AreEqual(1000, value.Value);
		Assert.AreEqual(1000, value.Set(1000));
		Assert.AreEqual(1000, value.Set(1005, 1000));
		Assert.AreEqual(1005, value.Value);
		Assert.AreEqual(1005, value.Set(5000, 1000));
		Assert.AreEqual(1005, value.Value);
	}
}