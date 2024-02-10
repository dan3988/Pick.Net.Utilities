using System.Numerics;

namespace Pick.Net.Utilities.Threading;

public static partial class AtomicValueExtensions
{
	/// <summary>
	/// Set the value to <paramref name="value"/> if its current value is equal to <paramref name="comparand"/>, and return <c>true</c> if the value changed.
	/// </summary>
	/// <inheritdoc cref="IAtomicValue{T}.Set(T, T)"/>
	public static bool TrySet<TSelf, TValue>(this ref TSelf reference, TValue value, TValue comparand)
		where TSelf : struct, IAtomicValue<TSelf, TValue>
		where TValue : INumber<TValue>
	{
		return reference.Set(value, comparand) == comparand;
	}
}
