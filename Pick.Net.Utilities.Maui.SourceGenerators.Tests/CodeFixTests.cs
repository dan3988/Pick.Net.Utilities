using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

public abstract class CodeFixTests<TAnalyzer, TCodeFix>
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TCodeFix : CodeFixProvider, new()
{
	protected static CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier> CreateTest(string code, string expected)
	{
		var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
		{
			TestCode = code,
			FixedCode = expected
		};

		test.SetUpReferences();

		return test;
	}

	protected static CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier> CreateTest(string code)
		=> CreateTest(code, code);
}
