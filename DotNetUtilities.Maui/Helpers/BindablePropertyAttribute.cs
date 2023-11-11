namespace DotNetUtilities.Maui.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class BindablePropertyAttribute : Attribute
{
	public string Name { get; }

	public Type Type { get; }

	public BindingMode DefaultMode { get; init; }

	public PropertyAccessLevel AccessLevel { get; init; }

	private PropertyAccessLevel? _writeAccessLevel;

	public PropertyAccessLevel WriteAccessLevel
	{
		get => _writeAccessLevel ?? AccessLevel;
		set => _writeAccessLevel = value;
	}

	public BindablePropertyAttribute(string name, Type type)
	{
		Name = name;
		Type = type;
	}

	public bool IsReadOnly(out PropertyAccessLevel writeLevel)
	{
		writeLevel = _writeAccessLevel.GetValueOrDefault();
		return _writeAccessLevel.HasValue;
	}
}
