#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="int"/> value
/// </summary>
[DebuggerDisplay(AtomicHelper.DebuggerDisplay)]
public partial struct AtomicInt32(int value) : IAtomicNumber<AtomicInt32, int>
{
	static AtomicInt32 IAtomicValue<AtomicInt32, int>.Create(int value)
		=> new(value);

	private int _value = value;

	public int Value
	{
		readonly get => AtomicHelper.Read(in _value);
		set => Interlocked.Exchange(ref _value, value);
	}

	public int Set(int value)
		=> Interlocked.Exchange(ref _value, value);

	public int Set(int value, int comparand)
		=> Interlocked.CompareExchange(ref _value, value, comparand);

	public int Increment()
		=> Interlocked.Increment(ref _value);

	public int Decrement()
		=> Interlocked.Decrement(ref _value);

	public int Add(int amount)
		=> Interlocked.Add(ref _value, amount);

	public readonly override string ToString()
		=> AtomicHelper.Read(in _value).ToString();

	public readonly override int GetHashCode()
		=> AtomicHelper.Read(in _value).GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicInt32 other && Equals(other);

	public readonly bool Equals(AtomicInt32 other)
		=> AtomicHelper.Read(in _value) == AtomicHelper.Read(in other._value);

	public readonly bool Equals(int other)
		=> AtomicHelper.Read(in _value) == other;

	public static bool operator ==(AtomicInt32 left, AtomicInt32 right)
		=> left.Equals(right);

	public static bool operator !=(AtomicInt32 left, AtomicInt32 right)
		=> !left.Equals(right);

	public static bool operator ==(AtomicInt32 left, int right)
		=> left.Equals(right);

	public static bool operator !=(AtomicInt32 left, int right)
		=> !left.Equals(right);

	public static bool operator ==(int left, AtomicInt32 right)
		=> right.Equals(left);

	public static bool operator !=(int left, AtomicInt32 right)
		=> !right.Equals(left);
}

/// <summary>
/// Thread safe <see cref="uint"/> value
/// </summary>
[DebuggerDisplay(AtomicHelper.DebuggerDisplay)]
public partial struct AtomicUInt32(uint value) : IAtomicNumber<AtomicUInt32, uint>
{
	static AtomicUInt32 IAtomicValue<AtomicUInt32, uint>.Create(uint value)
		=> new(value);

	private uint _value = value;

	public uint Value
	{
		readonly get => AtomicHelper.Read(in _value);
		set => Interlocked.Exchange(ref _value, value);
	}

	public uint Set(uint value)
		=> Interlocked.Exchange(ref _value, value);

	public uint Set(uint value, uint comparand)
		=> Interlocked.CompareExchange(ref _value, value, comparand);

	public uint Increment()
		=> Interlocked.Increment(ref _value);

	public uint Decrement()
		=> Interlocked.Decrement(ref _value);

	public uint Add(uint amount)
		=> Interlocked.Add(ref _value, amount);

	public readonly override string ToString()
		=> AtomicHelper.Read(in _value).ToString();

	public readonly override int GetHashCode()
		=> AtomicHelper.Read(in _value).GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicUInt32 other && Equals(other);

	public readonly bool Equals(AtomicUInt32 other)
		=> AtomicHelper.Read(in _value) == AtomicHelper.Read(in other._value);

	public readonly bool Equals(uint other)
		=> AtomicHelper.Read(in _value) == other;

	public static bool operator ==(AtomicUInt32 left, AtomicUInt32 right)
		=> left.Equals(right);

	public static bool operator !=(AtomicUInt32 left, AtomicUInt32 right)
		=> !left.Equals(right);

	public static bool operator ==(AtomicUInt32 left, uint right)
		=> left.Equals(right);

	public static bool operator !=(AtomicUInt32 left, uint right)
		=> !left.Equals(right);

	public static bool operator ==(uint left, AtomicUInt32 right)
		=> right.Equals(left);

	public static bool operator !=(uint left, AtomicUInt32 right)
		=> !right.Equals(left);
}

