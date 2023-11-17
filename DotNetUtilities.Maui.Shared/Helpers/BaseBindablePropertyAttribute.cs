using System.ComponentModel;

namespace DotNetUtilities.Maui.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class BaseBindablePropertyAttribute : Attribute
{
	public string Name { get; }

	public abstract Type PropertyType { get; }

	public abstract Type? AttachedType { get; }

	public object? DefaultValue => GetDefaultValue();

	public BindingMode DefaultMode { get; init; } = BindingMode.OneWay;

	public bool DefaultValueFactory { get; init; }

	public PropertyVisibility Visibility { get; init; }

	private PropertyVisibility? _writeVisibility;

	public PropertyVisibility WriteVisibility
	{
		get => _writeVisibility ?? Visibility;
		set => _writeVisibility = value;
	}

	private protected BaseBindablePropertyAttribute(string name)
	{
		Name = name;
	}

	protected private abstract object? GetDefaultValue();

	public bool IsReadOnly(out PropertyVisibility writeLevel)
	{
		writeLevel = _writeVisibility.GetValueOrDefault();
		return _writeVisibility.HasValue;
	}
}

public abstract class BaseBindablePropertyAttribute<TValue> : BaseBindablePropertyAttribute
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Type PropertyType => typeof(TValue);

	public new TValue? DefaultValue { get; init; }

	private protected BaseBindablePropertyAttribute(string name) : base(name)
	{
	}

	private protected override object? GetDefaultValue()
		=> DefaultValue;
}
