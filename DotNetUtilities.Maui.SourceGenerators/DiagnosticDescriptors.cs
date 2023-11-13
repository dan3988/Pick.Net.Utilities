using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	public static DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		"DNU0001",
		"PropertyName cannot be null or empty",
		"PropertyName cannot be null or empty",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyName cannot be null or empty.");

	public static DiagnosticDescriptor BindablePropertyNullPropertyType = new(
		"DNU0002",
		"PropertyType cannot be null",
		"PropertyType cannot be null",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyType cannot be null.");

	public static DiagnosticDescriptor BindablePropertyInvalidAccessor = new(
		"DNU0003",
		"Invalid access level for [BindableProperty]",
		"Invalid PropertyAccessLevel: {0}",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyAccessLevel.");

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
