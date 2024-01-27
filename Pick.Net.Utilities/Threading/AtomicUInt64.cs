using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="ulong"/> value
/// </summary>
public struct AtomicUInt64(ulong value) : IAtomicNumber<AtomicUInt64, ulong>
{
	static AtomicUInt64 IAtomicValue<AtomicUInt64, ulong>.Create(ulong value)
		=> new(value);

	private ulong _value = value;

	public ulong Value
	{
		readonly get => _value;
		set => Interlocked.Exchange(ref _value, value);
	}

	public bool Set(ulong value)
		=> Interlocked.Exchange(ref _value, value) != value;

	public ulong Increment()
		=> Interlocked.Increment(ref _value);

	public ulong Decrement()
		=> Interlocked.Decrement(ref _value);

	public readonly override string ToString()
		=> _value.ToString();

	public readonly override int GetHashCode()
		=> _value.GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicUInt64 other && Equals(other);

	public readonly bool Equals(AtomicUInt64 other)
		=> _value == other._value;

	public readonly bool Equals(ulong other)
		=> _value == other;

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