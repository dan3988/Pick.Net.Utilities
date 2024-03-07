namespace Pick.Net.Utilities.Maui.MarkupExtensions;

/// <summary>
/// Returns a list of the names of the enum constants declared in the specified enum type.
/// </summary>
[ContentProperty(nameof(Type))]
[AcceptEmptyServiceProvider]
public sealed class EnumNamesExtension : IMarkupExtension<IList<string>>
{
	/// <summary>
	/// The type of enum to get values for
	/// </summary>
	public required Type Type { get; init; }

	/// <inheritdoc/>
	public IList<string> ProvideValue(IServiceProvider serviceProvider)
		=> Enums.GetNameList(Type);

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> ProvideValue(serviceProvider);
}
