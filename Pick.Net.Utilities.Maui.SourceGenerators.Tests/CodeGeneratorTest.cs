using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTest<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	private static readonly MSTestVerifier Verifier = new();
	private static readonly AssemblyName AssemblyName = typeof(TGenerator).Assembly.GetName();
	private static readonly string FileNamePrefix = AssemblyName.Name + "\\" + typeof(TGenerator) + "\\";

	protected void AssertCodeEqual(string expected, string actual, string? message = null)
		=> Verifier.EqualOrDiff(expected, actual, message);

	protected void CheckGeneratedCode(Compilation compilation, string fullTypeName, string expectedCode)
	{
		var fileName = FileNamePrefix + fullTypeName + ".g.cs";
		var file = compilation.SyntaxTrees.FirstOrDefault(v => v.FilePath == fileName);
		Assert.IsNotNull(file, "Generated file \"{0}\" was not found in compilation", fileName);
		Verifier.EqualOrDiff(expectedCode, file.ToString());
	}

	protected void CheckDiagnostics(ImmutableArray<Diagnostic> diagnostics)
	{
		Verifier.Empty("Diagnostics", diagnostics);
	}

	protected CSharpCompilation CreateCompilation(string code, out CSharpSyntaxTree tree, [CallerMemberName] string test = null!)
	{
		var type = GetType();
		var fullName = $"{type.Namespace}\\{type.Name}\\{test}";
		return TestHelper.CreateCompilation(code, type.Namespace!, fullName + ".cs", out tree);
	}

	protected CSharpGeneratorDriver CreateDriver(CSharpParseOptions options)
	{
		return (CSharpGeneratorDriver)CSharpGeneratorDriver.Create(new TGenerator()).WithUpdatedParseOptions(options);
	}
}
