using System.Collections.Immutable;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Category = "BindablePropertyGenerator";

	public static readonly DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		"DNU0001",
		"PropertyName cannot be null or empty",
		"PropertyName cannot be null or empty",
		Category,
		DiagnosticSeverity.Error,
		true,
		"PropertyName cannot be null or empty.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidPropertyName = new(
		"DNU0002",
		"PropertyName is not a valid .NET identifier",
		"PropertyName is not a valid .NET identifier: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"PropertyName is not a valid .NET identifier.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		"DNU0003",
		"Invalid BindingMode value for [BindableProperty]",
		"Invalid BindingMode value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Invalid BindingMode value.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidVisibility = new(
		"DNU0004",
		"Invalid PropertyVisibility value for [BindableProperty]",
		"Invalid PropertyVisibility value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyVisibility value.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNotSupported = new(
		"DNU0005",
		"DefaultValue not supported for given property type on [BindableProperty]",
		"DefaultValue not supported for property type {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"DefaultValue not supported for the given property type.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueAndFactory = new(
		"DNU0006",
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
