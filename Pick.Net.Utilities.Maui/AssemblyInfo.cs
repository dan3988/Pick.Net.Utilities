using System.Reflection;
using System.Runtime.CompilerServices;

using Pick.Net.Utilities.Maui;

using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

[assembly: XmlnsPrefix(AssemblyInfo.NamespaceUri, "dp")]
[assembly: XmlnsDefinition(AssemblyInfo.NamespaceUri, AssemblyInfo.NamespacePrefix + nameof(Pick.Net.Utilities.Maui.MarkupExtensions))]
[assembly: InternalsVisibleTo("Pick.Net.Utilities.Maui.SourceGenerators.Tests")]

namespace Pick.Net.Utilities.Maui;

internal static class AssemblyInfo
{
	public const string NamespaceUri = "http://dpickett.co.uk/maui-extensions";
	internal const string Namespace = "Pick.Net.Utilities.Maui";
	internal const string NamespacePrefix = Namespace + ".";

	public static readonly Module Module = typeof(AssemblyInfo).Module;
	public static readonly Assembly Assembly = Module.Assembly;
}
