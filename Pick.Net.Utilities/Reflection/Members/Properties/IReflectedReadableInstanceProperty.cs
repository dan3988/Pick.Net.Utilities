namespace Pick.Net.Utilities.Reflection.Members.Properties;

public interface IReflectedReadableInstanceProperty<in TOwner> : IReflectedInstanceProperty<TOwner>
{
	object? GetValue(TOwner owner);
}

public interface IReflectedReadableInstanceProperty<in TOwner, out TProperty> : IReflectedReadableInstanceProperty<TOwner>
{
	new Func<TOwner, TProperty> Getter { get; }

	object? IReflectedReadableInstanceProperty<TOwner>.GetValue(TOwner owner)
		=> GetValue(owner);

	new TProperty GetValue(TOwner owner);
}