namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedStaticProperty : IReflectedProperty
{
}

public interface IReflectedStaticProperty<TProperty> : IReflectedStaticProperty
{
	new Func<TProperty>? Getter { get; }

	new Action<TProperty>? Setter { get; }
}
