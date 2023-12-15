namespace Pick.Net.Utilities.Collections;

/// <summary>
/// Copy of the internal class <see cref="System.Collections.HashHelpers"/> <a href="https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Collections/HashHelpers.cs">System.Collections.HashHelpers</a>.
/// </summary>
public static class HashHelpers
{
	// This is the maximum prime smaller than Array.MaxArrayLength
	public const int MaxPrimeArrayLength = 0x7FEFFFFD;

	internal const int hashPrime = 101;

	internal static readonly int[] primes = [
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
	];

	public const int MinPrime = 3;

	public static bool IsPrime(int candidate)
	{
		if ((candidate & 1) != 0)
		{
			int limit = (int)Math.Sqrt(candidate);
			for (int divisor = 3; divisor <= limit; divisor += 2)
			{
				if ((candidate % divisor) == 0)
					return false;
			}
			return true;
		}
		return candidate == 2;
	}

	public static int GetPrime(int min)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(min);
#else
		if (min < 0)
			throw new ArgumentOutOfRangeException(nameof(min), $"Value must be a non-negative number.");
#endif
		for (int i = 0; i < primes.Length; i++)
		{
			int prime = primes[i];
			if (prime >= min) return prime;
		}

		//outside of our predefined table. 
		//compute the hard way. 
		for (int i = (min | 1); i < int.MaxValue; i += 2)
		{
			if (IsPrime(i) && ((i - 1) % hashPrime != 0))
				return i;
		}
		return min;
	}

	// Returns size of hashtable to grow to.
	public static int ExpandPrime(int oldSize)
	{
		int newSize = 2 * oldSize;

		// Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
		// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
		return (uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize ? MaxPrimeArrayLength : GetPrime(newSize);
	}
}