using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pick.Net.Utilities.Collections;

[DebuggerTypeProxy(typeof(IMapDebugView<,>))]
[DebuggerDisplay(CollectionHelper.DebuggerDisplay)]
public sealed class Map<TKey, TValue> : IMap, IMap<TKey, TValue>, ICollection, ICollection<IMapEntry<TKey, TValue>>, ICollection<IReadOnlyMapEntry>
	where TKey : notnull
	where TValue : class
{
	private const int lowBits = 0x7FFFFFFF;

	private readonly IEqualityComparer<TKey> _comparer;

	/// <summary>
	/// The number of entries in use, including free entries.
	/// </summary>
	private int _count;
	/// <summary>
	/// The number of times this collection has been mutated
	/// </summary>
	private int _version;
	private Entry?[] _entries;

	private KeyCollection? keys;
	private ValueCollection? values;

	public int Count => _count;

	public KeyCollection Keys => keys ??= new(this);

	public ValueCollection Values => values ??= new(this);

	public TValue? this[TKey key]
	{
		get => TryGetEntry(key)?.Value;
		set => _ = value == null ? Remove(key) : Insert(key, value, DictionaryInsertBehaviour.Overwrite);
	}

	#region Explicit Property Implementations

	IReadOnlyCollection<TKey> IReadOnlyMap<TKey, TValue>.Keys => Keys;

	IReadOnlyCollection<TValue> IReadOnlyMap<TKey, TValue>.Values => Values;

	bool ICollection<IReadOnlyMapEntry>.IsReadOnly => false;

	bool ICollection<IMapEntry<TKey, TValue>>.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IReadOnlyMap.this[object key] => ((IMap)this)[key];

	object? IMap.this[object key]
	{
		get => key is TKey k ? this[k] : null;
		set
		{
			if (key is not TKey k)
				throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

			if (value is not TValue v)
				throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

			this[k] = v;
		}
	}

	#endregion

	public Map() : this(0, null)
	{
	}

	public Map(int initialCapacity, IEqualityComparer<TKey>? comparer = null)
	{
		var size = initialCapacity <= HashHelpers.MinPrime ? HashHelpers.MinPrime : HashHelpers.GetPrime(initialCapacity);
		_comparer = comparer ?? EqualityComparer<TKey>.Default;
		_entries = new Entry[size];
	}

	public Map(Dictionary<TKey, TValue> values) : this(values, values.Comparer)
	{
	}

	public Map(IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? comparer = null)
	{
		values.TryGetNonEnumeratedCount(out var count);

		var size = HashHelpers.GetPrime(count);
		_comparer = comparer ?? EqualityComparer<TKey>.Default;
		_entries = new Entry[size];

		foreach (var (key, value) in values)
			ConstructorInsert(key, value);
	}

	public Map(IEnumerable<IReadOnlyMapEntry<TKey, TValue>> values, IEqualityComparer<TKey>? comparer = null)
	{
		values.TryGetNonEnumeratedCount(out var count);

		var size = HashHelpers.GetPrime(count);
		_comparer = comparer ?? EqualityComparer<TKey>.Default;
		_entries = new Entry[size];

		foreach (var (key, value) in values)
			ConstructorInsert(key, value);
	}

	private bool EnsureCapacity(int min)
	{
		var cap = _entries.Length;
		if (cap >= min)
			return false;

		cap = HashHelpers.ExpandPrime(cap);
		if (cap < min)
			cap = min;

		var oldEntries = _entries;
		var newEntries = new Entry?[cap];

		for (var i = 0; i < oldEntries.Length; i++)
		{
			Entry? next;

			for (var e = oldEntries[i]; e != null; e = next)
			{
				next = e.Next;

				ref var bucket = ref newEntries[e.HashCode % cap];
				while (bucket != null)
					bucket = ref bucket.Next;

				bucket = e;
				e.Next = null;
			}
		}

		_entries = newEntries;
		return true;
	}

	private void ConstructorInsert(TKey key, TValue value)
	{
		var hash = lowBits & _comparer.GetHashCode(key);

		ref var entry = ref _entries[hash % _entries.Length];

		while (entry != null)
		{
			if (hash == entry.HashCode && _comparer.Equals(key, entry.Key))
				throw CollectionHelper.EnumeratorInvalidatedException();

			entry = ref entry.Next;
		}

		entry = new(key, hash, value);
	}

	private Entry? TryGetEntry(TKey key)
	{
		var hash = lowBits & _comparer.GetHashCode(key);

		ref var entry = ref _entries[hash % _entries.Length];

		while (entry != null)
		{
			if (hash == entry.HashCode && _comparer.Equals(key, entry.Key))
				return entry;

			entry = ref entry.Next;
		}

		return null;
	}

	private bool Insert(TKey key, TValue value, DictionaryInsertBehaviour behaviour)
	{
		var hash = lowBits & _comparer.GetHashCode(key);
		ref var entry = ref _entries[hash % _entries.Length];

		while (entry != null)
		{
			if (hash == entry.HashCode && _comparer.Equals(key, entry.Key))
			{
				if (behaviour == DictionaryInsertBehaviour.Throw)
					throw CollectionHelper.DuplicateAddedException(key);

				if (behaviour == DictionaryInsertBehaviour.Overwrite)
				{
					entry.Value = value;
					goto Added;
				}

				return false;
			}

			entry = ref entry.Next;
		}

		if (!EnsureCapacity(++_count))
		{
			entry = new(key, hash, value);
			goto Added;
		}

		entry = ref _entries[hash % _entries.Length]!;

		while (entry != null)
			entry = ref entry.Next;

		entry = new(key, hash, value);
	Added:
		_version++;
		return true;
	}

	public IMapEntry<TKey, TValue>? GetEntry(TKey key)
		=> TryGetEntry(key);

	public void CopyTo(IReadOnlyMapEntry[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public void CopyTo(IMapEntry<TKey, TValue>[] array, int arrayIndex)
		=> CopyTo((Array)array, arrayIndex);

	public void CopyTo(Array array, int index)
	{
		for (var i = 0; i < _entries.Length; i++)
		{
			var e = _entries[i];
			while (e != null)
			{
				array.SetValue(e, index++);
				e = e.Next;
			}
		}
	}

	public bool ContainsKey(TKey key)
		=> TryGetEntry(key) != null;

	public void Add(IMapEntry<TKey, TValue> item)
		=> Add(item.Key, item.Value);

	public void Add(TKey key, TValue value)
		=> Insert(key, value, DictionaryInsertBehaviour.Throw);

	public bool TryAdd(TKey key, TValue value)
		=> Insert(key, value, DictionaryInsertBehaviour.Return);

	public void Clear()
	{
		for (var i = 0; i < _entries.Length; i++)
		{
			var entry = _entries[i];

			while (entry != null)
			{
				var next = entry.Next;
				entry.Next = null;
				entry = next;
			}
		}

		_count = 0;
		_entries = new Entry?[HashHelpers.MinPrime];
	}

	public bool Remove(TKey key)
	{
		var value = default(TValue);
		return Remove(key, false, ref value);
	}

	public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		value = default;
		return Remove(key, false, ref value);
	}

	public bool Remove(TKey key, bool checkValue, [MaybeNullWhen(false)] ref TValue? value)
	{
		var hash = lowBits & _comparer.GetHashCode(key);

		ref var entry = ref _entries[hash % _entries.Length];

		while (entry != null)
		{
			if (hash == entry.HashCode && _comparer.Equals(key, entry.Key) && (!checkValue || EqualityComparer<TValue>.Default.Equals(value, entry.Value)))
			{
				var old = entry;
				entry = entry.Next;
				old.Next = null;
				value = old.Value;
				_version++;
				_count--;
				return true;
			}

			entry = ref entry.Next;
		}

		return false;
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IReadOnlyMapEntry> IEnumerable<IReadOnlyMapEntry>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IReadOnlyMapEntry<TKey, TValue>> IEnumerable<IReadOnlyMapEntry<TKey, TValue>>.GetEnumerator()
		=> GetEnumerator();

	IEnumerator<IMapEntry<TKey, TValue>> IEnumerable<IMapEntry<TKey, TValue>>.GetEnumerator()
		=> GetEnumerator();

	public Enumerator GetEnumerator()
		=> new(this);

	private bool Contains(TKey key, TValue value, IReadOnlyMapEntry entry)
	{
		var existing = TryGetEntry(key);
		return existing != null && (ReferenceEquals(existing, entry) || EqualityComparer<TValue>.Default.Equals(existing.Value, value));
	}

	#region Explicit Method Implementations

	bool ICollection<IReadOnlyMapEntry>.Contains(IReadOnlyMapEntry item)
		=> item is { Key: TKey key, Value: TValue value } && Contains(key, value, item);

	bool ICollection<IMapEntry<TKey, TValue>>.Contains(IMapEntry<TKey, TValue> item)
		=> Contains(item.Key, item.Value, item);

	bool IMap.Remove(object key)
		=> key is TKey k && Remove(k);

	bool ICollection<IMapEntry<TKey, TValue>>.Remove(IMapEntry<TKey, TValue> item)
	{
		var value = item.Value;
		return Remove(item.Key, true, ref value);
	}

	bool ICollection<IReadOnlyMapEntry>.Remove(IReadOnlyMapEntry item)
		=> item is { Key: TKey key, Value: TValue value } && Contains(key, value, item);

	void ICollection<IReadOnlyMapEntry>.Add(IReadOnlyMapEntry item)
		=> ((IMap)this).Add(item.Key, item.Value);

	void IMap.Add(object key, object? value)
	{
		if (key is not TKey k)
			throw new ArgumentException("Could not cast key to type " + typeof(TKey), nameof(key));

		if (value is not TValue v)
			throw new ArgumentException("Could not cast value to type " + typeof(TValue), nameof(value));

		Add(k, v);
	}

	IReadOnlyMapEntry<TKey, TValue>? IReadOnlyMap<TKey, TValue>.GetEntry(TKey key)
		=> GetEntry(key);

	IReadOnlyMapEntry? IReadOnlyMap.GetEntry(object key)
		=> key is TKey k ? GetEntry(k) : null;

	#endregion

	[DebuggerDisplay("Key = {Key}, Value = {Value}")]
	private sealed class Entry(TKey key, int hashCode, TValue value, Entry? next = null) : IMapEntry<TKey, TValue>
	{
		internal readonly TKey Key = key;
		internal readonly int HashCode = hashCode;
		internal Entry? Next = next;
		[AllowNull]
		internal TValue Value = value;

		object IReadOnlyMapEntry.Key => Key;

		TKey IReadOnlyMapEntry<TKey, TValue>.Key => Key;

		object IReadOnlyMapEntry.Value => Value;

		TValue IReadOnlyMapEntry<TKey, TValue>.Value => Value;

		TValue IMapEntry<TKey, TValue>.Value
		{
			get => Value;
			set => Value = value;
		}
	}

	public struct Enumerator(Map<TKey, TValue> owner) : IEnumerator<IMapEntry<TKey, TValue>>
	{
		private readonly Map<TKey, TValue> _owner = owner;
		private int _version = owner._version;
		private int _index;
		private Entry? _current;

		public readonly IMapEntry<TKey, TValue> Current => _current!;

		readonly object? IEnumerator.Current => _current;

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

			var index = _index;
			var entries = _owner._entries;
			if (entries.Length <= index)
				return false;

			var current = _current == null ? entries[0] : _current.Next;
			while (current == null)
			{
				if (++index >= (uint)entries.Length)
				{
					_index = index;
					_current = null;
					return false;
				}

				current = entries[index];
			}

			_index = index;
			_current = current;
			return true;
		}

		public void Reset()
		{
			CheckVersion();
			_index = 0;
			_current = default;
		}
	}

	private abstract class BaseEnumerator<TItem>(Map<TKey, TValue> owner) : IEnumerator<TItem>
	{
		private readonly Map<TKey, TValue> _owner = owner;
		private int _version = owner._version;
		private int _index;
		private Entry? _current;

		public TItem Current { get; private set; } = default!;

		object? IEnumerator.Current => Current;

		protected abstract TItem GetValue(Entry entry);

		private void CheckVersion()
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

			var index = _index;
			var entries = _owner._entries;
			if (entries.Length <= index)
				return false;

			var current = _current == null ? entries[0] : _current.Next;
			while (current == null)
			{
				if (++index >= (uint)entries.Length)
				{
					_index = index;
					_current = null;
					Current = default!;
					return false;
				}

				current = entries[index];
			}

			_index = index;
			_current = current;
			Current = GetValue(current);
			return true;
		}

		public void Reset()
		{
			CheckVersion();
			_index = 0;
			_current = default;
		}
	}

	private sealed class KeyEnumerator(Map<TKey, TValue> owner) : BaseEnumerator<TKey>(owner)
	{
		protected override TKey GetValue(Entry entry)
			=> entry.Key;
	}

	public sealed class KeyCollection(Map<TKey, TValue> owner) : AbstractReadOnlyCollection<TKey>
	{
		public override int Count => owner.Count;

		public override bool Contains(TKey item)
			=> owner.ContainsKey(item);

		public override IEnumerator<TKey> GetEnumerator()
			=> new KeyEnumerator(owner);
	}

	private sealed class ValueEnumerator(Map<TKey, TValue> owner) : BaseEnumerator<TValue>(owner)
	{
		protected override TValue GetValue(Entry entry)
			=> entry.Value;
	}

	public sealed class ValueCollection(Map<TKey, TValue> owner) : AbstractReadOnlyCollection<TValue>
	{
		public override int Count => owner.Count;

		public override IEnumerator<TValue> GetEnumerator()
			=> new ValueEnumerator(owner);
	}
}