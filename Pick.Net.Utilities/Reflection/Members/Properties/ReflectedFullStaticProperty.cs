using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedFullStaticProperty<TProperty>(PropertyInfo member) : ReflectedStaticProperty<TProperty>(member), IReflectedFullStaticProperty<TProperty>
{
	internal override string DebugSuffix => "{ get; set; }";

	public override Func<TProperty> Getter { get; } = member.GetMethod!.CreateFunc<TProperty>();

	public override Action<TProperty> Setter { get; } = member.SetMethod!.CreateAction<TProperty>();

	public TProperty GetValue()
		=> Getter.Invoke();

	public void SetValue(TProperty value)
		=> Setter.Invoke(value);

	public override IReflectedReadableStaticProperty AsReadable()
		=> this;

	public override IReflectedReadableStaticProperty<TExpectedProperty> AsReadable<TExpectedProperty>()
		=> AsReadable<TExpectedProperty>(this);

	public override IReflectedWritableStaticProperty<TExpectedProperty> AsWritable<TExpectedProperty>()
		=> AsWritable<TExpectedProperty>(this);

	public override IReflectedFullStaticProperty<TExpectedProperty> AsFull<TExpectedProperty>()
		=> AsFull<TExpectedProperty>(this);
}