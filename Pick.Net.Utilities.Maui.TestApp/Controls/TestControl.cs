using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

public partial class TestControl : BindableObject
{
	[BindableProperty(ValidateValueCallback = true, CoerceValueCallback = true, DefaultValueFactory = true)]
	public static string GetAttached(Entry entry)
		=> (string)entry.GetValue(AttachedProperty);

	public static void SetAttached(Entry entry, string value)
		=> entry.SetValue(AttachedProperty, value);

	private static partial bool ValidateAttachedValue(string? value)
		=> value != null;

	private static partial string CoerceAttachedValue(string value)
		=> value;

	private static partial string GenerateAttachedDefaultValue(Entry bindable)
		=> "default value";

	[BindableProperty(DefaultValue = "", DefaultMode = BindingMode.TwoWay, ValidateValueCallback = true, CoerceValueCallback = true)]
	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	[BindableProperty(DefaultValueFactory = true)]
	public string TransformedText
	{
		get => (string)GetValue(TransformedTextProperty);
		private set => SetValue(TransformedTextPropertyKey, value);
	}

	private partial string GenerateTransformedTextDefaultValue()
		=> Text;

	[BindableProperty(DefaultValue = 0)]
	public int MaxLength
	{
		get => (int)GetValue(MaxLengthProperty);
		set => SetValue(MaxLengthProperty, value);
	}

	[BindableProperty(DefaultValue = 4000)]
	public int MinLength
	{
		get => (int)GetValue(MinLengthProperty);
		set => SetValue(MinLengthProperty, value);
	}

	[BindableProperty(DefaultValue = "Default Value")]
	public string SomethingInternal
	{
		get => (string)GetValue(SomethingInternalProperty);
		set => SetValue(SomethingInternalProperty, value);
	}

	private partial bool ValidateTextValue(string value)
		=> value.Length >= MinLength && value.Length <= MaxLength;

	private partial string CoerceTextValue(string value)
		=> value.ToUpper();

	public partial class NestedControl : BindableObject
	{
		[BindableProperty(DefaultMode = BindingMode.TwoWay)]
		public int Value
		{
			get => (int)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public partial class DeepNestedControl : BindableObject
		{
			[BindableProperty(DefaultMode = BindingMode.TwoWay)]
			public int Value
			{
				get => (int)GetValue(ValueProperty);
				set => SetValue(ValueProperty, value);
			}
		}
	}
}
