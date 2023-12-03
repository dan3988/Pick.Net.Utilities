using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTests<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	protected CodeGeneratorTest<TGenerator> CreateTest(string code, LanguageVersion version = LanguageVersion.CSharp12, [CallerMemberName] string testName = null!)
	{
		var test = new CodeGeneratorTest<TGenerator>()
		{
			TestsType = GetType(),
			TestName = testName,
			TestCode = code,
			Options = CSharpParseOptions.Default.WithLanguageVersion(version)
		};

		test.SetUpReferences(false);

		return test;
	}
}
