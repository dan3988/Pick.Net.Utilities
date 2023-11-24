using System.Collections.Immutable;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct CreateResult(BindablePropertySyntaxGenerator? Result, ImmutableArray<Diagnostic> Diagnostics = default)
{
	public CreateResult(Diagnostic error) : this(null, ImmutableArray.Create(error))
	{
	}

	public CreateResult(ImmutableArray<Diagnostic>.Builder diagnostics) : this(null, diagnostics.ToImmutable())
	{
	}

	public CreateResult(BindablePropertySyntaxGenerator? result, ImmutableArray<Diagnostic>.Builder diagnostics) : this(result, diagnostics.ToImmutable())
	{
	}
}