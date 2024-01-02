namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public sealed record MethodSignature(string MethodName, SyntaxTokenList Modifiers, ImmutableArray<SyntaxToken> ParameterNames)
{
	public MethodSignature(IMethodSymbol method) : this(method.Name, method.GetModifiers(), method.Parameters.Select(v => v.Name))
	{
	}

	public MethodSignature(string methodName, SyntaxTokenList modifiers, params string[] parameters) : this(methodName, modifiers, (IEnumerable<string>)parameters)
	{
	}

	public MethodSignature(string methodName, SyntaxTokenList modifiers, IEnumerable<string> parameters) : this(methodName, modifiers, parameters.Select(SyntaxFactory.Identifier).ToImmutableArray())
	{
	}

	public MethodDeclarationSyntax BuildMethod(TypeSyntax returnType, IReadOnlyList<TypeSyntax> parameterTypes)
		=> BuildMethod(returnType, parameterTypes, Modifiers.Add(Keywords.Partial), null, null, SyntaxHelper.Semicolon);

	public MethodDeclarationSyntax BuildMethod(TypeSyntax returnType, IReadOnlyList<TypeSyntax> parameterTypes, ArrowExpressionClauseSyntax expressionBody)
		=> BuildMethod(returnType, parameterTypes, Modifiers, null, expressionBody, SyntaxHelper.Semicolon);

	public MethodDeclarationSyntax BuildMethod(TypeSyntax returnType, IReadOnlyList<TypeSyntax> parameterTypes, BlockSyntax body)
		=> BuildMethod(returnType, parameterTypes, Modifiers, body, null, default);

	private MethodDeclarationSyntax BuildMethod(TypeSyntax returnType, IReadOnlyList<TypeSyntax> parameterTypes, SyntaxTokenList modifiers, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolon)
	{
		if (parameterTypes.Count != ParameterNames.Length)
			throw new ArgumentException($"Expected {parameterTypes.Count} parameter(s), but signature has {ParameterNames.Length}.");

		var parameters = new ParameterSyntax[ParameterNames.Length];

		for (var i = 0; i < parameters.Length; i++)
			parameters[i] = SyntaxFactory.Parameter(default, default, parameterTypes[i], ParameterNames[i], null);

		var list = SyntaxFactory.SeparatedList(parameters);
		var parameterList = SyntaxFactory.ParameterList(list);
		var identifier = SyntaxFactory.Identifier(MethodName);
		return SyntaxFactory.MethodDeclaration(default, modifiers, returnType, null, identifier, null, parameterList, default, body, expressionBody, semicolon);
	}
}