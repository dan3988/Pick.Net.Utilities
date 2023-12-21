using System.Reflection;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class AssemblyInfo
{
	public static readonly Assembly Assembly = typeof(AssemblyInfo).Assembly;
	public static readonly AssemblyName AssemblyName = Assembly.GetName();
	public static readonly string FullName = AssemblyName.FullName;
	public static readonly string Version = AssemblyName.Version.ToString();
}
