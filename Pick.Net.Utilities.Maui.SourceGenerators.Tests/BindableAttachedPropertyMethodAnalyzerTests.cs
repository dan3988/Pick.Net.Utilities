using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using AnalyzerTest = CSharpAnalyzerTest<BindableAttachedPropertyMethodAnalyzer, MSTestVerifier>;
using AnalyzerVerifier = CSharpAnalyzerVerifier<BindableAttachedPropertyMethodAnalyzer, MSTestVerifier>;

[TestClass]
public class BindableAttachedPropertyMethodAnalyzerTests
{
	[TestMethod]
	public async Task MethodNameDiagnosticTest()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static partial string NoGetPrefix(BindableObject obj);
	}
	""";

		var test = new AnalyzerTest
		{
			TestCode = code,
			ReferenceAssemblies = TestHelper.Net80,
			SolutionTransforms =
			{
				TestHelper.AddAnalyzerToSolution
			},
			TestState =
			{
				AdditionalReferences =
				{
					TestHelper.MauiAssembly,
					TestHelper.UtilitiesMauiAssembly
				}
			},
			ExpectedDiagnostics =
			{
				AnalyzerVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance).WithSpan(9, 31, 9, 42).WithArguments("Test"),
				AnalyzerVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName).WithSpan(9, 31, 9, 42).WithArguments("NoGetPrefix")
			}
		};

		await test.RunAsync();
	}

	[TestMethod]
	public async Task MethodReturnTypeDiagnosticTest()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static void GetTest(BindableObject obj)
		{
		}
	}
	""";

		var test = new AnalyzerTest
		{
			TestCode = code,
			ReferenceAssemblies = TestHelper.Net80,
			SolutionTransforms =
			{
				TestHelper.AddAnalyzerToSolution
			},
			TestState =
			{
				AdditionalReferences =
				{
					TestHelper.MauiAssembly,
					TestHelper.UtilitiesMauiAssembly
				}
			},
			ExpectedDiagnostics =
			{
				AnalyzerVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn).WithSpan(9, 21, 9, 28).WithArguments("Test")
			}
		};

		await test.RunAsync();
	}

	[TestMethod]
	public async Task MethodNoParametersDiagnosticTest()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static string GetTest() => "";
	}
	""";

		var test = new AnalyzerTest
		{
			TestCode = code,
			ReferenceAssemblies = TestHelper.Net80,
			SolutionTransforms =
			{
				TestHelper.AddAnalyzerToSolution
			},
			TestState =
			{
				AdditionalReferences =
				{
					TestHelper.MauiAssembly,
					TestHelper.UtilitiesMauiAssembly
				}
			},
			ExpectedDiagnostics =
			{
				AnalyzerVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature).WithSpan(9, 23, 9, 30).WithArguments("Test")
			}
		};

		await test.RunAsync();
	}

	[TestMethod]
	public async Task MethodMultipleParametersDiagnosticTest()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static string GetTest(BindableObject obj, object unused)
			=> (string)obj.GetValue(TestProperty);
	}
	""";

		var test = new AnalyzerTest
		{
			TestCode = code,
			ReferenceAssemblies = TestHelper.Net80,
			SolutionTransforms =
			{
				TestHelper.AddAnalyzerToSolution
			},
			TestState =
			{
				AdditionalReferences =
				{
					TestHelper.MauiAssembly,
					TestHelper.UtilitiesMauiAssembly
				}
			},
			ExpectedDiagnostics =
			{
				AnalyzerVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature).WithSpan(9, 23, 9, 30).WithArguments("Test")
			}
		};

		await test.RunAsync();
	}
}
