using Microsoft.CodeAnalysis;

namespace CodeGeneration.SourceGenerators;

internal record BindablePropertyEntry(string PropertyName, string PropertyType, SyntaxTokenList GetModifiers, SyntaxTokenList SetModifiers);
