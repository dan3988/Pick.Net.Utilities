using System.Collections.Immutable;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct CreateResult(BindablePropertySyntaxGenerator? Result, ImmutableArray<Diagnostic> Diagnostics = default)
{
	public CreateResult(Diagnostic error) : this(null, ImmutableArray.Create(error))
	{
	}
}