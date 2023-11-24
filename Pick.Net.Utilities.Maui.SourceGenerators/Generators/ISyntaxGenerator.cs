namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal interface ISyntaxGenerator
{
	INamedTypeSymbol DeclaringType { get; }

	string PropertyName { get; }

	SyntaxReference? Owner { get; }

	void GenerateMembers(ICollection<MemberDeclarationSyntax> members);
}