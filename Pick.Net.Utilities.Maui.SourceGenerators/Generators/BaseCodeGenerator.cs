using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

public abstract class BaseCodeGenerator<T> : IIncrementalGenerator
	where T : class
{
	private protected abstract IncrementalValueProvider<GeneratorOutput<T>> Register(SyntaxValueProvider provider);

	private protected abstract TypeDeclarationSyntax AddMembers(TypeDeclarationSyntax declaration, ClassInfo classInfo, INamedTypeSymbol type, T value);

	private protected virtual string GetTypeName(INamedTypeSymbol type)
		=> type.Name;

	private void GenerateOutput(SourceProductionContext context, GeneratorOutput<T> generationOutput)
	{
		foreach (var diagnostic in generationOutput.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		foreach (var (type, value) in generationOutput.Types)
		{
			var typeName = GetTypeName(type);
			var classInfo = ClassInfo.Create(type, typeName);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, typeName).WithModifiers(ModifierLists.Partial);

			declaration = AddMembers(declaration, classInfo, type, value);
			declaration = classInfo.ParentTypes.Aggregate(declaration, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).WithModifiers(ModifierLists.Partial).AddMembers(current));

			var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
			var root = classInfo.Namespace == "" ? (MemberDeclarationSyntax)declaration : NamespaceDeclaration(IdentifierName(classInfo.Namespace)).AddMembers(declaration);
			var unit = CompilationUnit()
				.AddMembers(root)
				.WithLeadingTrivia(Trivia(nullableEnable))
				.AddFormatting();

			var fileName = classInfo.GetFileName();
			context.AddSource(fileName, unit, Encoding.UTF8);
			context.CancellationToken.ThrowIfCancellationRequested();
		}
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var values = Register(context.SyntaxProvider);
		context.RegisterSourceOutput(values, GenerateOutput);
	}
}
