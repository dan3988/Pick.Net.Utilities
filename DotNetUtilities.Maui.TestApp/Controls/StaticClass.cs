using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty("Test", typeof(string), AttachedType = typeof(BindableObject))]
public static partial class StaticClass
{
}
