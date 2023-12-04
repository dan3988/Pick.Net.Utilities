using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

internal partial class DefaultValueConversions : BindableObject
{
	public const byte ByteValue = 5;

	[BindableProperty(DefaultValue = nameof(ByteValue))]
	public byte Byte
	{
		get => (byte)GetValue(ByteProperty);
		set => SetValue(ByteProperty, value);
	}

	public const int DecimalIntValue = 5;

	[BindableProperty(DefaultValue = nameof(DecimalIntValue))]
	public double DecimalInt
	{
		get => (double)GetValue(DecimalIntProperty);
		set => SetValue(DecimalIntProperty, value);
	}

	public const long DecimalLongValue = 5;

	[BindableProperty(DefaultValue = nameof(DecimalLongValue))]
	public double DecimalLong
	{
		get => (double)GetValue(DecimalLongProperty);
		set => SetValue(DecimalLongProperty, value);
	}

	public const double DecimalDoubleValue = 5;

	[BindableProperty(DefaultValue = nameof(DecimalDoubleValue))]
	public double DecimalDouble
	{
		get => (double)GetValue(DecimalDoubleProperty);
		set => SetValue(DecimalDoubleProperty, value);
	}

	public const int FloatIntValue = 5;

	[BindableProperty(DefaultValue = nameof(FloatIntValue))]
	public float FloatInt
	{
		get => (float)GetValue(FloatIntProperty);
		set => SetValue(FloatIntProperty, value);
	}

	public const long FloatLongValue = 5;

	[BindableProperty(DefaultValue = nameof(FloatLongValue))]
	public float FloatLong
	{
		get => (float)GetValue(FloatLongProperty);
		set => SetValue(FloatLongProperty, value);
	}

	public const int IntUintValue = -1;

	[BindableProperty(DefaultValue = nameof(IntUintValue))]
	public uint IntUint
	{
		get => (uint)GetValue(IntUintProperty);
		set => SetValue(IntUintProperty, value);
	}

	public const int IntEnumValue = 5;

	[BindableProperty(DefaultValue = nameof(IntEnumValue))]
	public ConsoleColor IntEnum
	{
		get => (ConsoleColor)GetValue(IntEnumProperty);
		set => SetValue(IntEnumProperty, value);
	}

	public const int IntIConvertibleValue = 5;

	[BindableProperty(DefaultValue = nameof(IntIConvertibleValue))]
	public IConvertible IntIConvertible
	{
		get => (IConvertible)GetValue(IntIConvertibleProperty);
		set => SetValue(IntIConvertibleProperty, value);
	}

	public Element GenerateElement()
	{
		return new BoxView();
	}

	[BindableProperty(DefaultValue = nameof(GenerateElement))]
	public BindableObject Element
	{
		get => (BindableObject)GetValue(ElementProperty);
		set => SetValue(ElementProperty, value);
	}

	[BindableProperty(DefaultValue = nameof(GenerateElement))]
	public View View
	{
		get => (View)GetValue(ViewProperty);
		set => SetValue(ViewProperty, value);
	}
}