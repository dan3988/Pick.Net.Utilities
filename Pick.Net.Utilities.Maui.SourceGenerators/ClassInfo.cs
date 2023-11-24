using System.Collections.Immutable;
using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal sealed record ClassInfo(string Namespace, string TypeName, ImmutableArray<string> ParentTypes)
{
	public static ClassInfo Create(ITypeSymbol type)
	{
		var parentTypes = type.GetContainingTypeNames();
		var ns = type.ContainingNamespace.GetFullName();
		return new(ns, type.Name, parentTypes);
	}

	public string GetFileName(string? suffix = null)
	{
		var sb = new StringBuilder(Namespace).Append('.');

		for (var i = ParentTypes.Length; --i >= 0;)
			sb.Append(ParentTypes[i]).Append('+');

		sb.Append(TypeName);

		if (suffix != null)
			sb.Append('.').Append(suffix);

		return sb.Append(".g.cs").ToString();
	}
}