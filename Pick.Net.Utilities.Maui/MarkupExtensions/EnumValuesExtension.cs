using System.Collections;

namespace Pick.Net.Utilities.Maui.MarkupExtensions;

/// <summary>
/// Returns a list of enum constants declared in the specified enum type.
/// </summary>
[ContentProperty(nameof(Type))]
[AcceptEmptyServiceProvider]
public sealed class EnumValuesExtension : IMarkupExtension<IList>
{
	/// <summary>
	/// The type of enum to get values for
	/// </summary>
	public required Type Type { get; init; }

	/// <inheritdoc/>
	public IList ProvideValue(IServiceProvider serviceProvider)
		=> Enums.GetValueList(Type);

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> ProvideValue(serviceProvider);
}
