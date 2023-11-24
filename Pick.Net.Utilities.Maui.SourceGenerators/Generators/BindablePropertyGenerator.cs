using System.Collections.Immutable;
using System.Text;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly Type AttributeType = typeof(BindablePropertyAttribute);

	private static CreateResult CreateForProperty(IPropertySymbol symbol, AttributeData attribute)
	{
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = symbol.SetMethod?.DeclaredAccessibility ?? Accessibility.Private;
		var props = new SyntaxGeneratorSharedProperties(attribute.ApplicationSyntaxReference, symbol.ContainingType, symbol.Name, symbol.Type, accessibility, writeAccessibility, BindingMode.OneWay, null, false, false, false);
		var generator = new BindableInstancePropertySyntaxGenerator(props);
		return new(generator);
	}

	private static CreateResult CreateForMethod(IMethodSymbol symbol, AttributeData attribute)
	{
		var name = symbol.Name;
		if (!name.StartsWith("Get"))
		{
			var diagnostic = DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName.CreateDiagnostic(attribute.ApplicationSyntaxReference, name);
			return new(diagnostic);
		}

		if (symbol.ReturnsVoid)
		{
			var diagnostic = DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn.CreateDiagnostic(attribute.ApplicationSyntaxReference);
			return new(diagnostic);
		}

		var arguments = symbol.Parameters;
		if (arguments.Length != 1)
		{
			var diagnostic = DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature.CreateDiagnostic(attribute.ApplicationSyntaxReference);
			return new(diagnostic);
		}

		name = name.Substring(3);

		var accessibility = symbol.DeclaredAccessibility;
		var props = new SyntaxGeneratorSharedProperties(attribute.ApplicationSyntaxReference, symbol.ContainingType, name, symbol.ReturnType, accessibility, accessibility, BindingMode.OneWay, null, false, false, false);
		var generator = new BindableAttachedPropertySyntaxGenerator(props, arguments[0].Type);
		return new(generator);
	}

	private static CreateResult TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return context.TargetSymbol.Kind switch
		{
			SymbolKind.Property => CreateForProperty((IPropertySymbol)context.TargetSymbol, context.Attributes[0]),
			SymbolKind.Method => CreateForMethod((IMethodSymbol)context.TargetSymbol, context.Attributes[0]),
			_ => throw new InvalidOperationException("Unexpected syntax node: " + context.TargetSymbol.Kind)
		};
	}

	private static GenerationOutput GroupGenerators(ImmutableArray<CreateResult> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<PropertyCollection>();
		var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, Dictionary<string, BindablePropertySyntaxGenerator>>(SymbolEqualityComparer.Default);

		//foreach (var value in values)
		foreach (var (generator, diagnostics) in values)
		{
			if (!diagnostics.IsDefaultOrEmpty)
				diagnosticsBuilder.AddRange(diagnostics);
			
			if (generator == null)
				continue;

			if (!map.TryGetValue(generator.DeclaringType, out var namedProperties))
			{
				map[generator.DeclaringType] = namedProperties = new();
				types.Add(new(generator.DeclaringType, namedProperties.Values));
			}
			else if (namedProperties.ContainsKey(generator.PropertyName))
			{
				diagnosticsBuilder.Add(DiagnosticDescriptors.BindablePropertyDuplicateName, generator.Owner);
				continue;
			}

			namedProperties[generator.PropertyName] = generator;
		}

		return new(diagnosticsBuilder.ToImmutable(), types.ToImmutable());
	}

	private static void GenerateOutput(SourceProductionContext context, GenerationOutput generationOutput)
	{
		foreach (var diagnostic in generationOutput.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		//var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		foreach (var type in generationOutput.Types)
		{
			var classInfo = ClassInfo.Create(type.DeclaringType);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, classInfo.TypeName).AddModifier(SyntaxKind.PartialKeyword);
			var members = new List<MemberDeclarationSyntax>();

			foreach (var generator in type.Properties)
				generator.GenerateMembers(members);

			declaration = declaration.AddMembers(members.ToArray());
			declaration = classInfo.ParentTypes.Aggregate(declaration, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).AddModifier(SyntaxKind.PartialKeyword).AddMembers(current));

			var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
			var ns = NamespaceDeclaration(IdentifierName(classInfo.Namespace)).AddMembers(declaration);
			var unit = CompilationUnit()
				.AddMembers(ns)
				.WithLeadingTrivia(Trivia(nullableEnable))
				.AddFormatting();

			var fileName = classInfo.GetFileName();
			context.AddSource(fileName, unit, Encoding.UTF8);
			context.CancellationToken.ThrowIfCancellationRequested();
		}
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataType(AttributeType, TransformMember);
		var collected = info.Collect().Select(GroupGenerators);
		context.RegisterSourceOutput(collected, GenerateOutput);
	}
}