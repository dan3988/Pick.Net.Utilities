using System;
using System.Numerics;
using System.Reflection;

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
		var boxed = Enums.GetValueList<ConsoleColor>();

		CollectionAssert.AreEqual(values, list);		
		CollectionAssert.AreEqual(list, boxed);
		Assert.AreSame(boxed, Enums.GetValueList(typeof(ConsoleColor)));
	}

	[TestMethod]
	public void TestNames()
	{
		var values = Enum.GetNames<ConsoleColor>();
		var list = Enums.GetNames<ConsoleColor>();
		var boxed = Enums.GetNameList<ConsoleColor>();

		CollectionAssert.AreEqual(values, list);
		CollectionAssert.AreEqual(list, boxed);
		Assert.AreSame(boxed, Enums.GetNameList(typeof(ConsoleColor)));
	}

	[TestMethod]
	public void TestTypeCode()
	{
		static void DoTest<T>(TypeCode expected) where T : unmanaged, Enum
		{
			Assert.AreEqual(expected, Enums.GetTypeCode<T>(), $"Enums.GetTypeCode<T>() returned incorrect value for type {typeof(T)}.");
			Assert.AreEqual(expected, Enums.GetTypeCode(typeof(T)), $"Enums.GetTypeCode(Type) returned incorrect value for type {typeof(T)}.");
		}

		DoTest<NegativeEnum>(TypeCode.Int32);
		DoTest<UIntEnum>(TypeCode.UInt32);
		DoTest<LongEnum>(TypeCode.Int64);
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

	[TestMethod]
	public void TestConversion()
	{
		static void TestEqual<TIn, TOut>(TIn input, TOut expected, Func<TIn, TOut> fn)
			where TIn : unmanaged, Enum
			where TOut : unmanaged, INumber<TOut>
		{
			Assert.AreEqual(expected, fn(input));
		}

		static void TestOverflows<TIn, TOut>(TIn input, Func<TIn, TOut> fn)
			where TIn : unmanaged, Enum
			where TOut : unmanaged, INumber<TOut>
		{
			Assert.ThrowsException<OverflowException>(() => fn(input));
		}

		TestEqual(LongEnum.Negative1, -1, Enums.ToInt64);
		TestOverflows(LongEnum.Negative1, Enums.ToUInt64);
		TestEqual(LongEnum.Negative1, -1, Enums.ToInt32);
		TestOverflows(LongEnum.Negative1, Enums.ToUInt32);
		TestEqual(LongEnum.Negative1, (short)-1, Enums.ToInt16);
		TestOverflows(LongEnum.Negative1, Enums.ToUInt16);
		TestEqual(LongEnum.Negative1, (sbyte)-1, Enums.ToSByte);
		TestOverflows(LongEnum.Negative1, Enums.ToByte);

		TestEqual(LongEnum.IntMaxValue, int.MaxValue, Enums.ToInt64);
		TestEqual(LongEnum.IntMaxValue, (ulong)int.MaxValue, Enums.ToUInt64);
		TestEqual(LongEnum.IntMaxValue, int.MaxValue, Enums.ToInt32);
		TestEqual(LongEnum.IntMaxValue, (uint)int.MaxValue, Enums.ToUInt32);
		TestOverflows(LongEnum.IntMaxValue, Enums.ToInt16);
		TestOverflows(LongEnum.IntMaxValue, Enums.ToUInt16);
		TestOverflows(LongEnum.IntMaxValue, Enums.ToSByte);
		TestOverflows(LongEnum.IntMaxValue, Enums.ToByte);
	}
}
