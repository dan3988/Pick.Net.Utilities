namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedPropertyCollection : IReadOnlyCollection<IReflectedProperty>
{
	Type DeclaringType { get; }

	IReflectedProperty GetProperty(ReadOnlySpan<char> key);

	IReflectedProperty? TryGetProperty(ReadOnlySpan<char> key);
}