using System;

namespace DotNetUtilities.Maui.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class BindablePropertyAttribute : Attribute
{
	public string Name { get; }

	public Type Type { get; }

	public BindingMode DefaultMode { get; init; } = BindingMode.OneWay;

	public Type? AttachedType { get; init; }

	public object? DefaultValue { get; init; }
	
	public PropertyVisibility Visibility { get; init; }

	private PropertyVisibility? _writeVisibility;

	public PropertyVisibility WriteVisibility
	{
		get => _writeVisibility ?? Visibility;
		set => _writeVisibility = value;
	}

	public BindablePropertyAttribute(string name, Type type)
	{
		Name = name;
		Type = type;
	}

	public bool IsReadOnly(out PropertyVisibility writeLevel)
	{
		writeLevel = _writeVisibility.GetValueOrDefault();
		return _writeVisibility.HasValue;
	}
}
