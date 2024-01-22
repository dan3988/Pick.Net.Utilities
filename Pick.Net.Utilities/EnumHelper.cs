using System.Numerics;

namespace Pick.Net.Utilities;

internal abstract unsafe class EnumHelper
{
	public static readonly EnumHelper<sbyte> SByte = new();
	public static readonly EnumHelper<byte> Byte = new();
	public static readonly EnumHelper<ushort> UInt16 = new();
	public static readonly EnumHelper<short> Int16 = new();
	public static readonly EnumHelper<uint> UInt32 = new();
	public static readonly EnumHelper<int> Int32 = new();
	public static readonly EnumHelper<ulong> UInt64 = new();
	public static readonly EnumHelper<long> Int64 = new();

	private static readonly EnumHelper?[] TypeCodeLookup =
	{
		/* TypeCode.Empty = 0		*/ null,
		/* TypeCode.Object = 1		*/ null,
		/* TypeCode.DBNull = 2		*/ null,
		/* TypeCode.Boolean = 3		*/ null,
		/* TypeCode.Char = 4		*/ null,
		/* TypeCode.SByte = 5		*/ SByte,
		/* TypeCode.Byte = 6		*/ Byte,
		/* TypeCode.Int16 = 7		*/ Int16,
		/* TypeCode.UInt16 = 8		*/ UInt16,
		/* TypeCode.Int32 = 9		*/ Int32,
		/* TypeCode.UInt32 = 10		*/ UInt32,
		/* TypeCode.Int64 = 9		*/ Int64,
		/* TypeCode.UInt64 = 10		*/ UInt64
	};

	public static EnumHelper ForTypeCode(TypeCode typeCode)
	{
		if (unchecked((uint)typeCode < (uint)TypeCodeLookup.Length))
		{
			var value = TypeCodeLookup[(int)typeCode];
			if (value != null)
				return value;
		}

		throw new ArgumentException("Invalid enum underlying type: " + typeCode, nameof(typeCode));
	}

	public abstract bool HasFlag(void* value, void* flag);

	public abstract bool Equals(void* x, void* y);

	public abstract int Compare(void* x, void* y);
}

internal sealed unsafe class EnumHelper<T> : EnumHelper
	where T : unmanaged, IBitwiseOperators<T, T, T>, IEqualityOperators<T, T, bool>, IComparable<T>
{
	public override bool HasFlag(void* value, void* flag)
		=> (*(T*)value & *(T*)flag) == *(T*)flag;

	public override bool Equals(void* x, void* y)
		=> *(T*)x == *(T*)y;

	public override int Compare(void* x, void* y)
		=> (*(T*)x).CompareTo(*(T*)y);
}
