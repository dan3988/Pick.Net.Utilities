using System.Collections.Concurrent;
using System.Reflection;
using Pick.Net.Utilities.Collections;

/* Unmerged change from project 'Pick.Net.Utilities (net7.0)'
Before:
namespace Pick.Net.Utilities.Reflection.Members.Properties.Impl;
After:
namespace Pick.Net.Utilities.Reflection.Members.Properties.Net.Utilities.Reflection.Members.Properties;
using Pick.Net.Utilities.Reflection.Members.Properties.Impl;
*/


namespace Pick.Net.Utilities.Reflection.Members.Properties;

public abstract class ReflectedInstancePropertyCollection : ReflectedPropertyCollection, IReflectedInstancePropertyCollection
{
	internal static readonly ReflectedInstancePropertyCollection<object> Object = new(null, []);
	internal static readonly ReflectedInstancePropertyCollection<ValueType> ValueType = new(Object, []);
	internal static readonly ReflectedInstancePropertyCollection<Enum> Enum = new(ValueType, []);

	private static readonly ConcurrentDictionary<Type, ReflectedInstancePropertyCollection> Collections = new()
	{
		[typeof(object)] = Object,
		[typeof(ValueType)] = ValueType,
		[typeof(Enum)] = Enum
	};

	internal static ReflectedInstancePropertyCollection<T> ForType<T>()
	{
		return (ReflectedInstancePropertyCollection<T>)Collections.GetOrAdd(typeof(T), Create);
	}

	private static ReflectedInstancePropertyCollection ForType(Type type)
	{
		return Collections.GetOrAdd(type, Create);
	}

	private static ReflectedInstancePropertyCollection Create(Type type)
	{
		var method = typeof(ReflectedInstancePropertyCollection).GetMethod(nameof(CreateImpl), ReflectionHelper.DeclaredNonPublicStatic)!.MakeGenericMethod(type);
		return (ReflectedInstancePropertyCollection)method.Invoke(null, BindingFlags.DoNotWrapExceptions, null, null, null)!;
	}

	private static ReflectedInstancePropertyCollection<T> CreateImpl<T>()
	{
		var type = typeof(T);
		var baseProperties = ForType(type.BaseType!);
		var props = type.GetProperties(ReflectionHelper.DeclaredInstance);
		var values = new StringDictionary<ReflectedInstanceProperty<T>>(props.Length);

		foreach (var prop in props)
		{
			if (prop.Name == "Item" && prop.GetIndexParameters().Length != 0)
				continue;

			var reflected = ReflectedInstanceProperty.Create<T>(prop);
			values.Add(prop.Name, reflected);
		}

		return new(baseProperties, values);
	}

	internal sealed override string DebugName => nameof(ReflectedInstancePropertyCollection);

	public IReflectedInstancePropertyCollection? BaseProperties { get; }

	private protected ReflectedInstancePropertyCollection(IReflectedInstancePropertyCollection? baseProperties)
	{
		BaseProperties = baseProperties;
	}

	public abstract override IReflectedInstanceProperty GetProperty(ReadOnlySpan<char> key);

	public abstract override IReflectedInstanceProperty? TryGetProperty(ReadOnlySpan<char> key);
}

public sealed class ReflectedInstancePropertyCollection<T> : ReflectedInstancePropertyCollection, IInternalReflectedInstancePropertyCollection<T>
{
	public override int Count => _values.Count;

	public override Type DeclaringType => typeof(T);

	private readonly StringDictionary<ReflectedInstanceProperty<T>> _values;

	internal ReflectedInstancePropertyCollection(IReflectedInstancePropertyCollection? baseProperties, StringDictionary<ReflectedInstanceProperty<T>> values) : base(baseProperties)
	{
		_values = values;
	}

	public override IReflectedInstanceProperty<T> GetProperty(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key) ?? throw new ArgumentException($"Instance property '{DeclaringType.FullName}.{key}' not found.");

	private IInternalReflectedInstanceProperty<T> GetPropertyInternal(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key) ?? throw new ArgumentException($"Instance property '{DeclaringType.FullName}.{key}' not found.");

	IInternalReflectedInstanceProperty<T>? IInternalReflectedInstancePropertyCollection<T>.TryGetDeclared(ReadOnlySpan<char> key)
		=> Lookup(key);

	private IInternalReflectedInstanceProperty<T>? Lookup(ReadOnlySpan<char> key)
		=> _values.GetValueOrDefault(key);

	public override IReflectedInstanceProperty<T>? TryGetProperty(ReadOnlySpan<char> key)
		=> TryGetPropertyInternal(key);

	private IInternalReflectedInstanceProperty<T>? TryGetPropertyInternal(ReadOnlySpan<char> key)
	{
		IInternalReflectedInstancePropertyCollection<T>? lookup = this;

		do
		{
			var prop = lookup.TryGetDeclared(key);
			if (prop != null)
				return prop;

			lookup = lookup.BaseProperties as IInternalReflectedInstancePropertyCollection<T>;
		}
		while (lookup != null);

		return null;
	}

	public IReflectedReadableInstanceProperty<T, TProperty> GetReadableProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsReadable<TProperty>();

	public IReflectedWritableInstanceProperty<T, TProperty> GetWritableProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsWritable<TProperty>();

	public IReflectedFullInstanceProperty<T, TProperty> GetFullProperty<TProperty>(ReadOnlySpan<char> key)
		=> GetPropertyInternal(key).AsFull<TProperty>();

	IEnumerator<IReflectedProperty> IEnumerable<IReflectedProperty>.GetEnumerator()
		=> GetEnumerator();

	public override void CopyTo(Array array, int index)
		=> _values.Values.CopyTo(array, index);

	public override IEnumerator<IReflectedInstanceProperty> GetEnumerator()
		=> _values.Values.GetEnumerator();
}
