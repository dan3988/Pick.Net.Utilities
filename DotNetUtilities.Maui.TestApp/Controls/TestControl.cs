using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty<string>("Text", DefaultValueFactory = true, ValidateValueCallback = true, CoerceValueCallback = true)]
[BindableProperty<int>("MaxLength", DefaultValue = 0)]
[BindableProperty<int>("MinLength", DefaultValue = 4000)]
[BindableProperty<string>("TransformedText", WriteVisibility = PropertyVisibility.Private)]
[BindableProperty<string>("SomethingInternal", DefaultValue = "Default Value", Visibility = PropertyVisibility.Protected, WriteVisibility = PropertyVisibility.Private)]
[AttachedBindableProperty<BindableObject, Entry>("Attached", DefaultValueFactory = true)]
[AttachedBindableProperty<bool, Entry>("InternalAttached", Visibility = PropertyVisibility.Internal)]
[AttachedBindableProperty<bool, Entry>("InternalReadOnlyAttached", Visibility = PropertyVisibility.Internal, WriteVisibility = PropertyVisibility.Private)]
public partial class TestControl : BindableObject
{
	private static partial BindableObject GenerateAttachedDefaultValue(Entry bindable)
	{
		return new Label();
	}

	private partial string GenerateTextDefaultValue()
	{
		return "";
	}

	[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
	public partial class NestedControl : BindableObject
	{
		[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
		public partial class DeepNestedControl : BindableObject
		{
		}
	}
}
