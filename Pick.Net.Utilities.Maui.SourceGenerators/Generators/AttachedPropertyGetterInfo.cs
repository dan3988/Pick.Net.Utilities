namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertyGetterInfo(string MethodName, SyntaxToken ObjectParamName, SyntaxTokenList Modifiers)
{
	public AttachedPropertyGetterInfo(string methodName, ParameterListSyntax parameters, SyntaxTokenList modifier) : this(methodName, parameters.Parameters[0].Identifier, modifier)
	{
	}
}
