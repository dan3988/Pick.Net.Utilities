namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct CreateResult(SyntaxReference? Owner, INamedTypeSymbol DeclaringType, BindablePropertySyntaxGenerator? Result, ImmutableArray<Diagnostic> Diagnostics = default)
{
	public CreateResult(SyntaxReference? owner, INamedTypeSymbol declaringType, Diagnostic error) : this(owner, declaringType, null, ImmutableArray.Create(error))
	{
	}

	public CreateResult(SyntaxReference? owner, INamedTypeSymbol declaringType, ImmutableArray<Diagnostic>.Builder diagnostics) : this(owner, declaringType, null, diagnostics.ToImmutable())
	{
	}

	public CreateResult(SyntaxReference? owner, INamedTypeSymbol declaringType, BindablePropertySyntaxGenerator? result, ImmutableArray<Diagnostic>.Builder diagnostics) : this(owner, declaringType, result, diagnostics.ToImmutable())
	{
	}
}