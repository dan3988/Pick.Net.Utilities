using System;
using System.ComponentModel;

namespace DotNetUtilities.Maui.Helpers;

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
