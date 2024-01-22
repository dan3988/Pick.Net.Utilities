using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedFullInstanceProperty<TOwner, TProperty>(PropertyInfo member) : ReflectedInstanceProperty<TOwner, TProperty>(member), IReflectedFullInstanceProperty<TOwner, TProperty>
{
	internal override string DebugSuffix => "{ get; set; }";

	public override Func<TOwner, TProperty> Getter { get; } = member.GetMethod!.CreateFunc<TOwner, TProperty>();

	public override Action<TOwner, TProperty> Setter { get; } = member.SetMethod!.CreateAction<TOwner, TProperty>();

	public TProperty GetValue(TOwner owner)
		=> Getter.Invoke(owner);

	public void SetValue(TOwner owner, TProperty value)
		=> Setter.Invoke(owner, value);

	public override IReflectedReadableInstanceProperty<TOwner> AsReadable()
		=> this;

	public override IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>()
		=> AsReadable<TExpectedProperty>(this);

	public override IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>()
		=> AsWritable<TExpectedProperty>(this);

	public override IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>()
		=> AsFull<TExpectedProperty>(this);
}