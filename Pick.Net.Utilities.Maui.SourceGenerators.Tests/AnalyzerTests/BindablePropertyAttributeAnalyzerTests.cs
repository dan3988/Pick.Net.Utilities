using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindablePropertyAttributeAnalyzerTests : CodeAnalyzerTests<BindablePropertyAttributeAnalyzer>
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

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, 9, 16, 5, (object)66)
			.RunAsync();
	}
}