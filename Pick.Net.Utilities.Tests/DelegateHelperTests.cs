using Pick.Net.Utilities.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class DelegateHelperTests
{
	static void TestGetArguments<T>(params Type[] expectedTypes) where T : Delegate
		=> CollectionAssert.AreEqual(expectedTypes, DelegateHelper.GetArgumentTypes<T>());

	[TestMethod]
	public void TestGetArguments()
	{
		TestGetArguments<Action>();
		TestGetArguments<Func<bool>>();
		TestGetArguments<Func<bool, int, string>>(typeof(bool), typeof(int));
		TestGetArguments<Action<string, string, string>>(typeof(string), typeof(string), typeof(string));
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetArgumentTypes<MulticastDelegate>());
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetArgumentTypes(typeof(string)));
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetArgumentTypes(typeof(Func<,,>)));
	}

	[TestMethod]
	public void TestGetRetrnType()
	{
		Assert.AreEqual(typeof(void), DelegateHelper.GetReturnType<Action>());
		Assert.AreEqual(typeof(bool), DelegateHelper.GetReturnType<Func<bool>>());
		Assert.AreEqual(typeof(string), DelegateHelper.GetReturnType<Func<bool, int, string>>());
		Assert.AreEqual(typeof(void), DelegateHelper.GetReturnType<Action<string, string, string>>());
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetReturnType<MulticastDelegate>());
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetReturnType(typeof(string)));
		Assert.ThrowsException<ArgumentException>(() => DelegateHelper.GetReturnType(typeof(Func<,,>)));
	}
}
