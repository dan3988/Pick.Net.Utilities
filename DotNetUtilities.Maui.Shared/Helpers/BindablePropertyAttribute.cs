using System;

namespace DotNetUtilities.Maui.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class BindablePropertyAttribute : Attribute
{
	public string Name { get; }

	public abstract Type PropertyType { get; }

	public object? DefaultValue
		=> GetDefaultValue();

	public BindingMode DefaultMode { get; init; } = BindingMode.OneWay;

	public Type? AttachedType { get; init; }

	public bool DefaultValueFactory { get; init; }

	public PropertyVisibility Visibility { get; init; }

	private PropertyVisibility? _writeVisibility;

	public PropertyVisibility WriteVisibility
	{
		get => _writeVisibility ?? Visibility;
		set => _writeVisibility = value;
	}

	private protected BindablePropertyAttribute(string name)
	{
		Name = name;
	}

	private protected abstract object? GetDefaultValue();

	public bool IsReadOnly(out PropertyVisibility writeLevel)
	{
		writeLevel = _writeVisibility.GetValueOrDefault();
		return _writeVisibility.HasValue;
	}
}

public sealed class BindablePropertyAttribute<T> : BindablePropertyAttribute
{
	public override Type PropertyType => typeof(T);

	public new T? DefaultValue { get; init; }

	public BindablePropertyAttribute(string name) : base(name)
	{
	}

	private protected override object? GetDefaultValue()
		=> DefaultValue;
}
