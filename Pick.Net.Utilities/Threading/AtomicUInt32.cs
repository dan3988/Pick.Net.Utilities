using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="uint"/> value
/// </summary>
public struct AtomicUInt32(uint value) : IAtomicNumber<AtomicUInt32, uint>
{
	static AtomicUInt32 IAtomicValue<AtomicUInt32, uint>.Create(uint value)
		=> new(value);

	private uint _value = value;

	public uint Value
	{
		readonly get => _value;
		set => Interlocked.Exchange(ref _value, value);
	}

	public bool Set(uint value)
		=> Interlocked.Exchange(ref _value, value) != value;

	public uint Increment()
		=> Interlocked.Increment(ref _value);

	public uint Decrement()
		=> Interlocked.Decrement(ref _value);

	public readonly override string ToString()
		=> _value.ToString();

	public readonly override int GetHashCode()
		=> _value.GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicUInt32 other && Equals(other);

	public readonly bool Equals(AtomicUInt32 other)
		=> _value == other._value;

	public readonly bool Equals(uint other)
		=> _value == other;

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