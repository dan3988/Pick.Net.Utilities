using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string), DefaultMode = BindingMode.TwoWay)]
[BindableProperty("MaxLength", typeof(int), DefaultValue = 0)]
[BindableProperty("MinLength", typeof(int), DefaultValue = 4000)]
[BindableProperty("TransformedText", typeof(string), WriteVisibility = PropertyVisibility.Private)]
[BindableProperty("SomethingInternal", typeof(string), DefaultValue = "Default Value", Visibility = PropertyVisibility.Protected, WriteVisibility = PropertyVisibility.Private)]
[BindableProperty("Attached", typeof(BindableObject), AttachedType = typeof(Entry))]
[BindableProperty("InternalAttached", typeof(bool), AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal)]
[BindableProperty("InternalReadOnlyAttached", typeof(bool), AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal, WriteVisibility = PropertyVisibility.Private)]
public partial class TestControl : BindableObject
{
	[BindableProperty("Value", typeof(int), DefaultMode = BindingMode.TwoWay)]
	public partial class NestedControl : BindableObject
	{
		[BindableProperty("Value", typeof(int), DefaultMode = BindingMode.TwoWay)]
		public partial class DeepNestedControl : BindableObject
		{
		}
	}
}
