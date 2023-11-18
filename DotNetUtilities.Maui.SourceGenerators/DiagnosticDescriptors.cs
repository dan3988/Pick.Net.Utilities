using System.Collections.Immutable;

using DotNetUtilities.Maui.SourceGenerators.Generators;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	public static DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		"DNU0001",
		"PropertyName cannot be null or empty",
		"PropertyName cannot be null or empty",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyName cannot be null or empty.");

	public static DiagnosticDescriptor BindablePropertyNullPropertyType = new(
		"DNU0002",
		"PropertyType cannot be null",
		"PropertyType cannot be null",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyType cannot be null.");

	public static DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		"DNU0003",
		"Invalid BindingMode value for [BindableProperty]",
		"Invalid BindingMode value: {0}",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid BindingMode value.");

	public static DiagnosticDescriptor BindablePropertyInvalidVisibility = new(
		"DNU0004",
		"Invalid PropertyVisibility value for [BindableProperty]",
		"Invalid PropertyVisibility value: {0}",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyVisibility value.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueNotSupported = new(
		"DNU0005",
		"DevaultValue not supported for given property type on [BindableProperty]",
		"DevaultValue not supported for property type {0}",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"DevaultValue not supported for the given property type.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueInvalid = new(
		"DNU0006",
		"Invalid DevaultValue value for [BindableProperty]",
		"Invalid DevaultValue value: {0}",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid DevaultValue value.");

	public static DiagnosticDescriptor BindablePropertyDefaultValueCantConvert = new(
		"DNU0007",
		"Can't convert given DefaultValue of [BindableProperty]",
		"Can't convert DefaultValue from {0} to {1}",
		typeof(BaseBindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Can't convert given DefaultValue.");

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
