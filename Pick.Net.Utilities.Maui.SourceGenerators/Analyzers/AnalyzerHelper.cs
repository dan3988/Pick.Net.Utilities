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
	
	public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params object?[] messageArgs)
	{
		var location = symbol.Locations.FirstOrDefault();
		ReportDiagnostic(context, descriptor, location, messageArgs);
	}

	public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location? location, params object?[] messageArgs)
	{
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		context.ReportDiagnostic(diagnostic);
	}

	public static bool IsBindablePropertyUsed(this BaseMethodDeclarationSyntax node, string propertyName, bool isReadOnly)
		=> IsBindablePropertyUsed(node.ExpressionBody, node.Body, propertyName, isReadOnly);

	public static bool IsBindablePropertyUsed(this PropertyDeclarationSyntax node, string propertyName)
	{
		if (node.AccessorList == null)
			return false;

		foreach (var accessor in node.AccessorList.Accessors)
			if (!IsBindablePropertyUsed(accessor, propertyName))
				return false;

		return true;
	}

	public static bool IsBindablePropertyUsed(this AccessorDeclarationSyntax node, string propertyName)
		=> IsBindablePropertyUsed(node.ExpressionBody, node.Body, propertyName, !node.IsKind(SyntaxKind.GetAccessorDeclaration) && node.Modifiers.Count > 0);

	private static bool IsBindablePropertyUsed(ArrowExpressionClauseSyntax? expressionBody, BlockSyntax? body, string propertyName, bool isReadOnly)
	{
		var suffix = isReadOnly ? "PropertyKey" : "Property";
		if (expressionBody != null && IsBindablePropertyUsed(expressionBody, propertyName, suffix))
			return true;

		if (body != null && IsBindablePropertyUsed(body, propertyName, suffix))
			return true;

		return false;
	}

	private static bool IsBindablePropertyUsed(SyntaxNode node, string propertyName, bool isReadOnly)
		=> IsBindablePropertyUsed(node, propertyName, isReadOnly ? "PropertyKey" : "Property");

	private static bool IsBindablePropertyUsed(SyntaxNode node, string propertyName, string suffix)
		=> node.SearchRecursive<IdentifierNameSyntax>(v => Identifiers.StringStartsAndEndsWith(v.Identifier.Text, propertyName, suffix));
}
