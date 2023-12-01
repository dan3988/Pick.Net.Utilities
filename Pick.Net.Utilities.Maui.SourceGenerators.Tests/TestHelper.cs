using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Maui.Controls;

using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal static class TestHelper
{
	public static readonly ReferenceAssemblies Net70 = new("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), Path.Combine("ref", "net7.0"));
	public static readonly ReferenceAssemblies Net80 = new("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "net8.0"));
	public static readonly Assembly MauiAssembly = typeof(BindableObject).Assembly;
	public static readonly Assembly UtilitiesAssembly = typeof(BooleanBox).Assembly;
	public static readonly Assembly UtilitiesMauiAssembly = AssemblyInfo.Assembly;
	public static readonly Assembly UtilitiesMauiSourceGeneratorsAssembly = typeof(BindablePropertyGenerator).Assembly;
	public static readonly Assembly ThisAssembly = typeof(TestHelper).Assembly;

	public static void SetUpReferences<TAnalyzer>(this AnalyzerTest<TAnalyzer> test)
		where TAnalyzer : IVerifier, new()
	{
		test.SolutionTransforms.Add(AddAnalyzerToSolution);
		test.TestState.AdditionalReferences.Add(MauiAssembly);
		test.TestState.AdditionalReferences.Add(UtilitiesMauiAssembly);
		test.ReferenceAssemblies = Net80;
	}

	public static void AddAnalyzerToSolution<T>(this AnalyzerTest<T> test) where T : IVerifier, new()
		=> AddAnalyzerToSolution(test.SolutionTransforms);

	public static void AddAnalyzerToSolution(this List<Func<Solution, ProjectId, Solution>> transforms)
		=> transforms.Add(AddAnalyzerToSolution);

	public static Solution AddAnalyzerToSolution(Solution solution, ProjectId id)
	{
		var reference = new AnalyzerFileReference(UtilitiesMauiSourceGeneratorsAssembly.Location, AnalyzerAssemblyLoader.Instance);
		return solution.WithProjectAnalyzerReferences(id, [reference]);
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
