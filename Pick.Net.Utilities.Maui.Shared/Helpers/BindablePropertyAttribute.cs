namespace Pick.Net.Utilities.Maui;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class BindablePropertyAttribute : Attribute
{
	public string? DefaultValue { get; init; }

	public BindingMode DefaultMode { get; init; } = BindingMode.OneWay;

	public bool CoerceValueCallback { get; init; }

	public bool ValidateValueCallback { get; init; }

	public BindablePropertyAttribute()
	{
	}
}