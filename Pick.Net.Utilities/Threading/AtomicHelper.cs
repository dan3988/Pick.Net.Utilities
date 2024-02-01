using System.Runtime.CompilerServices;

namespace Pick.Net.Utilities.Threading;

internal static class AtomicHelper
{
	public const string DebuggerDisplay = "{GetType().Name,nq}({Value})";

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Read(in int value)
		=> Interlocked.CompareExchange(ref Unsafe.AsRef(in value), 0, 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Read(in uint value)
		=> Interlocked.CompareExchange(ref Unsafe.AsRef(in value), 0, 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Read(in long value)
		=> Interlocked.CompareExchange(ref Unsafe.AsRef(in value), 0, 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Read(in ulong value)
		=> Interlocked.CompareExchange(ref Unsafe.AsRef(in value), 0, 0);
}
