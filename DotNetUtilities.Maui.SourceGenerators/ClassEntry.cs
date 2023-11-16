using System.Collections.Immutable;
using System.Text;

using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal record ClassEntry(string Namespace, string TypeName, ImmutableArray<string> ParentTypes, ImmutableArray<BindablePropertySyntaxGenerator> Properties)
{
	public string GetFileName()
	{
		var sb = new StringBuilder(Namespace);

		var types = ParentTypes;
		for (var i = types.Length; --i >= 0;)
			sb.Append(types[i]).Append('+');

		return sb.Append(TypeName).Append(".g.cs").ToString();
	}
}
