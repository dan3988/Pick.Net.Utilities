namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

public class HandlerPropertySyntaxGenerator(string name)
{
	private static readonly SyntaxToken IdentifierHandler = Identifier("handler");
	private static readonly SyntaxToken IdentifierView = Identifier("view");

	public string Name { get; } = name;

	public MethodDeclarationSyntax CreateMethod(TypeSyntax handlerType, TypeSyntax viewType)
	{
		var identifier = Identifier("Map" + Name);
		var parameters = SeparatedList(
		[
			Parameter(default, default, handlerType, IdentifierHandler, null),
			Parameter(default, default, viewType, IdentifierView, null)
		]);

		return MethodDeclaration(default, ModifierLists.PrivateStaticPartial, SyntaxHelper.TypeVoid, null, identifier, null, ParameterList(parameters), default, null, null, SyntaxHelper.Semicolon);
	}
}
