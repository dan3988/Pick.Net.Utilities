using System.Reflection;

using Pick.Net.Utilities.Maui;

using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

[assembly: XmlnsPrefix(AssemblyInfo.NamespaceUri, "dp")]
[assembly: XmlnsDefinition(AssemblyInfo.NamespaceUri, AssemblyInfo.NamespacePrefix + nameof(Pick.Net.Utilities.Maui.MarkupExtensions))]

namespace Pick.Net.Utilities.Maui;

public static class AssemblyInfo
{
	public const string NamespaceUri = "http://dpickett.co.uk/maui-extensions";
	internal const string Namespace = "Pick.Net.Utilities.Maui";
	internal const string NamespacePrefix = Namespace + ".";

	public static readonly Module Module = typeof(AssemblyInfo).Module;
	public static readonly Assembly Assembly = Module.Assembly;

	public static MauiAppBuilder UseMobileLib(this MauiAppBuilder builder)
	{
		return builder;
	}
}
