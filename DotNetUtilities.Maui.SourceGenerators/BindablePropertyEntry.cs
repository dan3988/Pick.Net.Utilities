using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal record BindablePropertyEntry(string PropertyName, IdentifierNameSyntax PropertyType, ExpressionSyntax DefaultModeExpression, SyntaxTokenList GetModifiers, SyntaxTokenList SetModifiers, string? AttachedType);
