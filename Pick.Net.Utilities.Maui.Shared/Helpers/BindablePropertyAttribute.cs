using System.ComponentModel;

namespace Pick.Net.Utilities.Maui.Helpers;

public sealed class BindablePropertyAttribute<TValue> : BaseBindablePropertyAttribute<TValue>
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Type? AttachedType => null;

	public BindablePropertyAttribute(string name) : base(name)
	{
	}

	private protected override object? GetDefaultValue()
		=> DefaultValue;
}
