using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string))]
[BindableProperty("MaxLength", typeof(int))]
[BindableProperty("MinLength", typeof(int))]
[BindableProperty("TransformedText", typeof(string), WriteAccessLevel = PropertyAccessLevel.Private)]
[BindableProperty("AttachedProperty", typeof(TestControl), AttachedType = typeof(Entry))]
[BindableProperty("PrivateAttachedProperty", typeof(TestControl), AttachedType = typeof(Entry), WriteAccessLevel = PropertyAccessLevel.Internal)]
public partial class TestControl : BindableObject
{
}
