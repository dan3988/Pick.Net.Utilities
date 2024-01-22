namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedStaticPropertyCollection : IReflectedPropertyCollection
{
	IReflectedStaticPropertyCollection? BaseProperties { get; }

	IReflectedProperty IReflectedPropertyCollection.GetProperty(ReadOnlySpan<char> key)
		=> GetProperty(key);

	new IReflectedStaticProperty GetProperty(ReadOnlySpan<char> key);

	IReflectedProperty? IReflectedPropertyCollection.TryGetProperty(ReadOnlySpan<char> key)
		=> TryGetProperty(key);

	new IReflectedStaticProperty? TryGetProperty(ReadOnlySpan<char> key);

	IReflectedReadableStaticProperty<TProperty> GetReadableProperty<TProperty>(ReadOnlySpan<char> key);

	IReflectedWritableStaticProperty<TProperty> GetWritableProperty<TProperty>(ReadOnlySpan<char> key);

	IReflectedFullStaticProperty<TProperty> GetFullProperty<TProperty>(ReadOnlySpan<char> key);
}
