﻿using Microsoft.CodeAnalysis.Diagnostics;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableInstancePropertyAccessorAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor[] Diagnostics =
	[
		DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed,
		DiagnosticDescriptors.BindablePropertyInstanceToAttached,
		DiagnosticDescriptors.BindablePropertyStaticProperty,
		DiagnosticDescriptors.BindablePropertyAttributeOnIndexer
	];

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Diagnostics);

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

		context.RegisterSyntaxNodeAction(c => AnalyzeIndexer(c, type), SyntaxKind.IndexerDeclaration);
		context.RegisterSyntaxNodeAction(c => AnalyzeProperty(c, type), SyntaxKind.PropertyDeclaration);
	}

	private static void AnalyzeIndexer(SyntaxNodeAnalysisContext context, INamedTypeSymbol attributeType)
	{
		var symbol = context.SemanticModel.GetDeclaredSymbol((IndexerDeclarationSyntax)context.Node);
		var attr = symbol?.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr != null)
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttributeOnIndexer, symbol!);
	}

	private static void AnalyzeProperty(SyntaxNodeAnalysisContext context, INamedTypeSymbol attributeType)
	{
		var node = (PropertyDeclarationSyntax)context.Node;
		if (node.AccessorList == null)
			return;

		var symbol = context.SemanticModel.GetDeclaredSymbol(node);
		var attr = symbol?.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		if (symbol!.IsStatic)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyStaticProperty, symbol);
			return;
		}

		context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, symbol);
		
		if (symbol.IsAutoProperty())
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, symbol, symbol.Name);
			return;
		}

		foreach (var accessor in node.AccessorList.Accessors)
		{
			if (!accessor.IsBindablePropertyUsed(symbol.Name))
			{
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, symbol, symbol.Name);
				break;
			}
		}
	}
}