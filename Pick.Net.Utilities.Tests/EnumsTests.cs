using System.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class EnumsTests
{
	[TestMethod]
	public void TestValues()
	{
		var values = Enum.GetValues<ConsoleColor>();
		var list = Enums.GetValues<ConsoleColor>();

		CollectionAssert.AreEqual(values, list);
	}

	[TestMethod]
	public void TestNames()
	{
		var values = Enum.GetNames<ConsoleColor>();
		var list = Enums.GetNames<ConsoleColor>();

		CollectionAssert.AreEqual(values, list);
	}

	[TestMethod]
	public void TestHasFlags()
	{
		var test2 = BindingFlags.Public.HasFlag(BindingFlags.Default);
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public, BindingFlags.Public));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public, (BindingFlags)0));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public, BindingFlags.NonPublic));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.Instance));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, BindingFlags.Public | BindingFlags.Instance));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.NonPublic));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
	}
}
