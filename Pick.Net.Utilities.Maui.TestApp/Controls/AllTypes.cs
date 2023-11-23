using System.Reflection;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

internal partial class AllTypes : BindableObject
{
	[BindableProperty(DefaultValue = 5)]
	public object Object
	{
		get => (object)GetValue(ObjectProperty);
		set => SetValue(ObjectProperty, value);
	}

	[BindableProperty(DefaultValue = true)]
	public bool Boolean
	{
		get => (bool)GetValue(BooleanProperty);
		set => SetValue(BooleanProperty, value);
	}

	[BindableProperty(DefaultValue = 'A')]
	public char Char
	{
		get => (char)GetValue(CharProperty);
		set => SetValue(CharProperty, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public sbyte SByte
	{
		get => (sbyte)GetValue(SByteProperty);
		set => SetValue(SByteProperty, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public byte Byte
	{
		get => (byte)GetValue(ByteProperty);
		set => SetValue(ByteProperty, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public short Int16
	{
		get => (short)GetValue(Int16Property);
		set => SetValue(Int16Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public ushort UInt16
	{
		get => (ushort)GetValue(UInt16Property);
		set => SetValue(UInt16Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public int Int32
	{
		get => (int)GetValue(Int32Property);
		set => SetValue(Int32Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public uint UInt32
	{
		get => (uint)GetValue(UInt32Property);
		set => SetValue(UInt32Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public long Int64
	{
		get => (long)GetValue(Int64Property);
		set => SetValue(Int64Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public ulong UInt64
	{
		get => (ulong)GetValue(UInt64Property);
		set => SetValue(UInt64Property, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public float Single
	{
		get => (float)GetValue(SingleProperty);
		set => SetValue(SingleProperty, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public double Double
	{
		get => (double)GetValue(DoubleProperty);
		set => SetValue(DoubleProperty, value);
	}

	[BindableProperty(DefaultValue = 1)]
	public decimal Decimal
	{
		get => (decimal)GetValue(DecimalProperty);
		set => SetValue(DecimalProperty, value);
	}

	[BindableProperty(DefaultValue = "text")]
	public string String
	{
		get => (string)GetValue(StringProperty);
		set => SetValue(StringProperty, value);
	}

	[BindableProperty(DefaultValue = ConsoleColor.Red)]
	public ConsoleColor Enum
	{
		get => (ConsoleColor)GetValue(EnumProperty);
		set => SetValue(EnumProperty, value);
	}

	[BindableProperty(DefaultValue = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)]
	public BindingFlags FlagEnum
	{
		get => (BindingFlags)GetValue(FlagEnumProperty);
		set => SetValue(FlagEnumProperty, value);
	}
}
