using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeGeneratorTests<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
	protected CodeGeneratorTest<TGenerator> CreateTest(string code, [CallerMemberName] string testName = null!)
	{
		return new()
		{
			TestsType = GetType(),
			TestName = testName,
			Input = code
		};
	}
}
