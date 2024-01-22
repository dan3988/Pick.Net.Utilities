using System.Collections.Concurrent;

using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Reflection.Members.Properties;

public sealed class ReflectedStaticPropertyCollection : ReflectedPropertyCollection, IReflectedStaticPropertyCollection
{
	private static readonly StringDictionary<ReflectedStaticProperty> EmptyProperties = [];

	internal static readonly ReflectedStaticPropertyCollection Object = new(typeof(object), null, EmptyProperties);
	internal static readonly ReflectedStaticPropertyCollection ValueType = new(typeof(ValueType), Object, EmptyProperties);
	internal static readonly ReflectedStaticPropertyCollection Enum = new(typeof(Enum), ValueType, EmptyProperties);

	private static readonly ConcurrentDictionary<Type, ReflectedStaticPropertyCollection> Collections = new()
	{
		[typeof(object)] = Object,
		[typeof(ValueType)] = ValueType,
		[typeof(Enum)] = Enum
	};

	internal static ReflectedStaticPropertyCollection ForType(Type type)
		=> Collections.GetOrAdd(type, Create);

	private static ReflectedStaticPropertyCollection Create(Type type)
	{
		var baseProperties = ForType(type.BaseType!);
		var props = type.GetProperties(ReflectionHelper.DeclaredStatic);
		if (props.Length == 0)
			return new(type, baseProperties, EmptyProperties);

		var values = new StringDictionary<ReflectedStaticProperty>(props.Length);

		foreach (var prop in props)
		{
			var reflected = ReflectedStaticProperty.Create(prop);
			values.Add(prop.Name, reflected);
		}

		return new(type, baseProperties, values);
	}

	internal override string DebugName => nameof(ReflectedStaticPropertyCollection);

	public override int Count => _values.Count;

	public override Type DeclaringType { get; }

	public ReflectedStaticPropertyCollection? BaseProperties { get; }

	IReflectedStaticPropertyCollection? IReflectedStaticPropertyCollection.BaseProperties => BaseProperties;

	private readonly StringDictionary<ReflectedStaticProperty> _values;

	private ReflectedStaticPropertyCollection(Type declaringType, ReflectedStaticPropertyCollection? baseProperties, StringDictionary<ReflectedStaticProperty> values)
	{
		DeclaringType = declaringType;
		BaseProperties = baseProperties;
		_values = values;
	}

	public override IReflectedStaticProperty GetProperty(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key) ?? throw new ArgumentException($"Static property '{DeclaringType.FullName}.{key}' not found.");

	private ReflectedStaticProperty GetPropertyInternal(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key) ?? throw new ArgumentException($"Static property '{DeclaringType.FullName}.{key}' not found.");

	private ReflectedStaticProperty? Lookup(ReadOnlySpan<char> key)
		=> _values.GetValueOrDefault(key);

	public override IReflectedStaticProperty? TryGetProperty(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key);

	private ReflectedStaticProperty? TryGetPropertyInternal(ReadOnlySpan<char> key)
	{
		var lookup = this;

		do
		{
			var prop = lookup.Lookup(key);
			if (prop != null)
				return prop;

			lookup = lookup.BaseProperties;
		}
		while (lookup != null);

		return null;
	}

	public IReflectedReadableStaticProperty<TProperty> GetReadableProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsReadable<TProperty>();

	public IReflectedWritableStaticProperty<TProperty> GetWritableProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsWritable<TProperty>();

	public IReflectedFullStaticProperty<TProperty> GetFullProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsFull<TProperty>();

	IEnumerator<IReflectedProperty> IEnumerable<IReflectedProperty>.GetEnumerator()
		=> GetEnumerator();

	public override void CopyTo(Array array, int index)
		=> _values.Values.CopyTo(array, index);

	public override IEnumerator<IReflectedStaticProperty> GetEnumerator()
		=> _values.Values.GetEnumerator();
}