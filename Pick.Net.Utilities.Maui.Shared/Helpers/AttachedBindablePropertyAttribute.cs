using System.ComponentModel;

namespace Pick.Net.Utilities.Maui.Helpers;

public sealed class AttachedBindablePropertyAttribute<TValue, TAttached> : BaseBindablePropertyAttribute<TValue>
	where TAttached : BindableObject
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Type AttachedType => typeof(TAttached);

	public AttachedBindablePropertyAttribute(string name) : base(name)
	{
	}
}
