namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record GenerationOutput(ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<GeneratedType> Types);
