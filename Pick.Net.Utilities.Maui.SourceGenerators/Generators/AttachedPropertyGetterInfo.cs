namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertyGetterInfo(SyntaxToken ObjectParamName, SyntaxTokenList Modifiers)
{
	public AttachedPropertyGetterInfo(ParameterListSyntax parameters, SyntaxTokenList modifier) : this(parameters.Parameters[0].Identifier, modifier)
	{
	}
}
