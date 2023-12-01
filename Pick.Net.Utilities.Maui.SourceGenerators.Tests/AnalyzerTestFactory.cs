using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal sealed class AnalyzerTestFactory<TAnalyzer>
	where TAnalyzer : DiagnosticAnalyzer, new()
{
	public CSharpAnalyzerTest<TAnalyzer, MSTestVerifier> CreateTest(string code)
	{
		var test = new CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
		{
			TestCode = code
		};

		test.SetUpReferences();

		return test;
	}
}
