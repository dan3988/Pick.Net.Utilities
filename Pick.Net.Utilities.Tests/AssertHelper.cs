using System.Collections;

namespace Pick.Net.Utilities.Tests;

public static class AssertHelper
{
	public static void ThrowsExceptionWithMessage<T>(Action action, string expectedMessage, string message = "") where T : Exception
	{
		var ex = Assert.ThrowsException<T>(action, message);
		Assert.AreEqual(expectedMessage, ex.Message);
	}

	public static void CollectionsEqual<T>(IEnumerable<T> source, params T[] expected)
		=> CollectionAssert.AreEqual(source as ICollection ?? source.ToArray(), expected);
}