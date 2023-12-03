using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

internal static class TestExtensions
{
	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> ExpectFixDiagnostic<TAnalyzer, TCodeFix, TVerifier>(this CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> test, DiagnosticDescriptor descriptor, int line, int startColumn, int length, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
		where TVerifier : IVerifier, new()
	{
		return ExpectFixDiagnostic(test, descriptor, line, startColumn, line, startColumn + length, messageArgs);
	}

	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> ExpectFixDiagnostic<TAnalyzer, TCodeFix, TVerifier>(this CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> test, DiagnosticDescriptor descriptor, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
		where TVerifier : IVerifier, new()
	{
		test.FixedState.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<TAnalyzer, TVerifier>.Diagnostic(descriptor), startLine, startColumn, endLine, endColumn, messageArgs);
		return test;
	}

	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> ExpectDiagnostic<TAnalyzer, TCodeFix, TVerifier>(this CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> test, DiagnosticDescriptor descriptor, int line, int startColumn, int length, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
		where TVerifier : IVerifier, new()
	{
		return ExpectDiagnostic(test, descriptor, line, startColumn, line, startColumn + length, messageArgs);
	}

	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> ExpectDiagnostic<TAnalyzer, TCodeFix, TVerifier>(this CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> test, DiagnosticDescriptor descriptor, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
		where TVerifier : IVerifier, new()
	{
		test.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<TAnalyzer, TVerifier>.Diagnostic(descriptor), startLine, startColumn, endLine, endColumn, messageArgs);
		return test;
	}

	public static CSharpAnalyzerTest<TAnalyzer, TVerifier> ExpectDiagnostic<TAnalyzer, TVerifier>(this CSharpAnalyzerTest<TAnalyzer, TVerifier> test, DiagnosticDescriptor descriptor, int line, int startColumn, int length, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TVerifier : IVerifier, new()
	{
		return ExpectDiagnostic(test, descriptor, line, startColumn, line, startColumn + length, messageArgs);
	}

	public static CSharpAnalyzerTest<TAnalyzer, TVerifier> ExpectDiagnostic<TAnalyzer, TVerifier>(this CSharpAnalyzerTest<TAnalyzer, TVerifier> test, DiagnosticDescriptor descriptor, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TVerifier : IVerifier, new()
	{
		test.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<TAnalyzer, TVerifier>.Diagnostic(descriptor), startLine, startColumn, endLine, endColumn, messageArgs);
		return test;
	}

	private static void Add(this List<DiagnosticResult> results, DiagnosticResult result, int startLine, int startColumn, int endLine, int endColumn, params object[] messageArgs)
		=> results.Add(result.WithSpan(startLine, startColumn, endLine, endColumn).WithArguments(messageArgs));

}
