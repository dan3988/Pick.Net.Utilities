using Microsoft.CodeAnalysis.Diagnostics;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
	private static readonly ImmutableArray<DiagnosticDescriptor> Diagnostics = ImmutableArray.Create(
	[
		DiagnosticDescriptors.BindablePropertyDefaultValueNull,
		DiagnosticDescriptors.BindablePropertyInvalidDefaultMode
	]);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Diagnostics;

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

		var dictionary = attr.NamedArguments.ToDictionary(v => v.Key, v => v.Value);
		if (dictionary.TryGetValue(nameof(BindablePropertyAttribute.DefaultMode), out var value) && (value is not { Value: int intValue } || !Enum.IsDefined(typeof(BindingMode), intValue)))
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, symbol, value.Value);

		if (!dictionary.TryGetValue(nameof(BindablePropertyAttribute.DefaultValue), out var defalutValueProp) || defalutValueProp.IsNull)
		{
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
}
