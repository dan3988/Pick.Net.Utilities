using Microsoft.CodeAnalysis.Diagnostics;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

internal static class AnalyzerHelper
{
	public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params object?[] messageArgs)
	{
		var location = symbol.Locations.FirstOrDefault();
		ReportDiagnostic(context, descriptor, location, messageArgs);
	}

	public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location? location, params object?[] messageArgs)
	{
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		context.ReportDiagnostic(diagnostic);
	}
}
