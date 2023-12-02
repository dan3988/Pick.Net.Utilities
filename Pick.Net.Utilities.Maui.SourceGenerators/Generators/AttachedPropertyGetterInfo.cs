namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record AttachedPropertyGetterInfo(string MethodName, SyntaxToken ObjectParamName, Accessibility Accessibility)
{
	public AttachedPropertyGetterInfo(IMethodSymbol method) : this(method.Name, SyntaxFactory.Identifier(method.Parameters[0].Name), method.DeclaredAccessibility)
	{
	}
}
