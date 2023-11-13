using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string), DefaultMode = BindingMode.TwoWay)]
[BindableProperty("MaxLength", typeof(int))]
[BindableProperty("MinLength", typeof(int))]
[BindableProperty("TransformedText", typeof(string), WriteVisibility = PropertyVisibility.Private)]
[BindableProperty("AttachedProperty", typeof(bool), AttachedType = typeof(Entry))]
[BindableProperty("PrivateAttachedProperty", typeof(bool), AttachedType = typeof(Entry), Visibility = PropertyVisibility.Internal)]
public partial class TestControl : BindableObject
{
}
