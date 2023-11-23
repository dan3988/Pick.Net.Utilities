using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly Type AttributeType = typeof(BindablePropertyAttribute);

	private static void GenerateProperties(SourceProductionContext context, IGrouping<ITypeSymbol, ISyntaxGenerator> group)
	{
		var (@namespace, typeName, fileName, parentTypes) = ClassInfo.Create(group.Key);
		var type = TypeDeclaration(SyntaxKind.ClassDeclaration, typeName).AddModifier(SyntaxKind.PartialKeyword);
		var members = new List<MemberDeclarationSyntax>();

		foreach (var generator in group)
			generator.GenerateMembers(members);

		type = type.AddMembers(members.ToArray());
		type = parentTypes.Aggregate(type, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).AddModifier(SyntaxKind.PartialKeyword).AddMembers(current));

		var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
		var ns = NamespaceDeclaration(IdentifierName(@namespace)).AddMembers(type);
		var unit = CompilationUnit()
			.AddMembers(ns)
			.WithLeadingTrivia(Trivia(nullableEnable))
			.AddFormatting();

		context.AddSource(fileName, unit);
#if DEBUG
		var text = unit.ToFullString();
		Console.WriteLine(fileName);
		Console.WriteLine(text);
#endif
	}

	private static ISyntaxGenerator Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var prop = (IPropertySymbol)context.TargetSymbol;
		return new InstancePropertySyntaxGenerator(prop.ContainingType, prop);
	}

	private static bool Predicate(SyntaxNode node, CancellationToken token)
		=> node.IsKind(SyntaxKind.PropertyDeclaration);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataName(AttributeType.FullName!, Predicate, Transform);
		var grouped = info.GroupBy(v => v.DeclaringType);
		context.RegisterSourceOutput(grouped, GenerateProperties);
	}
}