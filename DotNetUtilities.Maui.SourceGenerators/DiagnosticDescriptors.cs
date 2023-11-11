using Microsoft.CodeAnalysis;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	public static DiagnosticDescriptor BindablePropertyInvalidAccessor = new(
		"DNU0001",
		"Invalid access level for [BindableProperty]",
		"Invalid PropertyAccessLevel: {0}",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyAccessLevel.");
}