/// <summary>
/// Thread safe <see cref="long"/> value
/// </summary>
[DebuggerDisplay(AtomicHelper.DebuggerDisplay)]
public partial struct AtomicInt64(long value) : IAtomicNumber<AtomicInt64, long>
{
	static AtomicInt64 IAtomicValue<AtomicInt64, long>.Create(long value)
		=> new(value);

	private long _value = value;

	public long Value
	{
		readonly get => AtomicHelper.Read(in _value);
		set => Interlocked.Exchange(ref _value, value);
	}

	public long Set(long value)
		=> Interlocked.Exchange(ref _value, value);

	public long Set(long value, long comparand)
		=> Interlocked.CompareExchange(ref _value, value, comparand);

	public long Increment()
		=> Interlocked.Increment(ref _value);

	public long Decrement()
		=> Interlocked.Decrement(ref _value);

	public long Add(long amount)
		=> Interlocked.Add(ref _value, amount);

	public readonly override string ToString()
		=> AtomicHelper.Read(in _value).ToString();

	public readonly override int GetHashCode()
		=> AtomicHelper.Read(in _value).GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicInt64 other && Equals(other);

	public readonly bool Equals(AtomicInt64 other)
		=> AtomicHelper.Read(in _value) == AtomicHelper.Read(in other._value);

	public readonly bool Equals(long other)
		=> AtomicHelper.Read(in _value) == other;

	public static bool operator ==(AtomicInt64 left, AtomicInt64 right)
		=> left.Equals(right);

	public static bool operator !=(AtomicInt64 left, AtomicInt64 right)
		=> !left.Equals(right);

	public static bool operator ==(AtomicInt64 left, long right)
		=> left.Equals(right);

	public static bool operator !=(AtomicInt64 left, long right)
		=> !left.Equals(right);

	public static bool operator ==(long left, AtomicInt64 right)
		=> right.Equals(left);

	public static bool operator !=(long left, AtomicInt64 right)
		=> !right.Equals(left);
}

/// <summary>
/// Thread safe <see cref="ulong"/> value
/// </summary>
[DebuggerDisplay(AtomicHelper.DebuggerDisplay)]
public partial struct AtomicUInt64(ulong value) : IAtomicNumber<AtomicUInt64, ulong>
{
	static AtomicUInt64 IAtomicValue<AtomicUInt64, ulong>.Create(ulong value)
		=> new(value);

	private ulong _value = value;

	public ulong Value
	{
		readonly get => AtomicHelper.Read(in _value);
		set => Interlocked.Exchange(ref _value, value);
	}

	public ulong Set(ulong value)
		=> Interlocked.Exchange(ref _value, value);

	public ulong Set(ulong value, ulong comparand)
		=> Interlocked.CompareExchange(ref _value, value, comparand);

	public ulong Increment()
		=> Interlocked.Increment(ref _value);

	public ulong Decrement()
		=> Interlocked.Decrement(ref _value);

	public ulong Add(ulong amount)
		=> Interlocked.Add(ref _value, amount);

	public readonly override string ToString()
		=> AtomicHelper.Read(in _value).ToString();

	public readonly override int GetHashCode()
		=> AtomicHelper.Read(in _value).GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicUInt64 other && Equals(other);

	public readonly bool Equals(AtomicUInt64 other)
		=> AtomicHelper.Read(in _value) == AtomicHelper.Read(in other._value);

	public readonly bool Equals(ulong other)
		=> AtomicHelper.Read(in _value) == other;

	public static bool operator ==(AtomicUInt64 left, AtomicUInt64 right)
		=> left.Equals(right);

	public static bool operator !=(AtomicUInt64 left, AtomicUInt64 right)
		=> !left.Equals(right);

	public static bool operator ==(AtomicUInt64 left, ulong right)
		=> left.Equals(right);

	public static bool operator !=(AtomicUInt64 left, ulong right)
		=> !left.Equals(right);

	public static bool operator ==(ulong left, AtomicUInt64 right)
		=> right.Equals(left);

	public static bool operator !=(ulong left, AtomicUInt64 right)
		=> !right.Equals(left);
}
