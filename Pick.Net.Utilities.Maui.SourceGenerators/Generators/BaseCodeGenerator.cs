using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

public abstract class BaseCodeGenerator<T> : IIncrementalGenerator
	 where T : class, IMemberGenerator
{
	private readonly record struct ResultAndType(INamedTypeSymbol Owner, Result<T> Result);

	private protected abstract Type AttributeType { get; }

	private ResultAndType TransformMemberInternal(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var result = TransformMember(context, token);
		return new(context.TargetSymbol.ContainingType, result);
	}

	private protected abstract Result<T> TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token);

	private GenerationOutput GroupGenerators(ImmutableArray<ResultAndType> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<GeneratedType>();
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, List<T>>(SymbolEqualityComparer.Default);

		foreach (var (declaringType, result) in values)
		{
			if (!result.IsSuccessful(out var value, out var error))
			{
				diagnostics.Add(error);
				continue;
			}

			if (!map.TryGetValue(declaringType, out var properties))
			{
				map[declaringType] = properties = [];
				types.Add(new(declaringType, properties));
			}

			properties.Add(value);
		}

		return new(diagnostics.ToImmutable(), types.ToImmutable());
	}

	private void GenerateOutput(SourceProductionContext context, GenerationOutput generationOutput)
	{
		foreach (var diagnostic in generationOutput.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		foreach (var type in generationOutput.Types)
		{
			var classInfo = ClassInfo.Create(type.DeclaringType);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, classInfo.TypeName).WithModifiers(ModifierLists.Partial);
			var members = new List<MemberDeclarationSyntax>();

			foreach (var generator in type.Properties)
				generator.GenerateMembers(members);

			declaration = declaration.AddMembers([.. members]);
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
		var info = context.SyntaxProvider.ForAttributeWithMetadataType(AttributeType, TransformMemberInternal);
		var collected = info.Collect().Select(GroupGenerators);
		context.RegisterSourceOutput(collected, GenerateOutput);
	}
}
