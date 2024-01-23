using Microsoft.CodeAnalysis.Diagnostics;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
	private static readonly ImmutableArray<DiagnosticDescriptor> Diagnostics = ImmutableArray.Create(
	[
		DiagnosticDescriptors.BindablePropertyNoDefaultValue,
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
		if (context.IsGeneratedCode)
			return;

		var symbol = context.Symbol;
		var attr = symbol.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		var (propName, propType) = symbol switch
		{
			IPropertySymbol s => (s.Name, s.Type),
			IMethodSymbol s => (Identifiers.GetAttachedPropertyName(s.Name), s.ReturnType),
			_ => throw new ArgumentException("Unexpected symbol type: " + symbol.Kind, nameof(symbol))
		};

		var dictionary = attr.NamedArguments.ToDictionary(v => v.Key, v => v.Value);
		if (dictionary.TryGetValue(nameof(BindablePropertyAttribute.DefaultMode), out var value) && (value is not { Value: int intValue } || !Enum.IsDefined(typeof(BindingMode), intValue)))
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, symbol, propName, value.Value);

		if (!dictionary.TryGetValue(nameof(BindablePropertyAttribute.DefaultValue), out var defaultValueProp) || defaultValueProp.IsNull)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, symbol, propName);

			if (propType is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated })
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull, symbol, propName);
		}
	}
}
