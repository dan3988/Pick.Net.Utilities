using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty<string>("Text")]
[BindableProperty<int>("MaxLength", DefaultValue = 0)]
[BindableProperty<int>("MinLength", DefaultValue = 4000)]
[BindableProperty<string>("TransformedText", WriteVisibility = PropertyVisibility.Private)]
[BindableProperty<string>("SomethingInternal", DefaultValue = "Default Value", Visibility = PropertyVisibility.Protected, WriteVisibility = PropertyVisibility.Private)]
[BindableProperty<BindableObject>("Attached", AttachedType = typeof(Entry))]
[BindableProperty<bool>("InternalAttached", AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal)]
[BindableProperty<bool>("InternalReadOnlyAttached", AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal, WriteVisibility = PropertyVisibility.Private)]
public partial class TestControl : BindableObject
{
	[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
	public partial class NestedControl : BindableObject
	{
		[BindableProperty<int>("Value", DefaultMode = BindingMode.TwoWay)]
		public partial class DeepNestedControl : BindableObject
		{
		}
	}
}
