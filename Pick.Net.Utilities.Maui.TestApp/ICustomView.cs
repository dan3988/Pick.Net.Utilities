namespace Pick.Net.Utilities.Maui.TestApp;

public interface ICustomView : IView
{
	[HandlerProperty]
	bool IsChecked { get; set; }

	[HandlerProperty]
	bool IsReadOnly { get; set; }

	[HandlerProperty]
	string? Title { get; set; }
}
