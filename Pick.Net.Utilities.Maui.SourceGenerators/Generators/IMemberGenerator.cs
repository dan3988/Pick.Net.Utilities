namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public interface IMemberGenerator
{
	void GenerateMembers(ICollection<MemberDeclarationSyntax> type);
}