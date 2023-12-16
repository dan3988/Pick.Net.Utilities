using System.Collections;
using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Collections;

internal static class CollectionHelper
{
	public static T ConvertKey<T>(object? key, [CallerArgumentExpression(nameof(key))] string argName = null!)
	{
		ArgumentNullException.ThrowIfNull(key);
		return key is T t ? t : throw new ArgumentException($"The given key '{key}' is not a string and cannot be used in this generic collection", argName);
	}

	public static T ConvertValue<T>(object? value, [CallerArgumentExpression(nameof(value))] string argName = null!)
	{
		if (value == null)
		{
			return TypeHelper<T>.IsNullable ? default! : throw new ArgumentNullException(argName, $"Cannot convert null to '{typeof(T)}'.");
		}
		else if (value is T v)
		{
			return v;
		}

		throw new ArgumentException($"The given value '{value}' is not of type '{typeof(T)}' and cannot be used in this generic collection.", argName);
	}

	public static void CheckCopyToArgs(ICollection collection, Array array, int index, out int count)
		=> CheckCopyToArgs(array, index, count = collection.Count);

	public static void CheckCopyToArgs(Array array, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(array);

		if (unchecked((uint)index > (uint)array.Length))
			throw new ArgumentOutOfRangeException(nameof(index), "Index must be a non-negative number smaller than the size of the array");

		if (count > (index + array.Length))
			throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(index));
	}

	public static ArgumentException DuplicateAddedException<T>(T key, [CallerArgumentExpression(nameof(key))] string argName = null!)
		=> new("An item with the same key has already been added. Key: " + key, argName);

	public static KeyNotFoundException KeyNotFoundException<T>(T key)
		=> new($"The given key '{key}' was not present in the dictionary");

	public static KeyNotFoundException KeyNotFoundException(ReadOnlySpan<char> key)
		=> new($"The given key '{key}' was not present in the dictionary");

	public static InvalidOperationException EnumeratorInvalidatedException()
		=> new("Collection was modified; enumeration operation may not execute");
}
