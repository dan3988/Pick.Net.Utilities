using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

internal partial class DefaultValueConversions : BindableObject
{
	[BindableProperty(DefaultValue = (byte)5)]
	public byte Byte
	{
		get => (byte)GetValue(ByteProperty);
		set => SetValue(ByteProperty, value);
	}

	[BindableProperty(DefaultValue = 10)]
	public double DecimalInt
	{
		get => (double)GetValue(DecimalIntProperty);
		set => SetValue(DecimalIntProperty, value);
	}

	[BindableProperty(DefaultValue = 10L)]
	public double DecimalLong
	{
		get => (double)GetValue(DecimalLongProperty);
		set => SetValue(DecimalLongProperty, value);
	}

	[BindableProperty(DefaultValue = 5.5d)]
	public double DecimalDouble
	{
		get => (double)GetValue(DecimalDoubleProperty);
		set => SetValue(DecimalDoubleProperty, value);
	}

	[BindableProperty(DefaultValue = 10)]
	public float FloatInt
	{
		get => (float)GetValue(FloatIntProperty);
		set => SetValue(FloatIntProperty, value);
	}

	[BindableProperty(DefaultValue = 10L)]
	public float FloatLong
	{
		get => (float)GetValue(FloatLongProperty);
		set => SetValue(FloatLongProperty, value);
	}

	[BindableProperty(DefaultValue = 5u)]
	public uint IntUint
	{
		get => (uint)GetValue(IntUintProperty);
		set => SetValue(IntUintProperty, value);
	}
}