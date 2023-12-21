using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

public abstract class BaseCodeGenerator<T> : IIncrementalGenerator
	where T : class
{
	private readonly record struct ResultAndType(INamedTypeSymbol Owner, Result<T> Result);

	private protected abstract Type AttributeType { get; }

	private ResultAndType TransformMemberInternal(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var result = TransformMember(context, token);
		return new(context.TargetSymbol.ContainingType, result);
	}

	private protected abstract Result<T> TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token);

	private protected abstract TypeDeclarationSyntax AddMembers(TypeDeclarationSyntax declaration, ClassInfo classInfo, INamedTypeSymbol type, IReadOnlyList<T> values);

	private protected virtual string GetTypeName(INamedTypeSymbol type)
		=> type.Name;

	private GeneratorOutput<T> GroupGenerators(ImmutableArray<ResultAndType> values, CancellationToken token)
	{
		var builder = GeneratorOutput.CreateBuilder<T>();
		var map = new Dictionary<INamedTypeSymbol, List<T>>(SymbolEqualityComparer.Default);

		foreach (var (declaringType, result) in values)
		{
			if (!result.IsSuccessful(out var value, out var error))
			{
				builder.AddDiagnostic(error);
				continue;
			}

			if (!map.TryGetValue(declaringType, out var properties))
			{
				map[declaringType] = properties = [];
				builder.AddType(new(declaringType, properties));
			}

			properties.Add(value);
		}

		return builder.Build();
	}

	private void GenerateOutput(SourceProductionContext context, GeneratorOutput<T> generationOutput)
	{
		foreach (var diagnostic in generationOutput.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		foreach (var (type, values) in generationOutput.Types)
		{
			var typeName = GetTypeName(type);
			var classInfo = ClassInfo.Create(type, typeName);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, typeName).WithModifiers(ModifierLists.Partial);

			declaration = AddMembers(declaration, classInfo, type, values);
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
