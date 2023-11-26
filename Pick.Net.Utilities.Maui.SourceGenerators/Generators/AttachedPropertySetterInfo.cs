namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertySetterInfo(SyntaxToken ObjectParamName, SyntaxToken ValueParamName, SyntaxTokenList Modifiers)
{
	public AttachedPropertySetterInfo(string objectParamName, string valueParamName, SyntaxTokenList modifier) : this(SyntaxFactory.Identifier(objectParamName), SyntaxFactory.Identifier(valueParamName), modifier)
	{
	}

	public AttachedPropertySetterInfo(ParameterListSyntax parameters, SyntaxTokenList modifier) : this(parameters.Parameters[0].Identifier, parameters.Parameters[1].Identifier, modifier)
	{
	}
}
