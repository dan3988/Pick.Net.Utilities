using System.Collections.Immutable;
using System.Text;

using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators;

internal sealed record ClassEntry(string Namespace, string TypeName, ImmutableArray<string> ParentTypes, ImmutableArray<BindablePropertySyntaxGenerator> Properties, ImmutableArray<Diagnostic> Diagnostics)
{
	public string GetFileName(string? suffix = null)
	{
		var sb = new StringBuilder(Namespace).Append('.');

		var types = ParentTypes;
		for (var i = types.Length; --i >= 0;)
			sb.Append(types[i]).Append('+');

		sb.Append(TypeName);

		if (suffix != null)
			sb.Append('.').Append(suffix);

		return sb.Append(".g.cs").ToString();
	}
}
