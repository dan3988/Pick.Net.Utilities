namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class MaybeNullWhenAttribute : Attribute
{
	public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

	public bool ReturnValue { get; }
}