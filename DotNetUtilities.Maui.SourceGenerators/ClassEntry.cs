using System.Collections.Immutable;

namespace DotNetUtilities.Maui.SourceGenerators;

internal record ClassEntry(string Namespace, string TypeName, string FullName, string FileName, ImmutableArray<BindablePropertyEntry> Properties);
