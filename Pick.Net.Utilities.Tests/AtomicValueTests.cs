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
		Assert.IsFalse(value.Set(false));
		Assert.IsTrue(value.Set(true));
		Assert.IsTrue(value.Value);
		TestEquality(true, value);
		TestEquality(false, value);


		value = new AtomicBool(true);
		Assert.IsTrue(value.Value);
		Assert.IsFalse(value.Set(true));
		Assert.IsTrue(value.Set(false));
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
		Assert.IsFalse(value.Set(1));
		Assert.IsTrue(value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(499, value.Add(500));
		Assert.AreEqual(200, value.Value = 200);
		TestEquality(200, value);
		TestEquality(5, value);
	}

	[TestMethod]
	public void TestAtomicUInt32()
	{
		var value = new AtomicUInt32();

		Assert.AreEqual(0u, value.Value);
		Assert.AreEqual(1u, value.Increment());
		Assert.IsFalse(value.Set(1));
		Assert.IsTrue(value.Set(0));
		Assert.AreEqual(uint.MaxValue, value.Decrement());
		Assert.AreEqual(499u, value.Add(500));
		Assert.AreEqual(200u, value.Value = 200);
		TestEquality(200u, value);
		TestEquality(5u, value);
	}

	[TestMethod]
	public void TestAtomicInt64()
	{
		var value = new AtomicInt64();

		Assert.AreEqual(0, value.Value);
		Assert.AreEqual(1, value.Increment());
		Assert.IsFalse(value.Set(1));
		Assert.IsTrue(value.Set(0));
		Assert.AreEqual(-1, value.Decrement());
		Assert.AreEqual(499, value.Add(500));
		Assert.AreEqual(200, value.Value = 200);
		TestEquality(200L, value);
		TestEquality(5L, value);
	}

	[TestMethod]
	public void TestAtomicUInt64()
	{
		var value = new AtomicUInt64();

		Assert.AreEqual(0u, value.Value);
		Assert.AreEqual(1u, value.Increment());
		Assert.IsFalse(value.Set(1));
		Assert.IsTrue(value.Set(0));
		Assert.AreEqual(ulong.MaxValue, value.Decrement());
		Assert.AreEqual(499u, value.Add(500));
		Assert.AreEqual(200u, value.Value = 200);
		TestEquality(200UL, value);
		TestEquality(5UL, value);
	}
}