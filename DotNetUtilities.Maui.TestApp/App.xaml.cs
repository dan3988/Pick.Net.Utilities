﻿namespace DotNetUtilities.Maui.TestApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();
		}
	}
}