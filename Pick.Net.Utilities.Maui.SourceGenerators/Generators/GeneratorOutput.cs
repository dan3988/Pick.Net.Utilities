namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal abstract record GeneratorOutput(ImmutableArray<Diagnostic> Diagnostics)
{
	internal static GeneratorOutput<T>.Builder CreateBuilder<T>() where T : class
		=> new();
}

internal sealed record GeneratorOutput<T>(ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<GeneratedType<T>> Types) : GeneratorOutput(Diagnostics)
	where T : class
{
	internal sealed class Builder
	{
		private readonly ImmutableArray<Diagnostic>.Builder _diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		private readonly ImmutableArray<GeneratedType<T>>.Builder _types = ImmutableArray.CreateBuilder<GeneratedType<T>>();

		public void AddDiagnostic(Diagnostic diagnostic)
			=> _diagnostics.Add(diagnostic);

		public void AddType(GeneratedType<T> type)
			=> _types.Add(type);

		public GeneratorOutput<T> Build()
			=> new(_diagnostics.ToImmutable(), _types.ToImmutable());
	}
}
