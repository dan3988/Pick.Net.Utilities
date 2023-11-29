using Microsoft.CodeAnalysis.Diagnostics;
using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindablePropertyDefaultValueAnalyzer : DiagnosticAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.BindablePropertyDefaultValueNull);

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

		context.RegisterSymbolAction(c => AnalyzeSymbol(c, type), SymbolKind.Property, SymbolKind.Method);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol attributeType)
	{
		var symbol = context.Symbol;
		var attr = symbol.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		var hasDefault = false;
		var hasGenerator = false;
		var dictionary = attr.NamedArguments.ToDictionary(v => v.Key, v => v.Value);

		foreach (var (name, value) in attr.NamedArguments)
		{
			switch (name)
			{
				case nameof(BindablePropertyAttribute.DefaultValue):
					hasDefault = !value.IsNull;
					break;
				case nameof(BindablePropertyAttribute.DefaultValueFactory):
					hasGenerator = ((bool?)value.Value).GetValueOrDefault();
					break;
			}
		}

		var hasNonNullDefault = hasDefault || hasGenerator;
		if (hasDefault && hasGenerator)
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory, symbol);

		if (hasDefault || hasGenerator)
			return;

		switch (symbol)
		{
			case IPropertySymbol { Type: { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated } }:
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull, symbol, symbol.Name);
				break;
			case IMethodSymbol { ReturnType: { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated } } m:
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull, symbol, Identifiers.GetAttachedPropertyName(m.Name));
				break;
		}
	}
}
