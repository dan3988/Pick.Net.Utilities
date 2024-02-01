using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Threading;

/// <summary>
/// Thread safe <see cref="bool"/> value
/// </summary>
public struct AtomicBool(bool initialValue) : IAtomicValue<AtomicBool, bool>
{
	static AtomicBool IAtomicValue<AtomicBool, bool>.Create(bool value)
		=> new(value);

	private uint _value = Convert.ToUInt32(initialValue);

	public bool Value
	{
		readonly get => AtomicHelper.Read(in _value) != 0;
		set => Interlocked.Exchange(ref _value, Convert.ToUInt32(value));
	}

	public bool Set(bool value)
	{
		var (x, y) = value ? (1u, 0u) : (0u, 1u);
		return Interlocked.CompareExchange(ref _value, x, y) == y;
	}

	public readonly override string ToString()
		=> Value.ToString();

	public readonly override int GetHashCode()
		=> Value.GetHashCode();

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is AtomicBool other && Equals(other);

	public readonly bool Equals(AtomicBool other)
		=> Value == other.Value;

	public readonly bool Equals(bool other)
		=> Value == other;

	public static bool operator ==(AtomicBool left, AtomicBool right)
		=> left.Equals(right);

	public static bool operator !=(AtomicBool left, AtomicBool right)
		=> !left.Equals(right);

	public static bool operator ==(AtomicBool left, bool right)
		=> left.Equals(right);

	public static bool operator !=(AtomicBool left, bool right)
		=> !left.Equals(right);

	public static bool operator ==(bool left, AtomicBool right)
		=> right.Equals(left);

	public static bool operator !=(bool left, AtomicBool right)
		=> !right.Equals(left);
}
