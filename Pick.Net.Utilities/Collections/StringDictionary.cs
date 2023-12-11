using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Collections;

[DebuggerTypeProxy(typeof(IStringDictionaryDebugView<>))]
[DebuggerDisplay("Count = {Count}")]
public class StringDictionary<T> : IStringDictionary<T>, IDictionary<string, T>, IDictionary
{
	private static readonly Type ValueType = typeof(T);
	private static readonly bool ValueIsNullable = !ValueType.IsValueType || Nullable.GetUnderlyingType(ValueType) != null;

	private const int lowBits = 0x7FFFFFFF;
	private const int defaultCapacity = 4;

	private static string GetKeyAsString(object? key, [CallerArgumentExpression(nameof(key))] string argName = null!)
	{
		ArgumentNullException.ThrowIfNull(key);
		return key is string ? key.ToString()! : throw new ArgumentException($"The given key '{key}' is not a string and cannot be used in this generic collection", argName);
	}

	private static T GetValueAsT(object? value, [CallerArgumentExpression(nameof(value))] string argName = null!)
	{
		if (value == null)
		{
			if (ValueIsNullable)
				return default!;
		}
		else if (value is T v)
		{
			return v;
		}

		throw new ArgumentException($"The given value '{value}' is not of type '{ValueType}' and cannot be used in this generic collection.", argName);
	}

	private readonly StringComparison _comparison;
	/// <summary>
	/// The number of entries in use, including free entries.
	/// </summary>
	private int _size;
	/// <summary>
	/// The number of times this collection has been mutated
	/// </summary>
	private int _version;
	/// <summary>
	/// The negated index of the last free slot, or zero if <see cref="Remove(ReadOnlySpan{char}, out T)"/> has not been called
	/// </summary>
	private int _freeIndex;
	/// <summary>
	/// The number of free slots
	/// </summary>
	private int _freeCount;
	private Entry[] _entries;

	private KeyCollection? _keys;
	private ValueCollection? _values;

	public KeyCollection Keys => _keys ??= new(this);

	public ValueCollection Values => _values ??= new(this);

	public StringComparison Comparison => _comparison;

	public int Count => _size - _freeCount;

	public T this[string key]
	{
		get
		{
			ArgumentNullException.ThrowIfNull(key);
			return this[(ReadOnlySpan<char>)key];
		}

		set
		{
			ArgumentNullException.ThrowIfNull(key);
			ref var entry = ref Insert(key, true);
			entry.Value = value;
			_version++;
		}
	}

	public T this[ReadOnlySpan<char> key]
	{
		get
		{
			ref var entry = ref TryGetEntry(key);
			return !Unsafe.IsNullRef(ref entry) ? entry.Value : throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary");
		}
	}

	#region Explicit Properties

	bool ICollection<KeyValuePair<string, T>>.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool IDictionary.IsFixedSize => false;

	bool IDictionary.IsReadOnly => false;

	ICollection<string> IDictionary<string, T>.Keys => Keys;

	ICollection<T> IDictionary<string, T>.Values => Values;

	IReadOnlyStringCollection IReadOnlyStringDictionary<T>.Keys => Keys;

	IReadOnlyCollection<T> IReadOnlyStringDictionary<T>.Values => Values;

	ICollection IDictionary.Keys => Keys;

	ICollection IDictionary.Values => Values;

	object? IDictionary.this[object key]
	{
		get => key is string str ? this[(ReadOnlySpan<char>)str] : throw new ArgumentException($"The given key '{key}' is not a string and cannot be used in this generic collection");
		set => this[GetKeyAsString(key)] = GetValueAsT(value);
	}

	#endregion Explicit Properties

	public StringDictionary() : this(defaultCapacity, StringComparison.Ordinal)
	{
	}

	public StringDictionary(int initialCapacity) : this(initialCapacity, StringComparison.Ordinal)
	{
	}

	public StringDictionary(StringComparison comparison) : this(defaultCapacity, comparison)
	{
	}

	public StringDictionary(int initialCapacity, StringComparison comparison)
	{
		var size = HashHelpers.GetPrime(initialCapacity);
		_entries = new Entry[size];
		_comparison = comparison;
	}

	public StringDictionary(IReadOnlyStringDictionary<T> values) : this(values, values.Comparison)
	{
	}

	public StringDictionary(IEnumerable<KeyValuePair<string, T>> values) : this(values, StringComparison.Ordinal)
	{
	}

