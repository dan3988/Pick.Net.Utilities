using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="int"/> value
/// </summary>
public struct AtomicInt32(int value) : IAtomicNumber<AtomicInt32, int>
{
	static AtomicInt32 IAtomicValue<AtomicInt32, int>.Create(int value)
		=> new(value);

	private int _value = value;

	public int Value
	{
		readonly get => _value;
		set => Interlocked.Exchange(ref _value, value);
	}

	public bool Set(int value)
		=> Interlocked.Exchange(ref _value, value) != value;

	public int Increment()
		=> Interlocked.Increment(ref _value);

	public int Decrement()
		=> Interlocked.Decrement(ref _value);

	public readonly override string ToString()
		=> _value.ToString();

	public readonly override int GetHashCode()
		=> _value.GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicInt32 other && Equals(other);

	public readonly bool Equals(AtomicInt32 other)
		=> _value == other._value;

	public readonly bool Equals(int other)
		=> _value == other;

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
