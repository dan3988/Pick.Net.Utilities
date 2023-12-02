namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertySetterInfo(bool Partial, SyntaxToken ObjectParamName, SyntaxToken ValueParamName, Accessibility Accessibility)
{
	public AttachedPropertySetterInfo(IMethodSymbol symbol) : this(symbol, symbol.DeclaredAccessibility)
	{
	}

	public AttachedPropertySetterInfo(IMethodSymbol symbol, Accessibility accessibility) : this(true, symbol.Parameters[0].Name, symbol.Parameters[1].Name, accessibility)
	{
	}

	public AttachedPropertySetterInfo(bool partial, string objectParamName, string valueParamName, Accessibility accessibility) : this(partial, SyntaxFactory.Identifier(objectParamName), SyntaxFactory.Identifier(valueParamName), accessibility)
	{
	}
}
