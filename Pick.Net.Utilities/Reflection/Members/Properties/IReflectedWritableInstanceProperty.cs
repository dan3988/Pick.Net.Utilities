namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedWritableInstanceProperty<in TOwner, in TProperty> : IReflectedInstanceProperty<TOwner>
{
	new Action<TOwner, TProperty> Setter { get; }

	void SetValue(TOwner owner, TProperty value);
}