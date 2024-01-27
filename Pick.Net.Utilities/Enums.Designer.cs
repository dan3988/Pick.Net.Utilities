namespace Pick.Net.Utilities;

unsafe partial class Enums
{
	public static sbyte ToSByte<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToSByte(&value);
	public static sbyte ToSByteUnchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToSByteUnchecked(&value);
	public static byte ToByte<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToByte(&value);
	public static byte ToByteUnchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToByteUnchecked(&value);
	public static short ToInt16<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt16(&value);
	public static short ToInt16Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt16Unchecked(&value);
	public static ushort ToUInt16<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt16(&value);
	public static ushort ToUInt16Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt16Unchecked(&value);
	public static int ToInt32<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt32(&value);
	public static int ToInt32Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt32Unchecked(&value);
	public static uint ToUInt32<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt32(&value);
	public static uint ToUInt32Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt32Unchecked(&value);
	public static long ToInt64<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt64(&value);
	public static long ToInt64Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToInt64Unchecked(&value);
	public static ulong ToUInt64<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt64(&value);
	public static ulong ToUInt64Unchecked<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToUInt64Unchecked(&value);
	public static float ToSingle<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToSingle(&value);
	public static double ToDouble<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToDouble(&value);
	public static decimal ToDecimal<T>(this T value) where T : unmanaged, Enum => Enums<T>.Helper.ToDecimal(&value);
}

unsafe partial class EnumHelper
{
	public abstract sbyte ToSByte(void* value);
	public abstract sbyte ToSByteUnchecked(void* value);
	public abstract byte ToByte(void* value);
	public abstract byte ToByteUnchecked(void* value);
	public abstract short ToInt16(void* value);
	public abstract short ToInt16Unchecked(void* value);
	public abstract ushort ToUInt16(void* value);
	public abstract ushort ToUInt16Unchecked(void* value);
	public abstract int ToInt32(void* value);
	public abstract int ToInt32Unchecked(void* value);
	public abstract uint ToUInt32(void* value);
	public abstract uint ToUInt32Unchecked(void* value);
	public abstract long ToInt64(void* value);
	public abstract long ToInt64Unchecked(void* value);
	public abstract ulong ToUInt64(void* value);
	public abstract ulong ToUInt64Unchecked(void* value);
	public abstract float ToSingle(void* value);
	public abstract double ToDouble(void* value);
	public abstract decimal ToDecimal(void* value);
}

unsafe partial class EnumHelper<T>
{
	public override sbyte ToSByte(void* value) => sbyte.CreateChecked(*(T*)value);
	public override sbyte ToSByteUnchecked(void* value) => sbyte.CreateTruncating(*(T*)value);
	public override byte ToByte(void* value) => byte.CreateChecked(*(T*)value);
	public override byte ToByteUnchecked(void* value) => byte.CreateTruncating(*(T*)value);
	public override short ToInt16(void* value) => short.CreateChecked(*(T*)value);
	public override short ToInt16Unchecked(void* value) => short.CreateTruncating(*(T*)value);
	public override ushort ToUInt16(void* value) => ushort.CreateChecked(*(T*)value);
	public override ushort ToUInt16Unchecked(void* value) => ushort.CreateTruncating(*(T*)value);
	public override int ToInt32(void* value) => int.CreateChecked(*(T*)value);
	public override int ToInt32Unchecked(void* value) => int.CreateTruncating(*(T*)value);
	public override uint ToUInt32(void* value) => uint.CreateChecked(*(T*)value);
	public override uint ToUInt32Unchecked(void* value) => uint.CreateTruncating(*(T*)value);
	public override long ToInt64(void* value) => long.CreateChecked(*(T*)value);
	public override long ToInt64Unchecked(void* value) => long.CreateTruncating(*(T*)value);
	public override ulong ToUInt64(void* value) => ulong.CreateChecked(*(T*)value);
	public override ulong ToUInt64Unchecked(void* value) => ulong.CreateTruncating(*(T*)value);
	public override float ToSingle(void* value) => float.CreateChecked(*(T*)value);
	public override double ToDouble(void* value) => double.CreateChecked(*(T*)value);
	public override decimal ToDecimal(void* value) => decimal.CreateChecked(*(T*)value);
}