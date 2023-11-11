using System.Reflection;

using DotNetUtilities.Maui;

using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

[assembly: XmlnsPrefix(AssemblyInfo.NamespaceUri, "dp")]
[assembly: XmlnsDefinition(AssemblyInfo.NamespaceUri, AssemblyInfo.NamespacePrefix + nameof(DotNetUtilities.Maui.MarkupExtensions))]

namespace DotNetUtilities.Maui;

public static class AssemblyInfo
{
	public const string NamespaceUri = "http://dpickett.co.uk/maui-extensions";
	internal const string Namespace = "DotNetUtilities.Maui";
	internal const string NamespacePrefix = Namespace + ".";

	public static readonly Module Module = typeof(AssemblyInfo).Module;
	public static readonly Assembly Assembly = Module.Assembly;

	public static MauiAppBuilder UseMobileLib(this MauiAppBuilder builder)
	{
		return builder;
	}
}
