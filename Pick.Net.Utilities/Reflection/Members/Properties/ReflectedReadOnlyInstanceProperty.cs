using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedReadOnlyInstanceProperty<TOwner, TProperty>(PropertyInfo member) : ReflectedInstanceProperty<TOwner, TProperty>(member), IReflectedReadableInstanceProperty<TOwner, TProperty>
{
	internal override string DebugSuffix => "{ get; }";

	public override Func<TOwner, TProperty> Getter { get; } = member.GetMethod!.CreateFunc<TOwner, TProperty>();

	public override Action<TOwner, TProperty>? Setter => null;

	public TProperty GetValue(TOwner owner)
		=> Getter.Invoke(owner);

	public override IReflectedReadableInstanceProperty<TOwner> AsReadable()
		=> this;

	public override IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>()
		=> AsReadable<TExpectedProperty>(this);

	public override IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>()
		=> throw new InvalidOperationException($"Property '{DeclaringType}.{Name}' is read-only.");

	public override IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>()
		=> throw new InvalidOperationException($"Property '{DeclaringType}.{Name}' is read-only.");
}