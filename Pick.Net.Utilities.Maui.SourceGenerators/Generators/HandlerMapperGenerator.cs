namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

[Generator]
public sealed class HandlerMapperGenerator : BaseCodeGenerator<HandlerMapperGeneratorResult>
{
	private static readonly Type AttributeType = typeof(HandlerMapperAttribute);

	private static FieldDeclarationSyntax CreateMapperDeclaration(TypeSyntax handlerType, TypeSyntax viewType, IEnumerable<ExpressionSyntax> initializers)
	{
		var typeArguments = TypeArgumentList(SeparatedList([viewType, handlerType]));
		var mapperInterface = GenericName(Identifiers.IPropertyMapper, typeArguments);
		var mapperClass = GenericName(Identifiers.PropertyMapper, typeArguments);
		var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList(initializers));
		var val = ObjectCreationExpression(mapperClass, null, initializer);
		var variableDeclarator = VariableDeclarator(Identifier("GeneratedMapper"), null, EqualsValueClause(val));
		var variableDeclaration = VariableDeclaration(mapperInterface, SingletonSeparatedList(variableDeclarator));
		return FieldDeclaration(default, ModifierLists.PrivateStaticReadOnly, variableDeclaration, SyntaxHelper.Semicolon);
	}

	private protected override TypeDeclarationSyntax AddMembers(TypeDeclarationSyntax declaration, ClassInfo classInfo, INamedTypeSymbol type, HandlerMapperGeneratorResult result)
	{
		var initializers = new List<AssignmentExpressionSyntax>();
		var methods = new List<MethodDeclarationSyntax>();
		var handlerType = result.HandlerType.ToIdentifier();
		var viewType = result.ViewType.ToIdentifier();

		foreach (var property in result.Properties)
		{
			var method = property.CreateMethod(handlerType, viewType);
			var identifier = IdentifierName(method.Identifier);

			methods.Add(method);

			var key = Argument(SyntaxHelper.Literal(property.Name));
			var indexArg = BracketedArgumentList(SingletonSeparatedList(key));
			var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, ImplicitElementAccess(indexArg), identifier);

			initializers.Add(assignment);
		}

		var field = CreateMapperDeclaration(handlerType, viewType, initializers);
		var members = Enumerable.Repeat<MemberDeclarationSyntax>(field, 1).Concat(methods).ToArray();
		return declaration.AddMembers(members);
	}

	private protected override IncrementalValueProvider<GeneratorOutput<HandlerMapperGeneratorResult>> Register(SyntaxValueProvider provider)
		=> provider.ForAttributeWithMetadataType(AttributeType, TransformType).Collect().Select(Collect);

	private GeneratorOutput<HandlerMapperGeneratorResult> Collect(ImmutableArray<Result<HandlerMapperGeneratorResult>> source, CancellationToken token)
	{
		var builder = GeneratorOutput.CreateBuilder<HandlerMapperGeneratorResult>();

		foreach (var result in source)
		{
			if (!result.IsSuccessful(out var value, out var error))
			{
				builder.AddDiagnostic(error);
			}
			else
			{
				builder.AddType(new(value.HandlerType, value));
			}
		}

		return builder.Build();
	}

	private Result<HandlerMapperGeneratorResult> TransformType(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var type = (INamedTypeSymbol)context.TargetSymbol;
		var attribute = context.Attributes[0];
		if (context.Attributes is not [{ ConstructorArguments: [{ Value: INamedTypeSymbol viewType }] }])
			return default;

		var sm = context.SemanticModel;
		var attributeType = sm.Compilation.GetTypeByMetadataName("Pick.Net.Utilities.Maui.HandlerPropertyAttribute");
		if (attributeType == null)
			return default;

		var properties = ImmutableArray.CreateBuilder<HandlerPropertySyntaxGenerator>();

		foreach (var member in viewType.GetMembers().Where(v => v.Kind == SymbolKind.Property).Cast<IPropertySymbol>())
		{
			var attrs = member.GetAttributes();
			if (attrs.Any(v => SymbolEqualityComparer.Default.Equals(v.AttributeClass, attributeType)))
				properties.Add(new(member.Name));
		}

		var result = new HandlerMapperGeneratorResult(type, viewType, properties.ToImmutable());
		return new(result);
	}
}

