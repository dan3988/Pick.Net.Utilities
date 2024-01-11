using System.Text;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HandlerClassAnalyzer : DiagnosticAnalyzer
{
	private sealed record MauiPlatform(string Identifier, string Reference)
	{
		public static readonly MauiPlatform Android = new("Android", "Mono.Android");
		public static readonly MauiPlatform iOS = new("iOS", "Microsoft.iOS");
	}

	private static readonly MauiPlatform[] Platforms =
	[
		MauiPlatform.Android,
		MauiPlatform.iOS,
	];

	private static readonly ImmutableDictionary<string, MauiPlatform> ReferenceMap = Platforms.ToImmutableDictionary(v => v.Reference);

	private static readonly ImmutableArray<DiagnosticDescriptor> Diagnostics = ImmutableArray.Create(DiagnosticDescriptors.HandlerAddPartialPlatformClass);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = Diagnostics;

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();
		context.RegisterCompilationStartAction(Analyze);
	}

	private static void Analyze(CompilationStartAnalysisContext context)
	{
		var type = context.Compilation.GetTypeByMetadataName(typeof(HandlerMapperAttribute).FullName);
		if (type == null)
			return;

		context.RegisterSymbolAction(c => AnalyzeSymbol(c, type), SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol attributeType)
	{
		if (context.IsGeneratedCode)
			return;

		var type = (ITypeSymbol)context.Symbol;
		var attr = type.GetAttributes().FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType));
		if (attr is not { ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol viewType }] })
			return;

		var compilation = (CSharpCompilation)context.Compilation;
		var handlerPropertyAttributeType = compilation.GetTypeByMetadataName(typeof(HandlerPropertyAttribute).FullName);
		if (handlerPropertyAttributeType == null)
			return;

		var propertiesJson = default(string);

		foreach (var reference in compilation.ReferencedAssemblyNames)
		{
			if (!ReferenceMap.TryGetValue(reference.Name, out var platform))
				continue;

			propertiesJson ??= SerializeProperties(viewType, handlerPropertyAttributeType);

			var location = context.Symbol.Locations.FirstOrDefault();
			var properties = ImmutableDictionary.CreateRange<string, string?>(
			[
				new("Platform", platform.Identifier),
				new("Properties", propertiesJson),
				new("ViewType", viewType.GetFullyQualifiedName()),
				new("HandlerType", type.GetFullyQualifiedName())
			]);

			var diagnostic = Diagnostic.Create(DiagnosticDescriptors.HandlerAddPartialPlatformClass, location, properties, platform.Identifier);
			context.ReportDiagnostic(diagnostic);
		}
	}

	private static string SerializeProperties(INamedTypeSymbol viewType, INamedTypeSymbol handlerPropertyAttributeType)
	{
		var sb = new StringBuilder(";");

		foreach (var member in viewType.GetMembers().Where(v => v.Kind == SymbolKind.Property).Cast<IPropertySymbol>())
		{
			var attrs = member.GetAttributes();
			if (attrs.Any(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, handlerPropertyAttributeType)))
				sb.Append(member.Name).Append(';');
		}

		return sb.ToString(1, sb.Length - 1);
	}
}
