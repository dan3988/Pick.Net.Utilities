using System.Collections.Immutable;

using Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed record BindablePropertyGeneratorEntry(ClassInfo ClassInfo, ImmutableArray<BindablePropertySyntaxGenerator> Properties, ImmutableArray<Diagnostic> Diagnostics);