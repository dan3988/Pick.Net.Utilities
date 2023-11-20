namespace Pick.Net.Utilities.Maui.MarkupExtensions;

[AcceptEmptyServiceProvider]
public sealed class TrueExtension : IMarkupExtension<bool>
{
	bool IMarkupExtension<bool>.ProvideValue(IServiceProvider serviceProvider)
		=> true;

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> BooleanBox.True;
}
