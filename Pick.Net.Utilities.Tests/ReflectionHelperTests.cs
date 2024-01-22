using Pick.Net.Utilities.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class ReflectionHelperTests
{
	private sealed class UnknownType : List<object>
	{

	}

	[TestMethod]
	public void TestGetCollectionType()
	{
		Assert.AreEqual(typeof(int), typeof(int[]).TryGetCollectionType());
		Assert.AreEqual(typeof(char), typeof(string).TryGetCollectionType());
		Assert.AreEqual(typeof(string), typeof(IList<string>).TryGetCollectionType());
		Assert.AreEqual(typeof(KeyValuePair<int, bool>), typeof(Dictionary<int, bool>).TryGetCollectionType());
		Assert.AreEqual(typeof(string), typeof(List<string>).TryGetCollectionType());
		Assert.AreEqual(typeof(object), typeof(UnknownType).TryGetCollectionType());
		Assert.ThrowsException<ArgumentException>(() => typeof(int).GetCollectionType());
	}
}