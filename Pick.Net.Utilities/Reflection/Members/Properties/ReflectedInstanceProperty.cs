using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

internal abstract class ReflectedInstanceProperty : ReflectedProperty, IReflectedInstanceProperty
{
	internal static ReflectedInstanceProperty<T> Create<T>(PropertyInfo member)
	{
		Type type;

		if (member.CanRead)
		{
			type = member.CanWrite ? typeof(ReflectedFullInstanceProperty<,>) : typeof(ReflectedReadOnlyInstanceProperty<,>);
		}
		else
		{
			type = typeof(ReflectedWriteOnlyInstanceProperty<,>);
		}

		type = type.MakeGenericType(typeof(T), member.PropertyType);
		return (ReflectedInstanceProperty<T>)Activator.CreateInstance(type, member)!;
	}

	internal sealed override string DebugName => nameof(ReflectedInstanceProperty);

	private protected ReflectedInstanceProperty(PropertyInfo member) : base(member)
	{
	}
}

internal abstract class ReflectedInstanceProperty<TOwner> : ReflectedInstanceProperty, IInternalReflectedInstanceProperty<TOwner>
{
	private protected ReflectedInstanceProperty(PropertyInfo member) : base(member)
	{
	}

	public abstract IReflectedReadableInstanceProperty<TOwner> AsReadable();

	public abstract IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>();

	public abstract IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>();

	public abstract IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>();
}

internal abstract class ReflectedInstanceProperty<TOwner, TProperty> : ReflectedInstanceProperty<TOwner>, IReflectedInstanceProperty<TOwner, TProperty>
{
	private protected static IReflectedReadableInstanceProperty<TOwner, TExpectedProperty> AsReadable<TExpectedProperty>(IReflectedReadableInstanceProperty<TOwner, TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (expected != actual && (actual.IsValueType || !actual.IsAssignableTo(expected)))
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedReadableInstanceProperty<{typeof(TOwner)}, {actual}> cannot be cast to IReflectedReadableInstanceProperty<{typeof(TOwner)}, {expected}>.");

		return Unsafe.As<IReflectedReadableInstanceProperty<TOwner, TExpectedProperty>>(self);
	}

	private protected static IReflectedWritableInstanceProperty<TOwner, TExpectedProperty> AsWritable<TExpectedProperty>(IReflectedWritableInstanceProperty<TOwner, TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (!expected.IsAssignableTo(actual))
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedWritableInstanceProperty<{typeof(TOwner)}, {actual}> cannot be cast to IReflectedWritableInstanceProperty<{typeof(TOwner)}, {expected}>.");

		return Unsafe.As<IReflectedWritableInstanceProperty<TOwner, TExpectedProperty>>(self);
	}

	private protected static IReflectedFullInstanceProperty<TOwner, TExpectedProperty> AsFull<TExpectedProperty>(IReflectedFullInstanceProperty<TOwner, TProperty> self)
	{
		var (expected, actual) = (typeof(TExpectedProperty), typeof(TProperty));
		if (expected != actual)
			throw new InvalidOperationException($"Property '{self.Name}' IReflectedFullInstanceProperty<{typeof(TOwner)}, {actual}> cannot be cast to IReflectedFullInstanceProperty<{typeof(TOwner)}, {expected}>.");

		return Unsafe.As<IReflectedFullInstanceProperty<TOwner, TExpectedProperty>>(self);
	}

	public abstract override Func<TOwner, TProperty>? Getter { get; }

	public abstract override Action<TOwner, TProperty>? Setter { get; }

	private protected ReflectedInstanceProperty(PropertyInfo member) : base(member)
	{
	}
}