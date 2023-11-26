using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

public static partial class StaticClass
{
	[BindableProperty]
	public static partial string GetTest(Label label);

	public static void SetTest(Label label, string value)
		=> label.SetValue(TestProperty, value);
}
