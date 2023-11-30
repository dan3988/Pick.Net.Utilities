using Microsoft.CodeAnalysis.Diagnostics;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableInstancePropertyAccessorAnalyzer : DiagnosticAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed);

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

		context.RegisterSyntaxNodeAction(c => AnalyzeSymbol(c, type), SyntaxKind.PropertyDeclaration);
	}

	private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context, INamedTypeSymbol attributeType)
	{
		var node = (PropertyDeclarationSyntax)context.Node;
		if (node.AccessorList == null)
			return;

		var symbol = context.SemanticModel.GetDeclaredSymbol(node);
		var attr = symbol?.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		symbol!.GetType();

		if (symbol.IsAutoProperty())
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, symbol, symbol.Name);
			return;
		}

		foreach (var accessor in node.AccessorList.Accessors)
		{
			if (!accessor.IsBindablePropertyUsed(symbol!.Name))
			{
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, symbol, symbol.Name);
				break;
			}
		}
	}
}