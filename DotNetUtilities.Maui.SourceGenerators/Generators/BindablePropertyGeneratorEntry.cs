using System.Collections.Immutable;

using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators.Generators;

internal sealed record BindablePropertyGeneratorEntry(ClassInfo ClassInfo, ImmutableArray<BindablePropertySyntaxGenerator> Properties, ImmutableArray<Diagnostic> Diagnostics);