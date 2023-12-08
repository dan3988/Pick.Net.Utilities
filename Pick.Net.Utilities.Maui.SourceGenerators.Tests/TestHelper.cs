using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal static class TestHelper
{
	public static class Assemblies
	{
		public static readonly Assembly System = typeof(object).Assembly;
		public static readonly Assembly Maui = typeof(Microsoft.Maui.Controls.BindableObject).Assembly;
		public static readonly Assembly Utilities = typeof(BooleanBox).Assembly;
		public static readonly Assembly UtilitiesMaui = AssemblyInfo.Assembly;
		public static readonly Assembly UtilitiesMauiSourceGenerators = typeof(BindablePropertyGenerator).Assembly;
		public static readonly Assembly This = typeof(TestHelper).Assembly;
	}

	public static readonly ReferenceAssemblies Net70 = new("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), Path.Combine("ref", "net7.0"));
	public static readonly ReferenceAssemblies Net80 = new("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "net8.0"));

	public static void SetUpReferences<TAnalyzer>(this AnalyzerTest<TAnalyzer> test, bool addAnalyzer = true)
		where TAnalyzer : IVerifier, new()
	{
		if (addAnalyzer)
			test.SolutionTransforms.Add(AddAnalyzerToSolution);

		test.TestState.AdditionalReferences.Add(Assemblies.Maui);
		test.TestState.AdditionalReferences.Add(Assemblies.UtilitiesMaui);
		test.ReferenceAssemblies = Net80;
	}

	public static void AddAnalyzerToSolution<T>(this AnalyzerTest<T> test) where T : IVerifier, new()
		=> AddAnalyzerToSolution(test.SolutionTransforms);

	public static void AddAnalyzerToSolution(this List<Func<Solution, ProjectId, Solution>> transforms)
		=> transforms.Add(AddAnalyzerToSolution);

	public static Solution AddAnalyzerToSolution(Solution solution, ProjectId id)
	{
		var reference = new AnalyzerFileReference(Assemblies.UtilitiesMauiSourceGenerators.Location, AnalyzerAssemblyLoader.Instance);
		return solution.WithProjectAnalyzerReferences(id, [reference]);
	}

	public static CSharpCompilation CreateCompilation(string code, string assemblyName, string filePath, out CSharpSyntaxTree tree)
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);
		tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code, options, filePath);
		return CreateCompilation(tree, assemblyName);
	}

	public static CSharpCompilation CreateCompilation(CSharpSyntaxTree tree, string assemblyName)
	{
		var mscorlib = Path.GetDirectoryName(Assemblies.System.Location);
		var references = new MetadataReference[]
		{
			MetadataReference.CreateFromFile(Path.Join(mscorlib, "mscorlib.dll")),
			MetadataReference.CreateFromFile(Path.Join(mscorlib, "System.dll")),
			MetadataReference.CreateFromFile(Path.Join(mscorlib, "System.Core.dll")),
			MetadataReference.CreateFromFile(Path.Join(mscorlib, "System.Runtime.dll")),
			MetadataReference.CreateFromFile(Path.Join(mscorlib, "System.Private.CoreLib.dll")),
			MetadataReference.CreateFromFile(Assemblies.Maui.Location),
			MetadataReference.CreateFromFile(Assemblies.UtilitiesMaui.Location)
		};

		return CSharpCompilation.Create(assemblyName, [tree], references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
	}

	private sealed class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
	{
		public static IAnalyzerAssemblyLoader Instance = new AnalyzerAssemblyLoader();

		private AnalyzerAssemblyLoader() { }

		public void AddDependencyLocation(string fullPath)
		{
		}

		public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
	}
}
