using System.Numerics;

namespace Pick.Net.Utilities.Threading;

public interface IAtomicValue<T>
{
	T Value { get; set; }

	/// <summary>
	/// Set the value and return whether the value has changed
	/// </summary>
	/// <param name="value"></param>
	/// <returns><c>true</c> if the value has changed, otherwise <c>false</c>.</returns>
	bool Set(T value);
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
