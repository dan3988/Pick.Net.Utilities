using System.Reflection;

using Microsoft.CodeAnalysis.Testing;
using Microsoft.Maui.Controls;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal static class TestHelper
{
	public static readonly ReferenceAssemblies Net70 = new("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), Path.Combine("ref", "net7.0"));
	public static readonly ReferenceAssemblies Net80 = new("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "net8.0"));
	public static readonly Assembly MauiAssembly = typeof(BindableObject).Assembly;
	public static readonly Assembly UtilitiesAssembly = typeof(BooleanBox).Assembly;
	public static readonly Assembly UtilitiesMauiAssembly = AssemblyInfo.Assembly;
	public static readonly Assembly ThisAssembly = typeof(TestHelper).Assembly;
}
