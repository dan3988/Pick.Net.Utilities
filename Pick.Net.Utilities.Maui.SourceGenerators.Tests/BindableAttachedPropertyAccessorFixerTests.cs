using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using CodeFixTest = CSharpCodeFixTest<BindableAttachedPropertyMethodAnalyzer, BindableAttachedPropertyMethodToPartialFixProvider, MSTestVerifier>;
using CodeFixVerifier = CSharpCodeFixVerifier<BindableAttachedPropertyMethodAnalyzer, BindableAttachedPropertyMethodToPartialFixProvider, MSTestVerifier>;

[TestClass]
public class BindableAttachedPropertyAccessorFixerTests
{
	[TestMethod]
	public async Task ConvertPropertyGetMethod()
	{
		const string original = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		public static readonly BindableProperty WrongProperty;
	
		[BindableProperty]
		public static string GetValue(BindableObject obj)
			=> (string)obj.GetValue(WrongProperty);
	
		public static void SetValue(BindableObject obj, string value)
			=> obj.SetValue(WrongProperty, value);
	}
	""";

		const string expected = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;

	namespace Test;

	partial class TestClass : BindableObject
	{
		public static readonly BindableProperty WrongProperty;
	
		[BindableProperty]
		public static partial string GetValue(BindableObject obj);

		public static partial void SetValue(BindableObject obj, string value);
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
				},
				ExpectedDiagnostics =
				{
					CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed).WithSpan(11, 23, 11, 31).WithArguments("Value"),
					CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance).WithSpan(11, 23, 11, 31).WithArguments("Value")
				}
			},
			FixedState =
			{
				ExpectedDiagnostics =
				{
					CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance).WithSpan(11, 31, 11, 39).WithArguments("Value")
				}
			}
		};

		await test.RunAsync();
	}
}