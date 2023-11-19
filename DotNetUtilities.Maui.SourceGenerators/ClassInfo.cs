using System.Collections.Immutable;
using System.Text;

namespace DotNetUtilities.Maui.SourceGenerators;

internal sealed class ClassInfo
{
	public static ClassInfo Create(INamedTypeSymbol type, string? fileNameSuffix = null)
	{
		var parentTypes = type.GetContainingTypeNames();
		var ns = type.ContainingNamespace.GetFullName();
		var fileName = GetFileName(ns, type.Name, parentTypes, fileNameSuffix);
		return new(ns, type.Name, fileName, parentTypes);
	}

	private static string GetFileName(string @namespace, string typeName, ImmutableArray<string> parentTypes, string? suffix)
	{
		var sb = new StringBuilder(@namespace).Append('.');

		for (var i = parentTypes.Length; --i >= 0;)
			sb.Append(parentTypes[i]).Append('+');

		sb.Append(typeName);

		if (suffix != null)
			sb.Append('.').Append(suffix);

		return sb.Append(".g.cs").ToString();
	}

	public readonly string Namespace;
	public readonly string TypeName;
	public readonly string FileName;
	public readonly ImmutableArray<string> ParentTypes;

	private ClassInfo(string @namespace, string typeName, string fileName, ImmutableArray<string> parentTypes)
	{
		Namespace = @namespace;
		TypeName = typeName;
		FileName = fileName;
		ParentTypes = parentTypes;
	}

	public void Deconstruct(out string @namespace, out string typeName, out string fileName, out ImmutableArray<string> parentTypes)
	{
		@namespace = Namespace;
		typeName = TypeName;
		fileName = FileName;
		parentTypes = ParentTypes;
	}
}