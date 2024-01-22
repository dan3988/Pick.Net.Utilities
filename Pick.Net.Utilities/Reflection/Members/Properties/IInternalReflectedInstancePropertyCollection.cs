namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal interface IInternalReflectedInstancePropertyCollection<in T> : IReflectedInstancePropertyCollection<T>
{
	IInternalReflectedInstanceProperty<T>? TryGetDeclared(ReadOnlySpan<char> key);
}