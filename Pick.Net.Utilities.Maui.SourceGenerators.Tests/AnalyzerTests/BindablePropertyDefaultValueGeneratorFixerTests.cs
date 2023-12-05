using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindablePropertyDefaultValueGeneratorFixerTests : CodeFixTests<BindablePropertyAttributeAnalyzer, BindablePropertyDefaultValueGeneratorFixProvider>
{
	[TestMethod]
	public async Task InstanceProperty()
	{
		const string original = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    using System;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private string CreateText()
        {
            throw new NotImplementedException();
        }

        [BindableProperty(DefaultValue = nameof(CreateText))]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 9, 19, 4, "Text")
			.RunAsync();
	}
	[TestMethod]
	public async Task AttachedProperty()
	{
		const string original = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public static partial string? GetText(Entry entry);
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    using System;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private static string? CreateText(Entry entry)
        {
            throw new NotImplementedException();
        }

        [BindableProperty(DefaultValue = nameof(CreateText))]
        public static partial string? GetText(Entry entry);
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 9, 35, 7, "Text")
			.RunAsync();
	}
}
