using Pick.Net.Utilities.Maui.TestApp.Controls;

namespace Pick.Net.Utilities.Maui.TestApp;

public static class Program
{
	public static void Main()
	{
		var test = new TestControl
		{
			Text = "text",
			MinLength = 5,
			MaxLength = 5645
		};
	}
}
