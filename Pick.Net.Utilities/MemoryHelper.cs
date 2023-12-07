using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities;

public static unsafe class MemoryHelper
{
	private readonly struct ToHexStringState(nint pointer, int byteCount, bool upperCase)
	{
		private const string Lower = "0123456789abcdef";
		private const string Upper = "0123456789ABCDEF";

		private static void Create(Span<char> span, ToHexStringState state)
		{
			var ptr = (byte*)state.Pointer;

			fixed (char* chars = state.UpperCase ? Upper : Lower)
			fixed (char* strPtr = span)
			{
				const int lowBits = 0xF;

				var unfixedPtr = strPtr;
				var rem = state.ByteCount;
				while (--rem >= 0)
				{
					var lo = *ptr >> 4;
					var hi = *ptr & lowBits;

					*unfixedPtr = chars[lo];
					unfixedPtr++;
					*unfixedPtr = chars[hi];
					unfixedPtr++;
					ptr++;
				}
			}
		}

		public readonly nint Pointer = pointer;
		public readonly int ByteCount = byteCount;
		public readonly bool UpperCase = upperCase;

		public string Create() => string.Create(ByteCount * 2, this, Create);
	}

	private readonly struct ToBinaryStringState(nint pointer, int byteCount)
	{
		private const string chars = "01";
		private static readonly uint[] bitflags = [0b00000001, 0b00000010, 0b00000100, 0b00001000, 0b00010000, 0b00100000, 0b01000000, 0b10000000];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckFlag(char* chars, uint* flags, uint currentByte, ref char* output, int index)
		{
			var hasFlag = (flags[index] & currentByte) != 0;
			*output = chars[*(byte*)&hasFlag];
			output++;
		}

		private static void Create(Span<char> span, ToBinaryStringState state)
		{
			var ptr = (byte*)state.Pointer;

			fixed (uint* flags = bitflags)
			fixed (char* ch = chars)
			fixed (char* strPtr = span)
			{
				var unfixedPtr = strPtr;
				var rem = state.ByteCount;
				while (--rem >= 0)
				{
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 0);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 1);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 2);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 3);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 4);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 5);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 6);
					CheckFlag(ch, flags, *(uint*)ptr, ref unfixedPtr, 7);
					ptr++;
				}
			}
		}

		public readonly nint Pointer = pointer;
		public readonly int ByteCount = byteCount;

		public string Create() => string.Create(ByteCount * 8, this, Create);
	}

	/// <summary>
	/// Creates a string containing the little endian base 16 representation of all the values in <paramref name="value"/>
	/// </summary>
	/// <param name="value">The values to convert</param>
	/// <param name="upperCase">Whether to return an upper case hex string</param>
	public static string ToHexString<T>(this T[] value, bool upperCase = false)
		where T : unmanaged
	{
		fixed (T* p = value)
			return ToHexString(p, sizeof(T) * value.Length, upperCase);
	}

	/// <inheritdoc cref="ToHexString{T}(T[], bool)"/>
	public static string ToHexString<T>(this ReadOnlySpan<T> value, bool upperCase = false)
		where T : unmanaged
	{
		fixed (T* p = value)
			return ToHexString(p, sizeof(T) * value.Length, upperCase);
	}

	/// <summary>
	/// Converts <paramref name="value"/> to a little endian base 16 string representation of its raw data.
	/// </summary>
	/// <param name="value">The value to convert</param>
	/// <inheritdoc cref="ToHexString{T}(T[], bool)"/>
	public static string ToHexString<T>(T value, bool upperCase = false)
		where T : unmanaged
	{
		return ToHexString(&value, sizeof(T), upperCase);
	}

	/// <inheritdoc cref="ToHexString{T}(T, bool)"/>
	public static string ToHexString<T>(in T value, bool upperCase = false)
		where T : unmanaged
	{
		fixed (T* p = &value)
			return ToHexString(p, sizeof(T), upperCase);
	}

	/// <summary>
	/// Converts <paramref name="byteCount"/> bytes from <paramref name="ptr"/> to a little endian base 16 string.
	/// </summary>
	/// <param name="ptr">A pointer to the first byte to convert</param>
	/// <param name="ptr">The amaount of bytes to read from <paramref name="ptr"/></param>
	/// <inheritdoc cref="ToHexString{T}(T[], bool)"/>
	public static string ToHexString(void* ptr, int byteCount, bool upperCase = false)
		=> new ToHexStringState((nint)ptr, byteCount, upperCase).Create();

	/// <summary>
	/// Creates a string containing the little endian base 2 representation of all the values in <paramref name="value"/>
	/// </summary>
	/// <param name="value">The values to convert</param>
	public static string ToBinaryString<T>(this T[] value)
		where T : unmanaged
	{
		fixed (T* p = value)
			return ToBinaryString(p, sizeof(T) * value.Length);
	}

	/// <inheritdoc cref="ToBinaryString{T}(T[])"/>
	public static string ToBinaryString<T>(this ReadOnlySpan<T> value)
		where T : unmanaged
	{
		fixed (T* p = value)
			return ToBinaryString(p, sizeof(T) * value.Length);
	}

	/// <summary>
	/// Converts <paramref name="value"/> to a little endian base 2 string representation of its raw data.
	/// </summary>
	/// <param name="value">The value to convert</param>
	public static string ToBinaryString<T>(T value)
		where T : unmanaged
	{
		return ToBinaryString(&value, sizeof(T));
	}

	/// <inheritdoc cref="ToBinaryString{T}(T))"/>
	public static string ToBinaryString<T>(in T value)
		where T : unmanaged
	{
		fixed (T* p = &value)
			return ToBinaryString(p, sizeof(T));
	}

	/// <summary>
	/// Converts <paramref name="byteCount"/> bytes from <paramref name="ptr"/> to a little endian base 2 string.
	/// </summary>
	/// <param name="ptr">A pointer to the first byte to convert</param>
	/// <param name="ptr">The amaount of bytes to read from <paramref name="ptr"/></param>
	public static string ToBinaryString(void* ptr, int sizeInBytes)
		=> new ToBinaryStringState((nint)ptr, sizeInBytes).Create();
}
