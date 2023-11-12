using Microsoft.CodeAnalysis;

namespace DotNetUtilities.Maui.SourceGenerators;

internal record BindablePropertyEntry(string PropertyName, string PropertyType, SyntaxTokenList GetModifiers, SyntaxTokenList SetModifiers, string? AttachedType);
