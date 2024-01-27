using System.Numerics;

namespace Pick.Net.Utilities.Threading;

public interface IAtomicNumber<T> : IAtomicValue<T>
	where T : INumber<T>
{
	/// <summary>
	/// Adds one to the value
	/// </summary>
	/// <returns>The incremented value</returns>
	T Increment();

	/// <summary>
	/// Subtracts one from the value
	/// </summary>
	/// <returns>The decremented value</returns>
	T Decrement();
}

public interface IAtomicNumber<TSelf, TValue> : IAtomicValue<TSelf, TValue>, IAtomicNumber<TValue>
	where TSelf : IAtomicNumber<TSelf, TValue>
	where TValue : INumber<TValue>
{
}
