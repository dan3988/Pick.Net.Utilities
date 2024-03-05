namespace Pick.Net.Utilities.Maui;

/// <summary>
/// Use to indicate that the annotated property/method is an accessor for an instance/attached bindable property and a backing <see cref="BindableProperty"/> should be generated.
/// <para>
/// The attribute can be used to generate instance properties
/// <code>
/// partial class MyBindableObject : BindableObject
/// {
///		[BindableProperty]
///		public string MyValue
///		{
///			get => GetValue(MyValueProperty);
///			set => SetValue(MyValueProperty, value);
///		}
/// }
/// </code>
/// This will generate a partial class with the static read-only field <c>MyValueProperty</c>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class BindablePropertyAttribute : Attribute
{
	/// <summary>
	/// The name of a property/field that should be passed as the value for <see cref="BindableProperty.DefaultValue"/>, or a method that can be used as the <see cref="BindableProperty.CreateDefaultValueDelegate"/> for the property.
	/// </summary>
	public string? DefaultValue { get; init; }

	/// <summary>
	/// The value of <see cref="BindableProperty.DefaultBindingMode"/> for the property.
	/// </summary>
	public BindingMode DefaultMode { get; init; } = BindingMode.OneWay;

	/// <summary>
	/// Whether to generate a method to pass in as the <see cref="BindableProperty.CoerceValueDelegate"/> of the property.
	/// </summary>
	public bool CoerceValueCallback { get; init; }

	/// <summary>
	/// Whether to generate a method to pass in as the <see cref="BindableProperty.ValidateValueDelegate"/> of the property.
	/// </summary>
	public bool ValidateValueCallback { get; init; }

	public BindablePropertyAttribute()
	{
	}
}