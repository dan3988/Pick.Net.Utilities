using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTests<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	protected CodeGeneratorTest<TGenerator> CreateTest(string code, LanguageVersion version = LanguageVersion.CSharp12)
	{
		var test = new CodeGeneratorTest<TGenerator>()
		{
			TestCode = code,
			Options = CSharpParseOptions.Default.WithLanguageVersion(version)
		};

		test.SetUpReferences(false);

		return test;
	}
}
