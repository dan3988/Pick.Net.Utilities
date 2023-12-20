namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record GeneratorOutput(ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<GeneratedType> Types)
{
	internal static Builder CreateBuilder() => new();

	internal sealed class Builder
	{
		private readonly ImmutableArray<Diagnostic>.Builder _diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		private readonly ImmutableArray<GeneratedType>.Builder _types = ImmutableArray.CreateBuilder<GeneratedType>();

		public void AddDiagnostic(Diagnostic diagnostic)
			=> _diagnostics.Add(diagnostic);

		public void AddType(GeneratedType type)
			=> _types.Add(type);

		public GeneratorOutput Build()
			=> new(_diagnostics.ToImmutable(), _types.ToImmutable());
	}
}
