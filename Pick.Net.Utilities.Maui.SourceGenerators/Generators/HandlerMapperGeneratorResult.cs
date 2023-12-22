namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public sealed record HandlerMapperGeneratorResult(INamedTypeSymbol HandlerType, INamedTypeSymbol ViewType, ImmutableArray<HandlerPropertySyntaxGenerator> Properties);
