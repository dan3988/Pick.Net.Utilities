using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using Factory = AnalyzerTestFactory<BindableAttachedPropertyMethodAnalyzer>;

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

		await Factory.CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance, 9, 31, 11, "Test")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName, 9, 31, 11, "NoGetPrefix")
			.RunAsync();
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

		await Factory.CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn, 9, 21, 7, "Test")
			.RunAsync();
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

		await Factory.CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, 9, 23, 7, "Test")
			.RunAsync();
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

		await Factory.CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, 9, 23, 7, "Test")
			.RunAsync();
	}
}
