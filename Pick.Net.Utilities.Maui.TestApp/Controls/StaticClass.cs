using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.TestApp.Controls;

public static partial class StaticClass
{
	[BindableProperty]
	public static string GetTest(Label label)
		=> (string)label.GetValue(TestProperty);

	public static void SetTest(Label label, string value)
		=> label.SetValue(TestProperty, value);
}
