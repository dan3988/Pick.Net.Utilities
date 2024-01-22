﻿using System.Reflection;

namespace Pick.Net.Utilities.Tests;

[TestClass]
public class EnumsTests
{
	public enum NegativeEnum
	{
		Negative1 = -1,
		Negative2 = -2,
		Zero = 0,
		Positive1 = 1,
		Positive2 = 2,
	}

	public enum UIntEnum : uint
	{
		Max = uint.MaxValue,
		Zero = 0,
		Positive1 = 1,
		Positive2 = 2,
	}

	public enum LongEnum : long
	{
		Min = ~((long)uint.MaxValue),
		IntMinValue = int.MinValue,
		Negative2 = -2,
		Negative1 = -1,
		Zero = 0,
		Positive1 = 1,
		Positive2 = 2,
		IntMaxValue = int.MaxValue,
		Max = uint.MaxValue,
	}

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
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public, BindingFlags.Public));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public, (BindingFlags)0));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public, BindingFlags.NonPublic));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.Instance));
		Assert.IsTrue(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, BindingFlags.Public | BindingFlags.Instance));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.NonPublic));
		Assert.IsFalse(Enums.HasFlagsFast(BindingFlags.Public | BindingFlags.Instance, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
	}

	[TestMethod]
	public void TestMinMaxValue()
	{
		static void Test<T>(T expectedMin, T expectedMax) where T : unmanaged, Enum
		{
			Assert.AreEqual(expectedMin, Enums.GetMinValue<T>());
			Assert.AreEqual(expectedMax, Enums.GetMaxValue<T>());
		}

		Test(ConsoleColor.Black, ConsoleColor.White);
		Test(BindingFlags.Default, BindingFlags.DoNotWrapExceptions);
		Test(TypeCode.Empty, TypeCode.String);
		Test(NegativeEnum.Negative2, NegativeEnum.Positive2);
		Test(UIntEnum.Zero, UIntEnum.Max);
		Test(LongEnum.Min, LongEnum.Max);
	}

	[TestMethod]
	public void TestArithmetic()
	{
		Assert.AreEqual(ConsoleColor.Blue, ConsoleColor.DarkMagenta.Add(ConsoleColor.DarkRed));
		Assert.AreEqual(LongEnum.Negative1, LongEnum.IntMaxValue.Add(LongEnum.IntMinValue));
		Assert.AreEqual(LongEnum.IntMaxValue + 1, LongEnum.Max.Subtract(LongEnum.IntMaxValue));
		Assert.AreEqual(LongEnum.Max - 1, LongEnum.IntMaxValue.Multiply(LongEnum.Positive2));
		Assert.AreEqual(LongEnum.Negative1, LongEnum.IntMinValue.Divide(LongEnum.IntMaxValue));
		Assert.AreEqual(LongEnum.IntMinValue - 1, LongEnum.Min.BitwiseOr(LongEnum.IntMaxValue));
	}
}
