namespace Pick.Net.Utilities.Maui;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HandlerMapperAttribute(Type viewType) : Attribute
{
	public Type ViewType { get; } = viewType;
}
