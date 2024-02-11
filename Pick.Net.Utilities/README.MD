# Pick.Net.Utilites
Various helper classes for C#.

### Abstract collection classes
Abstract classes is designed to serve as a base class for creating read-only collections. Methods that modify the collection (i.e. `Add`, `Clear`, and `Remove`) will throw a `NotSupportedException`.

- [AbstractReadOnlyCollection](Collections/AbstractReadOnlyCollection.cs) (`ICollection`, `ICollection<T>` & `IReadOnlyCollection&lt<T>`)
- [AbstractReadOnlyList](Collections/AbstractReadOnlyList.cs) (`IList`, `IList<T>` & `IReadOnlyList<T>`)
- [AbstractReadOnlyDictionary](Collections/AbstractReadOnlyDictionary.cs) (`IDictionary`, `IDictionary<TKey, TValue>` & `IReadOnlyDictionary<TKey, TValue>`)
```csharp
public class SingleItemCollection(string item) : AbstractReadOnlyCollection<string>
{
	public override int Count => 1;

	public override IEnumerator<string> GetEnumerator()
	{
		yield return item;
	}
}

var collection = new SimpleCollection("test");
var array = collection.ToArray();
// [ "test" ]
((ICollection<T>)collection).Add("another item")
// throws a NotSupportedException
```

### Map
A dictionary-like collection with that allows covariance of the value type parameter and returns null instead of throwing a KeyNotFound exception when accessing a key not in the collection. Only supports reference type values.
```csharp
var map = new Map<string, string>
{
	{ "key1", "value1" },
	{ "key2", "value2" },
	{ "key3", "value3" }
};

var value1 = map["key1"];
// "value1"
var value4 = map["key4"];
// null

IReadOnlyMap<string, object> objectMap = map;
var value2 = objectMap["key2"];
// "value2"
```

### StringDictionary
A string-keyed dictionary that allows char spans to be used to get values
```csharp
var map = new StringDictionary<object>
{
	{ "key1", "value1" },
	{ "key2", "value2" },
	{ "key3", "value3" }
};

ReadOnlySpan<char> keys = "key1;key2;key3";

var value1 = map[keys[..4]];
// "value1"
var value2 = map[keys[5..^5]];
// "value2"
var value3 = map[keys[^4..]];
// "value3"
```

### DelegateHelper
Shortcuts for MethodInfo.CreateDelegate for `System.Action` and `System.Func` delegates.
```csharp
Action<object> handler = method.CreateAction<object>();
Func<int, string, bool> handler = method.CreateFunc<int, string, bool>();
```

Get delegate parameter and return types
```csharp
ImmutableArray<Type> types = DelegateHelper.GetParameterTypes<Action<string, int>>();
// [ typeof(string), typeof(int) ]
Type types = DelegateHelper.GetReturnType<Func<string>>();
// typeof(string)
```

### Atomic Values
Value type wrappers for `int`, `uint`, `long`, `ulong`, `float` and `double` that use the `System.Threading.Interlocked` class to perform atomic operations. e.g.
```csharp
private int value;

void DoSomething()
{
	var next = Interlocked.Increment(ref value);
	var old = Interlocked.Exchange(ref value, 5);
	if (Interlocked.CompareExchange(ref value, 0, 5) == 5)
	{
		...
	}
}
```
Can be simplified to
```csharp
private AtomicInt32 value;

void DoSomething()
{
	var next = value.Increment();
	var old = value.Set(5);
	if (value.TrySet(0, 5))
	{
		...
	}
}
```

- [AtomicInt32](Threading/AtomicNumber.Designer.cs#L11)
- [AtomicUInt32](Threading/AtomicNumber.Designer.cs#L77)
- [AtomicInt64](Threading/AtomicNumber.Designer.cs#L143)
- [AtomicUInt64](Threading/AtomicNumber.Designer.cs#L209)
- [AtomicSingle](Threading/AtomicNumber.Designer.cs#L275)
- [AtomicDouble](Threading/AtomicNumber.Designer.cs#L352)