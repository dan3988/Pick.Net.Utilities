using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

public partial class TestControl : BindableObject
{
	private static string CreateAttached()
		=> "default value";

	[BindableProperty(ValidateValueCallback = true, CoerceValueCallback = true, DefaultValue = nameof(CreateAttached))]
	public static string GetAttached(Entry entry)
		=> (string)entry.GetValue(AttachedProperty);

	public static void SetAttached(Entry entry, string value)
		=> entry.SetValue(AttachedProperty, value);

	private static partial bool ValidateAttachedValue(string value)
		=> value != null;

	private static partial string CoerceAttachedValue(string value)
		=> value;

	[BindableProperty(DefaultMode = BindingMode.TwoWay, ValidateValueCallback = true, CoerceValueCallback = true)]
	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	private string CreateTransformedText()
		=> Text;

	[BindableProperty(DefaultValue = nameof(CreateTransformedText))]
	public string TransformedText
	{
		get => (string)GetValue(TransformedTextProperty);
		private set => SetValue(TransformedTextPropertyKey, value);
	}

	private const int MaxLengthDefaultValue = 0;

	[BindableProperty(DefaultValue = nameof(MaxLengthDefaultValue))]
	public int MaxLength
	{
		get => (int)GetValue(MaxLengthProperty);
		set => SetValue(MaxLengthProperty, value);
	}

	private const int MinLengthDefaultValue = 4000;

	[BindableProperty(DefaultValue = nameof(MinLengthDefaultValue))]
	public int MinLength
	{
		get => (int)GetValue(MinLengthProperty);
		set => SetValue(MinLengthProperty, value);
	}

	private const string SomethingInternalDefaultValue = "Default Value";

	[BindableProperty(DefaultValue = nameof(SomethingInternalDefaultValue))]
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
