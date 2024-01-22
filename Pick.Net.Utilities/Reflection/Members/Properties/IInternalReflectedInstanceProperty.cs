namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal interface IInternalReflectedInstanceProperty<in TOwner> : IReflectedInstanceProperty<TOwner>
{
	IReflectedReadableInstanceProperty<TOwner> AsReadable();

	IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>();

	IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>();

	IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>();
}