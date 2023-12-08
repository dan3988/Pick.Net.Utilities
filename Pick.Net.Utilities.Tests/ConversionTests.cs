namespace Pick.Net.Utilities.Tests;

[TestClass]
public class ConversionTests
{
	[TestMethod]
	public void EnumConversion()
	{
		Assert.AreEqual(ConsoleColor.Red, Conversion.Convert<ConsoleColor>("Red"));
		Assert.AreEqual(ConsoleColor.Red, Conversion.Convert<ConsoleColor>("12"));
		Assert.AreEqual(ConsoleColor.Gray, Conversion.Convert<ConsoleColor>(7));
		Assert.AreEqual(ConsoleColor.Blue, Conversion.Convert<ConsoleColor?>(9));
		Assert.ThrowsException<ArgumentNullException>(() => Conversion.Convert(null, typeof(ConsoleColor)));
		Assert.ThrowsException<ArgumentException>(() => Conversion.Convert("Purple", typeof(ConsoleColor)));
		Assert.AreEqual(null, Conversion.Convert<ConsoleColor?>(null));
	}

	[TestMethod]
	public void StringConversion()
	{
		Assert.AreEqual("True", Conversion.Convert<string>(true));
		Assert.AreEqual("False", Conversion.Convert<string>(false));
		Assert.AreEqual("5", Conversion.Convert<string>(5));
		Assert.AreEqual("3.141592653589793", Conversion.Convert<string>(Math.PI));
		Assert.AreEqual(true, Conversion.Convert<bool>("True"));
		Assert.AreEqual(false, Conversion.Convert<bool>("False"));
		Assert.AreEqual(5, Conversion.Convert<int>("5"));
		Assert.AreEqual(Math.PI, Conversion.Convert<double>("3.141592653589793"));
	}

	[TestMethod]
	public void NumberConversion()
	{
		Assert.AreEqual(5, Conversion.Convert<byte>(5u));
		Assert.AreEqual(3u, Conversion.Convert<uint>(Math.PI));
		Assert.ThrowsException<OverflowException>(() => Conversion.Convert<uint>(ulong.MaxValue));
	}
}
