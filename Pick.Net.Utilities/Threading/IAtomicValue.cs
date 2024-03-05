using System.Numerics;

namespace Pick.Net.Utilities.Threading;

public interface IAtomicValue<T>
{
	//ref T UnsafeValue { get; }

	T Value { get; set; }

	/// <summary>
	/// Set the value and return the old value
	/// </summary>
	/// <inheritdoc cref="Set(T, T)"/>
	T Set(T value);

	/// <summary>
	/// Set the value if its current value is equal to <paramref name="comparand"/>, and return the old value
	/// </summary>
	/// <param name="value">The new value</param>
	/// <param name="comparand">The value to compare with the current value</param>
	/// <returns>The original value.</returns>
	T Set(T value, T comparand);
}

public interface IAtomicValue<TSelf, TValue> : IAtomicValue<TValue>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
	where TSelf : IAtomicValue<TSelf, TValue>
{
	static abstract TSelf Create(TValue value);
	static abstract bool operator ==(TSelf? left, TValue? right);
	static abstract bool operator !=(TSelf? left, TValue? right);
	static abstract bool operator ==(TValue? left, TSelf? right);
	static abstract bool operator !=(TValue? left, TSelf? right);

	bool Equals(TValue other);
}
