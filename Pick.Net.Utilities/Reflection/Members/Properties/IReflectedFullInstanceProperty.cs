namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedFullInstanceProperty<in TOwner, TProperty> : IReflectedReadableInstanceProperty<TOwner, TProperty>, IReflectedWritableInstanceProperty<TOwner, TProperty>
{
}