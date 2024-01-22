using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedWriteOnlyInstanceProperty<TOwner, TProperty>(PropertyInfo member) : ReflectedInstanceProperty<TOwner, TProperty>(member), IReflectedWritableInstanceProperty<TOwner, TProperty>
{
	internal override string DebugSuffix => "{ set; }";

	public override Func<TOwner, TProperty>? Getter => null;

	public override Action<TOwner, TProperty> Setter { get; } = member.SetMethod!.CreateAction<TOwner, TProperty>();

	public void SetValue(TOwner owner, TProperty value)
		=> Setter.Invoke(owner, value);

	public override IReflectedReadableInstanceProperty<TOwner> AsReadable()
		=> throw new InvalidOperationException($"Property '{typeof(TOwner)}.{Name}' is write-only.");

	public override IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>()
		=> throw new InvalidOperationException($"Property '{typeof(TOwner)}.{Name}' is write-only.");

	public override IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>()
		=> AsWritable<TExpectedProperty>(this);

	public override IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>()
		=> throw new InvalidOperationException($"Property '{typeof(TOwner)}.{Name}' is write-only.");
}