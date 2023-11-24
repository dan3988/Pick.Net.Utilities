namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record PropertyCollection(INamedTypeSymbol DeclaringType, IReadOnlyCollection<ISyntaxGenerator> Properties);