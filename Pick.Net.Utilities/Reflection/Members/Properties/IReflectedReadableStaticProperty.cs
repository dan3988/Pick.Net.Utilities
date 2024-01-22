namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedReadableStaticProperty : IReflectedStaticProperty
{
	object? GetValue();
}

public interface IReflectedReadableStaticProperty<out TProperty> : IReflectedReadableStaticProperty
{
	new Func<TProperty> Getter { get; }

	object? IReflectedReadableStaticProperty.GetValue()
		=> GetValue();

	new TProperty GetValue();
}