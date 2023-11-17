using DotNetUtilities.Maui.Helpers;

namespace DotNetUtilities.Maui.TestApp.Controls;

[BindableProperty<string>("Test", AttachedType = typeof(BindableObject))]
public static partial class StaticClass
{
}
