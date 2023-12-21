using System.Reflection;

using Pick.Net.Utilities.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class TypeHeirachyTests
{
	[TestMethod]
	public void TestInt()
	{
		var heirachy = typeof(int).GetBaseTypes(true).ToArray();
		CollectionAssert.AreEquivalent(heirachy, new Type[] { typeof(int), typeof(ValueType), typeof(object) });
	}

	[TestMethod]
	public void TestMethodInfo()
	{
		var heirachy = typeof(MethodInfo).GetBaseTypes().ToArray();
		CollectionAssert.AreEquivalent(heirachy, new Type[] { typeof(MethodBase), typeof(MemberInfo), typeof(object) });
	}

	[TestMethod]
	public void TestMethodInfoLimited()
	{
		var heirachy = typeof(MethodInfo).GetBaseTypes(typeof(MemberInfo)).ToArray();
		CollectionAssert.AreEquivalent(heirachy, new Type[] { typeof(MethodBase) });
	}
}
