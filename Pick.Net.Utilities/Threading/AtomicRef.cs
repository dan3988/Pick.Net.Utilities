using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Threading;

public struct AtomicRef<T>(T value) : IAtomicValue<T>, IEquatable<AtomicRef<T>>, IEqualityOperators<AtomicRef<T>, AtomicRef<T>, bool>, IAtomicRef
	where T : class?
{
	private T _value = value;

	readonly object? IAtomicRef.Value => _value;

	public T Value
	{
		readonly get => _value;
		set => Interlocked.Exchange(ref _value, value);
	}

	public T Set(T value)
		=> Interlocked.Exchange(ref _value, value);

	public T Set(T value, T comparand)
		=> Interlocked.CompareExchange(ref _value!, value, comparand);

	public readonly override int GetHashCode()
		=> RuntimeHelpers.GetHashCode(_value);

	public readonly override bool Equals([NotNullWhen(true)] object? value)
		=> value is IAtomicRef r && ReferenceEquals(_value, r.Value);

	public readonly bool Equals(AtomicRef<T> other)
		=> ReferenceEquals(_value, other._value);

	public static bool operator ==(AtomicRef<T> left, AtomicRef<T> right)
		=> left.Equals(right);

	public static bool operator !=(AtomicRef<T> left, AtomicRef<T> right)
		=> !left.Equals(right);
}