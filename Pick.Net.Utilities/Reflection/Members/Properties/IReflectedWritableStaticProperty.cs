namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedWritableStaticProperty<in TProperty> : IReflectedStaticProperty
{
	new Action<TProperty> Setter { get; }

	void SetValue(TProperty value);
}