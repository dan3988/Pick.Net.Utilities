using Pick.Net.Utilities.Reflection;
using Pick.Net.Utilities.Reflection.Members.Properties;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class ReflectedPropertyTests
{
	private abstract class BaseClass
	{
		public bool IsEmpty => Length == 0;

		public int Length => FirstName.Length + LastName.Length;

		public string FullName => FirstName + " " + LastName;

		public abstract string FirstName { get; set; }

		public abstract string LastName { get; set; }

		public string WriteOnly { set => Console.WriteLine("WriteOnly: {0}", value); }
	}

	private sealed class TestClass(string firstName, string lastName) : BaseClass 
	{
		public override string FirstName { get; set; } = firstName;

		public override string LastName { get; set; } = lastName;
	}

	private static void CheckEqual(ReflectedInstancePropertyCollection props, params IReflectedInstanceProperty[] expected)
		=> CollectionAssert.AreEqual(expected, props);

	[TestMethod]
	public void TestCollection()
	{
		var collection = ReflectionHelper.GetMembers<TestClass>().Properties;
		var baseCollection = ReflectionHelper.GetMembers(typeof(BaseClass)).Properties;

		var firstName = collection.GetProperty(nameof(TestClass.FirstName));
		var lastName = collection.GetProperty(nameof(TestClass.LastName));

		Assert.AreEqual(collection.BaseProperties, baseCollection);
		AssertHelper.CollectionsEqual(collection, firstName, lastName);
	}

	[TestMethod]
	public void TestAccessibility()
	{
		var collection = ReflectionHelper.GetMembers<TestClass>().Properties;
		var prop = collection.GetProperty("FirstName");

		Assert.AreEqual(prop, collection.GetReadableProperty<string>("FirstName"));
		Assert.AreEqual(prop, collection.GetWritableProperty<string>("FirstName"));
		Assert.AreEqual(prop, collection.GetFullProperty<string>("FirstName"));
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetWritableProperty<int>("IsEmpty"), "Property 'Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass.IsEmpty' is read-only.");
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetFullProperty<int>("IsEmpty"), "Property 'Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass.IsEmpty' is read-only.");
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetReadableProperty<int>("WriteOnly"), "Property 'Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass.WriteOnly' is write-only.");
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetFullProperty<int>("WriteOnly"), "Property 'Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass.WriteOnly' is write-only.");
	}

	[TestMethod]
	public void TestCovariance()
	{
		const string name = nameof(TestClass.FullName);

		var collection = ReflectionHelper.GetMembers<TestClass>().Properties;
		var prop = collection.GetReadableProperty<string>(name);
		var instance = new TestClass("John", "Doe");

		Assert.AreEqual("John Doe", prop.GetValue(instance));
		Assert.AreEqual(prop, collection.GetReadableProperty<IComparable>(name));
		Assert.AreEqual(prop, collection.GetReadableProperty<object>(name));
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetReadableProperty<Enum>(name), "Property 'FullName' IReflectedReadableInstanceProperty<Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass, System.String> cannot be cast to IReflectedReadableInstanceProperty<Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass, System.Enum>.");
		AssertHelper.ThrowsExceptionWithMessage<InvalidOperationException>(() => collection.GetReadableProperty<object>(nameof(BaseClass.Length)), "Property 'Length' IReflectedReadableInstanceProperty<Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass, System.Int32> cannot be cast to IReflectedReadableInstanceProperty<Pick.Net.Utilities.Tests.ReflectedPropertyTests+BaseClass, System.Object>.");
	}
}