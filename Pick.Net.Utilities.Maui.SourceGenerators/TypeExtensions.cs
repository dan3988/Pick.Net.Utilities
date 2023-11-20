namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class TypeExtensions
{
	private static readonly Dictionary<Type, TypeCode> typeCodeMapping = new()
	{
		{ typeof(object), TypeCode.Object },
		{ typeof(DBNull), TypeCode.DBNull },
		{ typeof(bool), TypeCode.Boolean },
		{ typeof(char), TypeCode.Char },
		{ typeof(sbyte), TypeCode.SByte },
		{ typeof(byte), TypeCode.Byte },
		{ typeof(short), TypeCode.Int16 },
		{ typeof(ushort), TypeCode.UInt16 },
		{ typeof(int), TypeCode.Int32 },
		{ typeof(uint), TypeCode.UInt32 },
		{ typeof(long), TypeCode.Int64 },
		{ typeof(ulong), TypeCode.UInt64 },
		{ typeof(float), TypeCode.Single },
		{ typeof(double), TypeCode.Double },
		{ typeof(decimal), TypeCode.Decimal },
		{ typeof(DateTime), TypeCode.DateTime },
		{ typeof(string), TypeCode.String },
	};

	public static bool TryGetTypeCode(this Type type, out TypeCode typeCode)
		=> typeCodeMapping.TryGetValue(type, out typeCode);
}
