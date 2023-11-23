namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal interface ISyntaxGenerator
{
	INamedTypeSymbol DeclaringType { get; }

	void GenerateMembers(ICollection<MemberDeclarationSyntax> members);
}