using System.Collections.Immutable;
using System.Text;

using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal sealed record ClassEntry(string Namespace, string TypeName, string FileName, ImmutableArray<string> ParentTypes, ImmutableArray<BindablePropertySyntaxGenerator> Properties, ImmutableArray<Diagnostic> Diagnostics);