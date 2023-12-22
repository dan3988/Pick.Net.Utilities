using Microsoft.Maui.Handlers;

namespace Pick.Net.Utilities.Maui.TestApp;

[HandlerMapper(typeof(ICustomView))]
public partial class CustomViewHandler : ViewHandler<ICustomView, object>
{
	public CustomViewHandler() : base(GeneratedMapper)
	{
	}

	protected override object CreatePlatformView()
	{
		throw new NotImplementedException();
	}
}