using System.ComponentModel;
using Pick.Net.Utilities.Reflection.Members.Properties;

namespace Pick.Net.Utilities.Reflection;

public abstract class TypeMembers
{
	internal static readonly TypeMembers Object = new TypeMembers<object>(ReflectedStaticPropertyCollection.Object, ReflectedInstancePropertyCollection.Object);
	internal static readonly TypeMembers ValueType = new TypeMembers<ValueType>(ReflectedStaticPropertyCollection.ValueType, ReflectedInstancePropertyCollection.ValueType);
	internal static readonly TypeMembers Enum = new TypeMembers<Enum>(ReflectedStaticPropertyCollection.Enum, ReflectedInstancePropertyCollection.Enum);

	public abstract ReflectedInstancePropertyCollection Properties { get; }

	private ReflectedStaticPropertyCollection? _staticProperties;
	public ReflectedStaticPropertyCollection StaticProperties => _staticProperties ??= ReflectedStaticPropertyCollection.ForType(DeclaringType);

	public abstract Type DeclaringType { get; }

	internal TypeMembers(ReflectedStaticPropertyCollection? staticProperties)
	{
		_staticProperties = staticProperties;
	}
}

public sealed class TypeMembers<T> : TypeMembers
{
	private ReflectedInstancePropertyCollection<T>? _properties;
	public override ReflectedInstancePropertyCollection<T> Properties => _properties ??= ReflectedInstancePropertyCollection.ForType<T>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Type DeclaringType => typeof(T);

	internal TypeMembers() : base(null)
	{
	}

	internal TypeMembers(ReflectedStaticPropertyCollection? staticProperties, ReflectedInstancePropertyCollection<T> properties) : base(staticProperties)
	{
		_properties = properties;
	}
}
