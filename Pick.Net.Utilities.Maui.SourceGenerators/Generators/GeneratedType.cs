namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record GeneratedType<T>(INamedTypeSymbol DeclaringType, T Properties) where T : class;