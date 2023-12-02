using Microsoft.CodeAnalysis.Diagnostics;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableAttachedPropertyMethodAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor[] Diagnostics =
	[
		DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed,
		DiagnosticDescriptors.BindablePropertyAttachedMethodToPartial,
		DiagnosticDescriptors.BindablePropertyAttachedToInstance,
		DiagnosticDescriptors.BindablePropertyAttachedPropertyNullabilityMismatch,
		DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName,
		DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn,
		DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature,
		DiagnosticDescriptors.BindablePropertyInstanceMethod
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

		context.RegisterSyntaxNodeAction(c => AnalyzeNode(c, type), SyntaxKind.MethodDeclaration);
	}

	private static (MethodDeclarationSyntax? Node, IMethodSymbol? Symbol) FindSetMethod(MethodDeclarationSyntax node, IMethodSymbol getterSymbol, string name)
	{
		if (getterSymbol.Parameters.Length == 0)
			return default;

		var setterSymbol = getterSymbol.ContainingType.GetAttachedSetMethod(getterSymbol.ReturnType, getterSymbol.Parameters[0].Type, name);
		if (setterSymbol == null)
			return default;

		var location = setterSymbol.Locations.FirstOrDefault(v => v.SourceTree == node.SyntaxTree);
		if (location == null)
			return default;

		var setterNode = node.Parent?.FindNode(location.SourceSpan) as MethodDeclarationSyntax;
		return (setterNode, setterSymbol);
	}

	private static void AnalyzeNode(SyntaxNodeAnalysisContext context, INamedTypeSymbol attributeType)
	{
		if (context.IsGeneratedCode)
			return;

		var node = (MethodDeclarationSyntax)context.Node;
		var symbol = context.SemanticModel.GetDeclaredSymbol(node);
		if (symbol == null)
			return;

		var attr = symbol?.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr == null)
			return;

		//prevent null warnings
		symbol!.GetType();

		if (!Identifiers.GetAttachedPropertyName(symbol.Name, out var propertyName))
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName, symbol, propertyName);

		var validSignature = true;

		if (symbol.ReturnsVoid)
		{
			validSignature = false;
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn, symbol, propertyName);
		}

		if (symbol.Parameters.Length != 1)
		{
			validSignature = false;
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, symbol, propertyName);
		}

		if (validSignature && !symbol.ContainingType.IsStatic)
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance, symbol);

		if (!symbol.IsStatic)
		{
			validSignature = false;
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceMethod, symbol);
		}

		if (!validSignature)
			return;

		var (setterNode, setterSymbol) = FindSetMethod(node, symbol, propertyName);

		if (!symbol.IsPartialDefinition)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedMethodToPartial, symbol, propertyName);

			if (!node.IsBindablePropertyUsed(propertyName, false) || (setterNode != null && setterNode.IsBindablePropertyUsed(propertyName, symbol.DeclaredAccessibility != setterSymbol!.DeclaredAccessibility)))
				context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed, symbol, propertyName);

		}

		if (setterSymbol != null && symbol.ReturnType.NullableAnnotation != setterSymbol.Parameters[1].Type.NullableAnnotation)
		{
			context.ReportDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedPropertyNullabilityMismatch, symbol, propertyName);
		}
	}
}