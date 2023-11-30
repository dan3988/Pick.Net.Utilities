using Microsoft.CodeAnalysis.Diagnostics;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableAttachedPropertyMethodAnalyzer : DiagnosticAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();
		context.RegisterCompilationStartAction(Analyze);
	}

	private static void Analyze(CompilationStartAnalysisContext context)
	{
		var type = context.Compilation.GetTypeByMetadataName(typeof(BindablePropertyAttribute).FullName);
		if (type == null)
			return;

		context.RegisterSyntaxNodeAction(c => AnalyzeSymbol(c, type), SyntaxKind.MethodDeclaration);
	}

	private static bool CheckSetMethod(MethodDeclarationSyntax node, IMethodSymbol getterSymbol, string name)
	{
		if (getterSymbol.Parameters.Length == 0)
			return false;

		var setter = getterSymbol.ContainingType.GetAttachedSetMethod(getterSymbol.ReturnType, getterSymbol.Parameters[0].Type, name);
		if (setter == null)
			return false;

		var location = setter.Locations.First();
		var setterNode = node.Parent?.FindNode(location.SourceSpan) as MethodDeclarationSyntax;
		return setterNode != null && !node.IsBindablePropertyUsed(name, false);
	}

	private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context, INamedTypeSymbol attributeType)
	{
		var node = (MethodDeclarationSyntax)context.Node;
		var symbol = context.SemanticModel.GetDeclaredSymbol(node);
		if (symbol == null || symbol.IsPartialDefinition)
			return;

		var attr = symbol?.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		var name = Identifiers.GetAttachedPropertyName(symbol!.Name);
		if (!node.IsBindablePropertyUsed(name, false) || CheckSetMethod(node, symbol, name))
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed, symbol, name);
	}
}