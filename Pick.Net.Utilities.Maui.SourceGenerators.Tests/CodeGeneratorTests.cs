using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTests<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	protected CodeGeneratorTest<TGenerator> CreateTest(string code, LanguageVersion version = LanguageVersion.CSharp12, [CallerMemberName] string testName = null!)
	{
		return new()
		{
			TestsType = GetType(),
			TestName = testName,
			Input = code,
			Options = CSharpParseOptions.Default.WithLanguageVersion(version)
		};
	}
}
