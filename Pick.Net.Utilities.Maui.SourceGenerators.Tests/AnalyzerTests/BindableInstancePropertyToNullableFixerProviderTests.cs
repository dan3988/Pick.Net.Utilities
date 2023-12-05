using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindableInstancePropertyToNullableFixerProviderTests : CodeFixTests<BindablePropertyAttributeAnalyzer, BindableInstancePropertyToNullableFixerProvider>
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

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull, 10, 16, 5, "Value")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 10, 16, 5, "Value")
			.ExpectFixDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 10, 17, 5, "Value")
			.RunAsync();
	}
}