using System.Diagnostics.CodeAnalysis;

using SConvert = System.Convert;

namespace DotNetUtilities.Maui;

public static class Conversion
{
	[return: NotNullIfNotNull(nameof(value))]
	public static object? Convert(object? value, Type type, IFormatProvider? formatProvider = null)
	{
		if (value == null)
		{
			if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
				throw new ArgumentException($"Cannot convert null to {type}.", nameof(value));

			return null;
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

	[return: NotNullIfNotNull(nameof(value))]
	public static T? Convert<T>(object? value, IFormatProvider? formatProvider = null) => (T?)Convert(value, typeof(T), formatProvider);

	public static T ChangeType<T>(object value) where T : IConvertible => (T)SConvert.ChangeType(value, typeof(T));

	public static T ParseEnum<T>(string value, bool ignoreCase = false) where T : unmanaged, Enum => (T)Enum.Parse(typeof(T), value, ignoreCase);
}
