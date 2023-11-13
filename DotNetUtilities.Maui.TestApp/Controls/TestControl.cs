using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string), DefaultMode = BindingMode.TwoWay)]
[BindableProperty("MaxLength", typeof(int))]
[BindableProperty("MinLength", typeof(int))]
[BindableProperty("TransformedText", typeof(string), WriteVisibility = PropertyVisibility.Private)]
[BindableProperty("SomethingInternal", typeof(string), Visibility = PropertyVisibility.Protected, WriteVisibility = PropertyVisibility.Private)]
[BindableProperty("Attached", typeof(bool), AttachedType = typeof(Entry))]
[BindableProperty("InternalAttached", typeof(bool), AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal)]
[BindableProperty("InternalReadOnlyAttached", typeof(bool), AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal, WriteVisibility = PropertyVisibility.Private)]
public partial class TestControl : BindableObject
{
}