	public StringDictionary(IEnumerable<KeyValuePair<string, T>> values, StringComparison comparison)
	{
		var size = values switch
		{
			StringDictionary<T> str => str._entries.Length,
			IEnumerable<KeyValuePair<string, T>> v when v.TryGetNonEnumeratedCount(out int count) => HashHelpers.GetPrime(count),
			_ => HashHelpers.MinPrime
		};

		_entries = new Entry[size];
		_comparison = comparison;

		var index = 0;

		foreach (var (key, value) in values)
		{
			ref var entry = ref Unsafe.NullRef<Entry>();

			var hash = lowBits & key.GetHashCode(comparison);
			var bucket = hash % _entries.Length;
			var i = ~_entries[bucket].Bucket;
			while (i >= 0)
			{
				entry = ref _entries[i];
				if (hash == entry.HashCode && string.Equals(key, entry.Key, _comparison))
					throw new ArgumentException("An item with the same key has already been added. Key: " + key);

				i = ~entry.Next;
			}

			ref var buck = ref _entries[bucket];
			entry = ref _entries[index];
			entry.Key = key;
			entry.HashCode = hash;
			entry.Value = value;
			entry.Next = buck.Bucket;
			buck.Bucket = ~index;
			index++;
		}

		_size = index;
	}

	private bool EnsureCapacity(int min)
	{
		var cap = _entries.Length;
		if (cap > min)
			return false;

		cap = cap == 0 ? defaultCapacity : cap << 1;
		if (cap < min)
			cap = min;

		var entries = _entries;

		Array.Resize(ref entries, cap);

		for (int i = 0; i < _size; i++)
		{
			ref var entry = ref entries[i];
			ref var bucket = ref entries[entry.HashCode % cap];
			entry.Next = bucket.Bucket;
			bucket.Bucket = ~i;
		}

		_entries = entries;
		return true;
	}

	private ref Entry TryGetEntry(ReadOnlySpan<char> key)
	{
		if (_size > 0)
		{
			var hash = lowBits & string.GetHashCode(key, _comparison);
			var i = ~_entries[hash % _entries.Length].Bucket;

			while (i >= 0)
			{
				ref var entry = ref _entries[i];

				if (hash == entry.HashCode && key.Equals(entry.Key, _comparison))
					return ref entry;

				i = ~entry.Next;
			}
		}

		return ref Unsafe.NullRef<Entry>();
	}

	private ref Entry Insert(string key, bool overwrite)
	{
		ArgumentNullException.ThrowIfNull(key);

		ref var entry = ref Unsafe.NullRef<Entry>();

		var hash = lowBits & key.GetHashCode(_comparison);
		var bucket = 0;
		if (_size > 0)
		{
			bucket = hash % _entries.Length;
			var i = ~_entries[bucket].Bucket;
			while (i >= 0)
			{
				entry = ref _entries[i];
				if (hash == entry.HashCode && string.Equals(key, entry.Key, _comparison))
					return ref (overwrite ? ref entry : ref Unsafe.NullRef<Entry>());

				i = ~entry.Next;
			}
		}

		int index;

		if (_freeCount > 0)
		{
			if (_size == 0)
				bucket = hash % _entries.Length;

			index = ~_freeIndex;
			_freeIndex = _entries[index].Next;
			_freeCount--;
		}
		else
		{
			index = _size;

			if (EnsureCapacity(++_size) || index == 0)
				bucket = hash % _entries.Length;
		}

		ref var buck = ref _entries[bucket];
		entry = ref _entries[index];
		entry.Key = key;
		entry.HashCode = hash;
		entry.Next = buck.Bucket;
		buck.Bucket = ~index;
		return ref entry;
	}

	public bool ContainsKey(string key)
		=> ContainsKey((ReadOnlySpan<char>)key);

