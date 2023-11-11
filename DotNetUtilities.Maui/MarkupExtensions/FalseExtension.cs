namespace DotNetUtilities.Maui.MarkupExtensions;

[AcceptEmptyServiceProvider]
public sealed class FalseExtension : IMarkupExtension<bool>
{
	bool IMarkupExtension<bool>.ProvideValue(IServiceProvider serviceProvider)
		=> false;

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> BooleanBox.False;
}
