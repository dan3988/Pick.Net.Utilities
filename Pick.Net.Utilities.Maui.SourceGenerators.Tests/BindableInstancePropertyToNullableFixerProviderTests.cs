using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using CodeFixTest = CSharpCodeFixTest<BindablePropertyDefaultValueAnalyzer, BindableInstancePropertyToNullableFixerProvider, MSTestVerifier>;
using CodeFixVerifier = CSharpCodeFixVerifier<BindablePropertyDefaultValueAnalyzer, BindableInstancePropertyToNullableFixerProvider, MSTestVerifier>;

[TestClass]
public class BindableInstancePropertyToNullableFixerProviderTests
{
	[TestMethod]
	public async Task MakePropertyNullable()
	{
		const string original = """
	#nullable enable
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public string Value
		{
			get => (string)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
	""";

		const string expected = """
	#nullable enable
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public string? Value
		{
			get => (string?)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
	""";

		var test = new CodeFixTest
		{
			TestCode = original,
			FixedCode = expected,
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
				CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull).WithSpan(10, 16, 10, 21).WithArguments("Value")
			}
		};

		await test.RunAsync();
	}

	[TestMethod]
	public async Task MakeAttachedPropertyNullable()
	{
		const string original = """
	#nullable enable
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static string GetValue(BindableObject obj)
		{
			return (string)obj.GetValue(ValueProperty);
		}
	
		[BindableProperty]
		public static void SetValue(BindableObject obj, string value)
		{
			obj.SetValue(ValueProperty, value);
		}
	}
	""";

		const string expected = """
	#nullable enable
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;

	namespace Test;

	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static string? GetValue(BindableObject obj)
		{
			return (string?)obj.GetValue(ValueProperty);
		}
	
		[BindableProperty]
		public static void SetValue(BindableObject obj, string? value)
		{
			obj.SetValue(ValueProperty, value);
		}
	}
	""";

		var test = new CodeFixTest
		{
			TestCode = original,
			FixedCode = expected,
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
				CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull).WithSpan(10, 23, 10, 31).WithArguments("Value")
			}
		};

		await test.RunAsync();
	}
}