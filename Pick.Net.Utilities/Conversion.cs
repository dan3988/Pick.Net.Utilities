using System.Diagnostics.CodeAnalysis;

using SConvert = System.Convert;

namespace Pick.Net.Utilities;

public static class Conversion
{
	private static readonly Type EmptyType = typeof(object).Module.GetType("System.Empty")!;

	private static readonly Type?[] TypeCodeTypes =
	{
		/* TypeCode.Empty       */ EmptyType,
		/* TypeCode.Object      */ typeof(object),
		/* TypeCode.DBNull      */ typeof(DBNull),
		/* TypeCode.Boolean     */ typeof(bool),
		/* TypeCode.Char        */ typeof(char),
		/* TypeCode.SByte       */ typeof(sbyte),
		/* TypeCode.Byte        */ typeof(byte),
		/* TypeCode.Int16       */ typeof(short),
		/* TypeCode.UInt16      */ typeof(ushort),
		/* TypeCode.Int32       */ typeof(int),
		/* TypeCode.UInt32      */ typeof(uint),
		/* TypeCode.Int64       */ typeof(long),
		/* TypeCode.UInt64      */ typeof(ulong),
		/* TypeCode.Single      */ typeof(float),
		/* TypeCode.Double      */ typeof(double),
		/* TypeCode.Decimal     */ typeof(decimal),
		/* TypeCode.DateTime    */ typeof(DateTime),
		/* gap                  */ null,
		/* TypeCode.String      */ typeof(string),
	};

	private static readonly Dictionary<Type, TypeCode> TypeCodeMap = new()
	{
		[EmptyType] = TypeCode.Empty,
		[typeof(object)] = TypeCode.Object,
		[typeof(DBNull)] = TypeCode.DBNull,
		[typeof(bool)] = TypeCode.Boolean,
		[typeof(char)] = TypeCode.Char,
		[typeof(sbyte)] = TypeCode.SByte,
		[typeof(byte)] = TypeCode.Byte,
		[typeof(short)] = TypeCode.Int16,
		[typeof(ushort)] = TypeCode.UInt16,
		[typeof(int)] = TypeCode.Int32,
		[typeof(uint)] = TypeCode.UInt32,
		[typeof(long)] = TypeCode.Int64,
		[typeof(ulong)] = TypeCode.UInt64,
		[typeof(float)] = TypeCode.Single,
		[typeof(double)] = TypeCode.Double,
		[typeof(decimal)] = TypeCode.Decimal,
		[typeof(DateTime)] = TypeCode.DateTime,
		[typeof(string)] = TypeCode.String,
	};

	public static Type ToType(this TypeCode typeCode)
	{
		if (unchecked((uint)typeCode < (uint)TypeCodeTypes.Length))
		{
			var type = TypeCodeTypes[(int)typeCode];
			if (type != null)
				return type;
		}

		throw new ArgumentException("Unknown TypeCode: " + typeCode, nameof(typeCode));
	}

	internal static TypeCode GetTypeCodeFast(Type type)
		=> TypeCodeMap[type];

	public static TypeCode GetTypeCode(Type type)
	{
		if (type.IsValueType)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum)
				type = type.GetEnumUnderlyingType();
		}

		if (!TypeCodeMap.TryGetValue(type, out var code))
			code = TypeCode.Object;

		return code;
	}

	/// <summary>
	/// Uses <see cref="SConvert.ChangeType(object?, Type)"/> to convert <paramref name="value"/> to an instance of <paramref name="type"/>, but supports conversion to nullable and enum types.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="type"></param>
	/// <param name="formatProvider"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c> and <paramref name="type"/> is a value type thas is not <see cref="Nullable{T}"/>.</exception>
	/// <exception cref="FormatException"><paramref name="value"/> is a string in a format not recognised by <paramref name="type"/>.</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> represents a number that is out of the range of <paramref name="type"/>.</exception>
	/// <exception cref="InvalidCastException">Conversion from <paramref name="value"/> to <paramref name="type"/> is not supported.</exception>
	[return: NotNullIfNotNull(nameof(value))]
	public static object? Convert(object? value, Type type, IFormatProvider? formatProvider = null)
	{
		var nt = Nullable.GetUnderlyingType(type);
		if (nt != null)
		{
			type = nt;
		}

		if (value == null)
		{
			return !type.IsValueType || nt != null ? null : throw new ArgumentNullException(nameof(value), $"Cannot convert null to {type}.");
		}

		if (type.IsInstanceOfType(value))
		{
			return value;
		}
		else if (type.IsEnum)
		{
			return value is string str ? Enum.Parse(type, str) : Enum.ToObject(type, value);
		}
		else
		{
			return SConvert.ChangeType(value, type, formatProvider);
		}
	}

	/// <summary>
	/// Uses <see cref="SConvert.ChangeType(object?, Type)"/> to convert <paramref name="value"/> to an instance of <typeparamref name="T"/>, but supports conversion to nullable and enum types.
	/// </summary>
	/// <inheritdoc cref="Convert(object?, Type, IFormatProvider?)"/>
	[return: NotNullIfNotNull(nameof(value))]
	public static T? Convert<T>(object? value, IFormatProvider? formatProvider = null) => (T?)Convert(value, typeof(T), formatProvider);

	public static T ChangeType<T>(object value) where T : IConvertible => (T)SConvert.ChangeType(value, typeof(T));
}
