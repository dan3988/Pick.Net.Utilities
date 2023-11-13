using System.Collections.Immutable;

using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal record ClassEntry(string Namespace, string TypeName, string FileName, ImmutableArray<BindablePropertySyntaxGenerator> Properties);
