using DotNetUtilities.Maui.TestApp.Controls;

using Microsoft.Extensions.Logging;

namespace DotNetUtilities.Maui.TestApp
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			var test = new TestControl
			{
				Text = "555",
				MinLength = 1,
				MaxLength = 5
			};

			return builder.Build();
		}
	}
}