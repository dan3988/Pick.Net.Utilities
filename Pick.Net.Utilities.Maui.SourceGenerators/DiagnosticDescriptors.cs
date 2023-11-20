using System.Collections.Immutable;

using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Category = "BindablePropertyGenerator";

	public static DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		"DNU0001",
		"PropertyName cannot be null or empty",
		"PropertyName cannot be null or empty",
		Category,
		DiagnosticSeverity.Error,
		true,
		"PropertyName cannot be null or empty.");

	public static DiagnosticDescriptor BindablePropertyNullPropertyType = new(
		"DNU0002",
		"PropertyType cannot be null",
		"PropertyType cannot be null",
		Category,
		DiagnosticSeverity.Error,
		true,
		"PropertyType cannot be null.");

	public static DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		"DNU0003",
		"Invalid BindingMode value for [BindableProperty]",
		"Invalid BindingMode value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Invalid BindingMode value.");

	public static DiagnosticDescriptor BindablePropertyInvalidVisibility = new(
		"DNU0004",
		"Invalid PropertyVisibility value for [BindableProperty]",
		"Invalid PropertyVisibility value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyVisibility value.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueNotSupported = new(
		"DNU0005",
		"DefaultValue not supported for given property type on [BindableProperty]",
		"DefaultValue not supported for property type {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"DefaultValue not supported for the given property type.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueInvalid = new(
		"DNU0006",
		"Invalid DefaultValue value for [BindableProperty]",
		"Invalid DefaultValue value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Invalid DefaultValue value.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueCantConvert = new(
		"DNU0007",
		"Can't convert given DefaultValue of [BindableProperty]",
		"Can't convert DefaultValue from {0} to {1}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Can't convert given DefaultValue.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueAndFactory = new(
		"DNU0008",
		"Can't specify a value for DefaultValue while DefaultValueFactory is true for [BindableProperty]",
		"Can't specify a value for DefaultValue while DefaultValueFactory is true",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Can't specify a value for DefaultValue while DefaultValueFactory is true.");

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