	public bool ContainsKey(ReadOnlySpan<char> key)
		=> !Unsafe.IsNullRef(ref TryGetEntry(key));

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value)
		=> TryGetValue((ReadOnlySpan<char>)key, out value);

	public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value)
	{
		ref var entry = ref TryGetEntry(key);
		if (Unsafe.IsNullRef(ref entry))
		{
			value = default;
			return false;
		}
		else
		{
			value = entry.Value;
			return true;
		}
	}

	public void Clear()
	{
		if (_size > 0)
		{
			_entries = new Entry[HashHelpers.MinPrime];
			_size = 0;
			_freeCount = 0;
			_freeIndex = 0;
			_version++;
		}
	}

	public void Add(string key, T value)
	{
		ref var entry = ref Insert(key, false);
		if (Unsafe.IsNullRef(ref entry))
			throw new ArgumentException("An item with the same key has already been added. Key: " + key);

		entry.Value = value;
		_version++;
	}

	public bool TryAdd(string key, T value)
		=> TryAdd(key, value, out _);

	public bool TryAdd(string key, T value, [MaybeNullWhen(true)] out T existing)
	{
		ref var entry = ref Insert(key, false);
		if (Unsafe.IsNullRef(ref entry))
		{
			existing = default;
			return true;
		}
		else
		{
			existing = entry.Value;
			_version++;
			return false;
		}
	}

	public bool Remove(string key)
	{
		var value = default(T);
		return Remove(key, false, ref value);
	}

	public bool Remove(string key, [MaybeNullWhen(false)] out T value)
	{
		value = default;
		return Remove(key, false, ref value);
	}

	public bool Remove(ReadOnlySpan<char> key)
	{
		var value = default(T);
		return Remove(key, false, ref value);
	}

	public bool Remove(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value)
	{
		value = default;
		return Remove(key, false, ref value);
	}

	private bool Remove(ReadOnlySpan<char> key, bool checkValue, [NotNullWhen(true)] ref T? value)
	{
		if (_size > 0)
		{
			var hash = lowBits & string.GetHashCode(key, _comparison);
			var bucket = hash % _entries.Length;
			var i = ~_entries[bucket].Bucket;
			var last = 0;

			while (i >= 0)
			{
				ref var entry = ref _entries[i];

				if (hash == entry.HashCode && key.Equals(entry.Key, _comparison))
				{
					if (checkValue && !EqualityComparer<T>.Default.Equals(entry.Value, value))
						return false;

					value = entry.Value!;
					entry.HashCode = -1;
					entry.Key = null!;
					entry.Value = default;
					entry.Next = _freeIndex;

					if (last == 0)
						_entries[bucket].Bucket = entry.Next;
					else
						_entries[last].Next = entry.Next;

					_freeIndex = ~i;
					_freeCount++;
					_version++;
					return true;
				}

				last = i;
				i = ~entry.Next;
			}
		}

		value = default;
		return false;
	}

	public void CopyTo(Array array, int index)
	{
		CheckCopyToArgs(array, index, out int count);

		if (array is KeyValuePair<string, T>[] pairArray)
		{
			CopyToImpl(pairArray, index, count);
		}
		else if (array is DictionaryEntry[] entryArray)
		{
			CopyToImpl(entryArray, index, count);
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				ref var entry = ref _entries[i];
				if (entry.HashCode > 0)
					array.SetValue(entry.Pair, index++);
			}
		}
	}

	public void CopyTo(KeyValuePair<string, T>[] array, int index)
	{
		CheckCopyToArgs(array, index, out int count);
		CopyToImpl(array, index, count);
	}

	private void CheckCopyToArgs(Array array, int index, out int count)
	{
		ArgumentNullException.ThrowIfNull(array);

		if (unchecked((uint)index > (uint)array.Length))
			throw new ArgumentOutOfRangeException(nameof(index), "Index must be a non-negative number smaller than the size of the array");

		if ((count = Count) > (index + array.Length))
			throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(index));
	}

	private void CopyToImpl(DictionaryEntry[] array, int index, int count)
	{
		for (int i = 0; i < count; i++)
		{
			ref var entry = ref _entries[i];
			if (entry.HashCode > 0)
				array[index++] = entry.DictionaryEntry;
		}
	}

	private void CopyToImpl(KeyValuePair<string, T>[] array, int index, int count)
	{
		for (int i = 0; i < count; i++)
		{
			ref var entry = ref _entries[i];
			if (entry.HashCode > 0)
				array[index++] = entry.Pair;
		}
	}

	public Enumerator GetEnumerator()
		=> new(this);

	#region Explicit Methods

	void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
		=> Add(item.Key, item.Value);

	bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
	{
		ref var entry = ref TryGetEntry(item.Key);
		return !Unsafe.IsNullRef(ref entry) && EqualityComparer<T>.Default.Equals(entry.Value, item.Value);
	}

	bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
	{
		var value = item.Value;
		return Remove(item.Key, true, ref value);
	}

	IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	void IDictionary.Add(object key, object? value)
		=> Add(GetKeyAsString(key), GetValueAsT(value));

	bool IDictionary.Contains(object key)
		=> key is string str && ContainsKey((ReadOnlySpan<char>)str);

	IDictionaryEnumerator IDictionary.GetEnumerator()
		=> GetEnumerator();

	void IDictionary.Remove(object key)
		=> Remove(GetKeyAsString(key));

	#endregion Explicit Methods

	[DebuggerDisplay("Key = {key}, Value = {value}")]
	private struct Entry
	{
		internal string Key;
		/// <summary>
		/// The lower 31 bits of the key's hash code, or -1 if this entry is a free slot.
		/// </summary>
		internal int HashCode;
		/// <summary>
		/// Stores the negated index of the entry for the bucket at the index of this entry.
		/// </summary>
		internal int Bucket;
		/// <summary>
		/// Stores the negated index of the next entry if there is a hash code collision.
		/// </summary>
		internal int Next;

		[AllowNull]
		internal T Value;

		internal readonly KeyValuePair<string, T> Pair => new(Key, Value);

		internal readonly DictionaryEntry DictionaryEntry => new(Key, Value);
	}

	public struct Enumerator(StringDictionary<T> owner) : IDictionaryEnumerator<string, T>
	{
		private readonly StringDictionary<T> _owner = owner;
		private int _version = owner._version;
		private int _index;
		private KeyValuePair<string, T> _current;

		public readonly KeyValuePair<string, T> Current => _current;

		private readonly void CheckVersion()
		{
			ObjectDisposedException.ThrowIf(_version < 0, typeof(Enumerator));
			if (_version != _owner._version)
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
		}

		public void Dispose()
		{
			_version = -1;
		}

		public bool MoveNext()
		{
			CheckVersion();

			while ((uint)_index < (uint)_owner._size)
			{
				ref var entry = ref _owner._entries[_index++];
				if (entry.HashCode > 0)
				{
					_current = entry.Pair;
					return true;
				}
			}

			_current = default;
			return false;
		}

		public void Reset()
		{
			CheckVersion();
			_index = 0;
			_current = default;
		}
	}

	private abstract class BaseEnumerator<TItem>(StringDictionary<T> owner) : IEnumerator<TItem>
	{
		private readonly StringDictionary<T> _owner = owner;
		private int _version = owner._version;
		private int _index;
		private TItem? _current;

		object? IEnumerator.Current => _current;

		public TItem Current => _current!;

		private void CheckVersion()
		{
			ObjectDisposedException.ThrowIf(_version < 0, this);
			if (_version != _owner._version)
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
		}

		public void Dispose()
		{
			_version = -1;
		}

		public bool MoveNext()
		{
			CheckVersion();

			while ((uint)_index < (uint)_owner._size)
			{
				ref var entry = ref _owner._entries[_index++];
				if (entry.HashCode > 0)
				{
					_current = GetValue(ref entry);
					return true;
				}
			}

			_current = default;
			return false;
		}

		protected abstract TItem GetValue(ref Entry entry);

		public void Reset()
		{
			CheckVersion();
			_index = 0;
			_current = default;
		}
	}

	private sealed class KeyEnumerator(StringDictionary<T> owner) : BaseEnumerator<string>(owner)
	{
		protected override string GetValue(ref StringDictionary<T>.Entry entry)
			=> entry.Key;
	}

	public sealed class KeyCollection(StringDictionary<T> owner) : AbstractReadOnlyCollection<string>, IReadOnlyStringCollection
	{
		public override int Count => owner.Count;

		public override bool Contains(string item)
			=> owner.ContainsKey((ReadOnlySpan<char>)item);

		public bool Contains(ReadOnlySpan<char> value)
			=> owner.ContainsKey(value);

		public override IEnumerator<string> GetEnumerator()
			=> new KeyEnumerator(owner);

	}

	private sealed class ValueEnumerator(StringDictionary<T> owner) : BaseEnumerator<T>(owner)
	{
		protected override T GetValue(ref StringDictionary<T>.Entry entry)
			=> entry.Value;
	}

	public sealed class ValueCollection(StringDictionary<T> owner) : AbstractReadOnlyCollection<T>
	{
		public override int Count => owner.Count;

		public override IEnumerator<T> GetEnumerator()
			=> new ValueEnumerator(owner);
	}
}
