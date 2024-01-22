namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedInstanceProperty : IReflectedProperty
{
}

public interface IReflectedInstanceProperty<in TOwner> : IReflectedInstanceProperty
{
}

public interface IReflectedInstanceProperty<in TOwner, TProperty> : IReflectedInstanceProperty<TOwner>
{
	new Func<TOwner, TProperty>? Getter { get; }

	new Action<TOwner, TProperty>? Setter { get; }
}