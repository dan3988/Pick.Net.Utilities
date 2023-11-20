using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

[BindableProperty<string>("Text", DefaultValueFactory = true, ValidateValueCallback = true, CoerceValueCallback = true)]
[BindableProperty<int>("MaxLength", DefaultValue = 0)]
[BindableProperty<int>("MinLength", DefaultValue = 4000)]
[BindableProperty<string>("TransformedText", WriteVisibility = PropertyVisibility.Private)]
[BindableProperty<string>("SomethingInternal", DefaultValue = "Default Value", Visibility = PropertyVisibility.Protected, WriteVisibility = PropertyVisibility.Private)]
[AttachedBindableProperty<BindableObject, Entry>("Attached", DefaultValueFactory = true, ValidateValueCallback = true, CoerceValueCallback = true)]
[AttachedBindableProperty<bool, Entry>("InternalAttached", Visibility = PropertyVisibility.Internal)]
[AttachedBindableProperty<bool, Entry>("InternalReadOnlyAttached", Visibility = PropertyVisibility.Internal, WriteVisibility = PropertyVisibility.Private)]
public partial class TestControl : BindableObject
{
	private static partial bool ValidateAttachedValue(BindableObject? value)
		=> value != null;

	private static partial BindableObject CoerceAttachedValue(BindableObject value)
		=> value;

	private static partial BindableObject GenerateAttachedDefaultValue(Entry bindable)
	{
		return new Label();
	}

	private partial string GenerateTextDefaultValue()
	{
		return "";
	}

	private partial bool ValidateTextValue(string value)
		=> value.Length >= MinLength && value.Length <= MaxLength;

	private partial string CoerceTextValue(string value)
		=> value.ToUpper();

	[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
	public partial class NestedControl : BindableObject
	{
		[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
		public partial class DeepNestedControl : BindableObject
		{
		}
	}
}
