namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

[Generator]
public sealed class HandlerMapperGenerator : BaseCodeGenerator<HandlerPropertySyntaxGenerator>
{
	private protected override Type AttributeType => typeof(HandlerPropertyAttribute);

	private protected override Result<HandlerPropertySyntaxGenerator> TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var symbol = (IPropertySymbol)context.TargetSymbol;
		var generator = new HandlerPropertySyntaxGenerator(symbol.Name);
		return new(generator);
	}

	private protected override string GetTypeName(INamedTypeSymbol type)
		=> type.Name + "Handler";

	private protected override TypeDeclarationSyntax AddMembers(TypeDeclarationSyntax declaration, ClassInfo classInfo, INamedTypeSymbol declaringType, IReadOnlyList<HandlerPropertySyntaxGenerator> values)
	{
		var initializers = new List<AssignmentExpressionSyntax>();
		var methods = new List<MethodDeclarationSyntax>();
		var handlerType = classInfo.IdentifierName();
		var viewType = declaringType.ToIdentifier();

		foreach (var property in values)
		{
			var method = property.CreateMethod(handlerType, viewType);
			var identifier = IdentifierName(method.Identifier);

			methods.Add(method);

			var key = Argument(SyntaxHelper.Literal(property.Name));
			var indexArg = BracketedArgumentList(SingletonSeparatedList(key));
			var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, ImplicitElementAccess(indexArg), identifier);

			initializers.Add(assignment);
		}

		var typeArguments = TypeArgumentList(SeparatedList([viewType, handlerType]));
		var mapperInterface = GenericName(Identifiers.IPropertyMapper, typeArguments);
		var mapperClass = GenericName(Identifiers.PropertyMapper, typeArguments);
		var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList<ExpressionSyntax>(initializers));
		var val = ObjectCreationExpression(mapperClass, null, initializer);
		var variableDeclarator = VariableDeclarator(Identifier("GeneratedMapper"), null, EqualsValueClause(val));
		var variableDeclaration = VariableDeclaration(mapperInterface, SingletonSeparatedList(variableDeclarator));
		var field = FieldDeclaration(default, ModifierLists.PrivateStaticReadOnly, variableDeclaration, SyntaxHelper.Semicolon);
		var members = Enumerable.Repeat<MemberDeclarationSyntax>(field, 1).Concat(methods).ToArray();
		return declaration.AddMembers(members);
	}
}

