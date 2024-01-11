using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

using static SyntaxFactory;

internal static class SyntaxGeneratorExtensions
{
	public static SyntaxNode GlobalType(this SyntaxGenerator generator, string name)
	{
		var split = name.Split('.');
		if (split[0] == "")
			goto Invalid;

		var identifier = generator.IdentifierName(split[0]);
		var node = (SyntaxNode)AliasQualifiedName(IdentifierName(Keywords.Global), (IdentifierNameSyntax)identifier);

		for (var i = 1; i < split.Length; i++)
		{
			var part = split[i];
			if (part == "")
				goto Invalid;

			identifier = generator.IdentifierName(part);
			node = generator.QualifiedName(node, identifier);
		}

		return node;

	Invalid:
		throw new ArgumentException($"Invalid type identifier: {name}", nameof(name));
	}
}
