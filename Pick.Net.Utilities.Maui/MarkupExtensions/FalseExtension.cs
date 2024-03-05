namespace Pick.Net.Utilities.Maui.MarkupExtensions;

/// <summary>
/// Returns a boxed <c>false</c> boolean value.
/// </summary>
/// <remarks>
/// Useful use in cases when a boolean value is needed but the property type is <see cref="object"/>, e.g. <see cref="BindingExtension.FallbackValue"/>.
/// </remarks>
[AcceptEmptyServiceProvider]
public sealed class FalseExtension : IMarkupExtension<bool>
{
	bool IMarkupExtension<bool>.ProvideValue(IServiceProvider serviceProvider)
		=> false;

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> BooleanBox.False;
}
