using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Text", typeof(string))]
[BindableProperty("MaxLength", typeof(int))]
[BindableProperty("MinLength", typeof(int))]
public partial class TestControl : BindableObject
{
}
