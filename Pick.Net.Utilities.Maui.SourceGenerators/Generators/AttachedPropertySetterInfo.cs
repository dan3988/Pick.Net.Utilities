namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertySetterInfo(SyntaxToken ObjectParamName, SyntaxToken ValueParamName, SyntaxTokenList Modifiers)
{
	public AttachedPropertySetterInfo(ParameterListSyntax parameters, SyntaxTokenList modifier) : this(parameters.Parameters[0].Identifier, parameters.Parameters[1].Identifier, modifier)
	{
	}
}
