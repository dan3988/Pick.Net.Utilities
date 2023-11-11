namespace DotNetUtilities.Maui.SourceGenerators;

internal enum PropertyAccessLevel
{
	Public = 0,
	Protected = 1,
	Internal = 2,
	Private = 4,
	ProtectedPrivate = Protected | Private,
	ProtectedInternal = Protected | Internal,
}
