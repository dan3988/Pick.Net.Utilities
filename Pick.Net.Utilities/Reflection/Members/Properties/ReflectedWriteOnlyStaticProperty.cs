using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedWriteOnlyStaticProperty<TProperty>(PropertyInfo member) : ReflectedStaticProperty<TProperty>(member), IReflectedWritableStaticProperty<TProperty>
{
	internal override string DebugSuffix => "{ set; }";

	public override Func<TProperty>? Getter => null;

	public override Action<TProperty> Setter { get; } = member.SetMethod!.CreateAction<TProperty>();

	public void SetValue(TProperty value)
		=> Setter.Invoke(value);

	public override IReflectedReadableStaticProperty AsReadable()
		=> throw new ArgumentException($"Property '{DeclaringType}.{Name}' is write-only.");

	public override IReflectedReadableStaticProperty<TExpectedProperty> AsReadable<TExpectedProperty>()
		=> throw new ArgumentException($"Property '{DeclaringType}.{Name}' is write-only.");

	public override IReflectedWritableStaticProperty<TExpectedProperty> AsWritable<TExpectedProperty>()
		=> AsWritable<TExpectedProperty>(this);

	public override IReflectedFullStaticProperty<TExpectedProperty> AsFull<TExpectedProperty>()
		=> throw new ArgumentException($"Property '{DeclaringType}.{Name}' is write-only.");
}