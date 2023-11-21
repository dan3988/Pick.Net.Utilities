using System.Collections.Immutable;
using System.Text;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Prefix = "PNUM";
	private const string Category = "Pick.Net.Utilities.Maui.SourceGenerators.BindablePropertyGenerator";

	private static string EnumText<T>() where T : unmanaged, Enum
	{
		var names = Enum.GetNames(typeof(T));
		return ListText(names);
	}

	private static string ListText(string[] values)
	{
		if (values.Length < 2)
			throw new ArgumentException("Must supply at leas 2 values", nameof(values));
		
		var sb = new StringBuilder();
		var i = 1;

		sb.Append(values[0]);

		while (true)
		{
			var value = values[i];
			if (++i == values.Length)
				return sb.Append(" or ").Append(value).ToString();

			sb.Append(", ").Append(value);
		}
	}

	public static readonly DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		Prefix + "0001",
		"Bindable property name cannot be null or empty",
		"Bindable property name cannot be null or empty",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Cannot create a BindableProperty with a null or empty property name.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidPropertyName = new(
		Prefix + "0002",
		"Supplied property name is not a valid .NET identifier",
		"Property name is not a valid .NET identifier: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Cannot create a BindableProperty with a property name that is not a valid .NET identifier.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		Prefix + "0003",
		"Supplied DefaultMode value is not a known value",
		"Unknown DefaultMode value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid BindingMode value ({EnumText<BindingMode>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidVisibility = new(
		Prefix + "0004",
		"Supplied PropertyVisibility value is not a known value",
		"Unknown PropertyVisibility value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid PropertyVisibility value ({EnumText<PropertyVisibility>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNotSupported = new(
		Prefix + "0005",
		"DefaultValue not supported for given property type",
		"Cannot specify DefaultValue for property type {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Cannot specify DefaultValue on for properties with types not supported by attributes.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueAndFactory = new(
		Prefix + "0006",
		"Value of DefaultValue will not be used",
		"Setting DefaultValue will have no effect when DefaultValueFactory is true",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"The value passed into DefaultValue will be ignored if DefaultValueFactory is true.");

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, SyntaxReference? owner, params object?[] messageArgs)
	{
		var location = owner == null ? null : Location.Create(owner.SyntaxTree, owner.Span);
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, Location? location, params object?[] messageArgs)
	{
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}
}
