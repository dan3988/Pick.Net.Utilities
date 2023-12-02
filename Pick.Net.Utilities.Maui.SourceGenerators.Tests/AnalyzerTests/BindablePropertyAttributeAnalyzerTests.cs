using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

using Factory = AnalyzerTestFactory<BindablePropertyAttributeAnalyzer>;

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

        await Factory.CreateTest(code)
            .ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, 9, 16, 5, (object)66)
            .RunAsync();
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

        await Factory.CreateTest(code)
            .ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory, 9, 16, 5, "Value")
            .RunAsync();
    }
}