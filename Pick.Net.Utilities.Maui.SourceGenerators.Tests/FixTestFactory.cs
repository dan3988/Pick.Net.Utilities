using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal static class FixTestFactory<TAnalyzer, TCodeFix>
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TCodeFix : CodeFixProvider, new()
{
	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier> CreateTest(string code, string expected)
	{
		var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
		{
			TestCode = code,
			FixedCode = expected
		};

		test.SetUpReferences();

		return test;
	}
}
