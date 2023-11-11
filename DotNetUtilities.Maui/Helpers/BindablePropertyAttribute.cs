namespace DotNetUtilities.Maui.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class BindablePropertyAttribute : Attribute
{
	public string Name { get; }

	public Type Type { get; }

	public BindablePropertyAttribute(string name, Type type)
	{
		Name = name;
		Type = type;
	}
}
