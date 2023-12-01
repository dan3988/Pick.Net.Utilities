using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using CodeFixTest = CSharpAnalyzerTest<BindablePropertyAttributeAnalyzer, MSTestVerifier>;
using CodeFixVerifier = CSharpAnalyzerVerifier<BindablePropertyAttributeAnalyzer, MSTestVerifier>;

[TestClass]
public class BindablePropertyAttributeAnalyzerTests
{
	[TestMethod]
	public async Task ReportUndefinedBindngMode()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty(DefaultMode = (BindingMode)66)]
		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
	""";

		var test = new CodeFixTest
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
				CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode).WithSpan(9, 16, 9, 21).WithArguments("66")
			}
		};

		await test.RunAsync();
	}
	[TestMethod]
	public async Task ReportDefaultValueAndGenerator()
	{
		const string code = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty(DefaultValue = "", DefaultValueFactory = true)]
		public string Value
		{
			get => (string)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private partial string GenerateValueDefaultValue() => "";
	}
	""";

		var test = new CodeFixTest
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
				CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory).WithSpan(9, 16, 9, 21).WithArguments("66")
			}
		};

		await test.RunAsync();
	}
}