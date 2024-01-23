using System.Reflection;

using Pick.Net.Utilities.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class TypeHierarchyTests
{
	[TestMethod]
	public void TestInt()
	{
		var hierarchy = typeof(int).GetBaseTypes(true).ToArray();
		CollectionAssert.AreEquivalent(hierarchy, new Type[] { typeof(int), typeof(ValueType), typeof(object) });
	}

	[TestMethod]
	public void TestMethodInfo()
	{
		var hierarchy = typeof(MethodInfo).GetBaseTypes().ToArray();
		CollectionAssert.AreEquivalent(hierarchy, new Type[] { typeof(MethodBase), typeof(MemberInfo), typeof(object) });
	}

	[TestMethod]
	public void TestMethodInfoLimited()
	{
		var hierarchy = typeof(MethodInfo).GetBaseTypes(typeof(MemberInfo)).ToArray();
		CollectionAssert.AreEquivalent(hierarchy, new Type[] { typeof(MethodBase) });
	}
}
