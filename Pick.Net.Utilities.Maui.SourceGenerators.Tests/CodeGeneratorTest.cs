using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public sealed class CodeGeneratorTest<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	private static readonly AssemblyName AssemblyName = typeof(TGenerator).Assembly.GetName();
	private static readonly string FileNamePrefix = AssemblyName.Name + "\\" + typeof(TGenerator) + "\\";

	public required Type TestsType { get; set; }

	public required string TestName { get; set; }

	public required string Input { get; set; }

	public CSharpParseOptions Options { get; set; } = CSharpParseOptions.Default;

	public List<CodeGeneratorTestOutput> OutputFiles { get; } = [];

	public CodeGeneratorTest<TGenerator> ExpectOutput(string name, string code)
	{
		OutputFiles.Add(new(name, code));
		return this;
	}

	public void Run()
	{
		var verifier = new MSTestVerifier();
		var type = TestsType;
		var tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(Input, Options, $"{type.Namespace}\\{type.Name}\\{TestName}.cs");
		var compilation = TestHelper.CreateCompilation(tree, type.Namespace!);
		var driver = (CSharpGeneratorDriver)CSharpGeneratorDriver.Create(new TGenerator()).WithUpdatedParseOptions(Options);

		driver.RunGeneratorsAndUpdateCompilation(compilation, out var generatedCompilation, out var diagnostics);
		verifier.Empty("Diagnostics", diagnostics);

		foreach (var (fileName, code) in OutputFiles)
		{
			var fullName = FileNamePrefix + fileName + ".g.cs";
			var file = generatedCompilation.SyntaxTrees.FirstOrDefault(v => v.FilePath == fullName);
			Assert.IsNotNull(file, "Generated file \"{0}\" was not found in compilation", fileName);
			verifier.EqualOrDiff(code, file.ToString());
		}
	}
}
