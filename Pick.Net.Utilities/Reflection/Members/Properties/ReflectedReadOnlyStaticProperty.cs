using System.Reflection;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal sealed class ReflectedReadOnlyStaticProperty<TProperty>(PropertyInfo member) : ReflectedStaticProperty<TProperty>(member), IReflectedReadableStaticProperty<TProperty>
{
	internal override string DebugSuffix => "{ get; }";

	public override Func<TProperty> Getter { get; } = member.GetMethod!.CreateFunc<TProperty>();

	public override Action<TProperty>? Setter => null;

	public TProperty GetValue()
		=> Getter.Invoke();

	public override IReflectedReadableStaticProperty AsReadable()
		=> this;

	public override IReflectedReadableStaticProperty<TExpectedProperty> AsReadable<TExpectedProperty>()
		=> AsReadable<TExpectedProperty>(this);

	public override IReflectedWritableStaticProperty<TExpectedProperty> AsWritable<TExpectedProperty>()
		=> throw new ArgumentException($"Property '{DeclaringType}.{Name}' is read-only.");

	public override IReflectedFullStaticProperty<TExpectedProperty> AsFull<TExpectedProperty>()
		=> throw new ArgumentException($"Property '{DeclaringType}.{Name}' is read-only.");
}