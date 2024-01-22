using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal abstract class ReflectedStaticProperty : ReflectedProperty, IReflectedStaticProperty
{
	internal static ReflectedStaticProperty Create(PropertyInfo member)
	{
		Type type;

		if (member.CanRead)
		{
			type = member.CanWrite ? typeof(ReflectedFullStaticProperty<>) : typeof(ReflectedReadOnlyStaticProperty<>);
		}
		else
		{
			type = typeof(ReflectedWriteOnlyStaticProperty<>);
		}

		type = type.MakeGenericType(member.PropertyType);
		return (ReflectedStaticProperty)Activator.CreateInstance(type, member)!;
	}

	internal sealed override string DebugName => nameof(ReflectedInstanceProperty);

	private protected ReflectedStaticProperty(PropertyInfo member) : base(member)
	{
	}

	public abstract IReflectedReadableStaticProperty AsReadable();

	public abstract IReflectedReadableStaticProperty<TExpectedProperty> AsReadable<TExpectedProperty>();

	public abstract IReflectedWritableStaticProperty<TExpectedProperty> AsWritable<TExpectedProperty>();

	public abstract IReflectedFullStaticProperty<TExpectedProperty> AsFull<TExpectedProperty>();
}

internal abstract class ReflectedStaticProperty<TProperty> : ReflectedStaticProperty, IReflectedStaticProperty<TProperty>
{
	private protected static IReflectedReadableStaticProperty<TExpectedProperty> AsReadable<TExpectedProperty>(IReflectedReadableStaticProperty<TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (expected != actual && (actual.IsValueType || !actual.IsAssignableTo(expected)))
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedReadableStaticProperty<{actual}> cannot be cast to IReflectedReadableStaticProperty<{expected}>.");

		return Unsafe.As<IReflectedReadableStaticProperty<TExpectedProperty>>(self);
	}

	private protected static IReflectedWritableStaticProperty<TExpectedProperty> AsWritable<TExpectedProperty>(IReflectedWritableStaticProperty<TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (!expected.IsAssignableTo(actual))
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedWritableStaticProperty<{actual}> cannot be cast to IReflectedWritableStaticProperty<{expected}>.");

		return Unsafe.As<IReflectedWritableStaticProperty<TExpectedProperty>>(self);
	}

	private protected static IReflectedFullStaticProperty<TExpectedProperty> AsFull<TExpectedProperty>(IReflectedFullStaticProperty<TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (expected != actual)
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedFullStaticProperty<{actual}> cannot be cast to IReflectedFullStaticProperty<{expected}>.");

		return Unsafe.As<IReflectedFullStaticProperty<TExpectedProperty>>(self);
	}

	public abstract override Func<TProperty>? Getter { get; }

	public abstract override Action<TProperty>? Setter { get; }

	private protected ReflectedStaticProperty(PropertyInfo member) : base(member)
	{
	}
}
