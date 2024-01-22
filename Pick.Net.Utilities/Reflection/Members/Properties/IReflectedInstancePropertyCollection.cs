namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedInstancePropertyCollection : IReflectedPropertyCollection
{
	IReflectedInstancePropertyCollection? BaseProperties { get; }

	IReflectedProperty IReflectedPropertyCollection.GetProperty(ReadOnlySpan<char> key)
		=> GetProperty(key);

	new IReflectedInstanceProperty GetProperty(ReadOnlySpan<char> key);

	IReflectedProperty? IReflectedPropertyCollection.TryGetProperty(ReadOnlySpan<char> key)
		=> TryGetProperty(key);

	new IReflectedInstanceProperty? TryGetProperty(ReadOnlySpan<char> key);
}

public interface IReflectedInstancePropertyCollection<in T> : IReflectedInstancePropertyCollection
{
	IReflectedInstanceProperty IReflectedInstancePropertyCollection.GetProperty(ReadOnlySpan<char> key)
		=> GetProperty(key);

	new IReflectedInstanceProperty<T> GetProperty(ReadOnlySpan<char> key);

	IReflectedInstanceProperty? IReflectedInstancePropertyCollection.TryGetProperty(ReadOnlySpan<char> key)
		=> TryGetProperty(key);

	new IReflectedInstanceProperty<T>? TryGetProperty(ReadOnlySpan<char> key);

	IReflectedReadableInstanceProperty<T, TProperty> GetReadableProperty<TProperty>(ReadOnlySpan<char> key);

	IReflectedWritableInstanceProperty<T, TProperty> GetWritableProperty<TProperty>(ReadOnlySpan<char> key);

	IReflectedFullInstanceProperty<T, TProperty> GetFullProperty<TProperty>(ReadOnlySpan<char> key);
}
