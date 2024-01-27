using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="long"/> value
/// </summary>
public struct AtomicInt64(long value) : IAtomicNumber<AtomicInt64, long>
{
	static AtomicInt64 IAtomicValue<AtomicInt64, long>.Create(long value)
		=> new(value);

	private long _value = value;

	public long Value
	{
		readonly get => _value;
		set => Interlocked.Exchange(ref _value, value);
	}

	public bool Set(long value)
		=> Interlocked.Exchange(ref _value, value) != value;

	public long Increment()
		=> Interlocked.Increment(ref _value);

	public long Decrement()
		=> Interlocked.Decrement(ref _value);

	public readonly override string ToString()
		=> _value.ToString();

	public readonly override int GetHashCode()
		=> _value.GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicInt64 other && Equals(other);

	public readonly bool Equals(AtomicInt64 other)
		=> _value == other._value;

	public readonly bool Equals(long other)
		=> _value == other;

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
