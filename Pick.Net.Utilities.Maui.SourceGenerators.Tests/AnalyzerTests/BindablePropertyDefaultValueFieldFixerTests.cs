using Microsoft.CodeAnalysis;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindablePropertyDefaultValueFieldFixerTests : CodeFixTests<BindablePropertyAttributeAnalyzer, BindablePropertyDefaultValueFieldFixProvider>
{
	[TestMethod]
	public async Task TestDefaultValueString()
	{
		const string original = """
    using Pick.Net.Utilities.Maui;
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
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private const string TextDefaultValue;

        [BindableProperty(DefaultValue = nameof(TextDefaultValue))]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 9, 19, 4, "Text")
			.ExpectFixDiagnostic("CS0145", DiagnosticSeverity.Error, 8, 26, 16)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestDefaultValueInt()
	{
		const string original = """
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public int Number
        {
            get => (int)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private const int NumberDefaultValue;

        [BindableProperty(DefaultValue = nameof(NumberDefaultValue))]
        public int Number
        {
            get => (int)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 9, 16, 6, "Number")
			.ExpectFixDiagnostic("CS0145", DiagnosticSeverity.Error, 8, 23, 18)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestDefaultValueObject()
	{
		const string original = """
    using System.Collections.Generic;
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public IReadOnlyList<string> Values
        {
            get => (IReadOnlyList<string>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }
    }
    """;

		const string expected = """
    using System.Collections.Generic;
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private static readonly IReadOnlyList<string> ValuesDefaultValue;

        [BindableProperty(DefaultValue = nameof(ValuesDefaultValue))]
        public IReadOnlyList<string> Values
        {
            get => (IReadOnlyList<string>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 10, 34, 6, "Values")
			.RunAsync();
	}

	[TestMethod]
	public async Task TestDefaultValueNullable()
	{
		const string original = """
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private const double ValueDefaultValue;

        [BindableProperty(DefaultValue = nameof(ValueDefaultValue))]
        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyNoDefaultValue, 9, 20, 5, "Value")
			.ExpectFixDiagnostic("CS0145", DiagnosticSeverity.Error, 8, 26, 17)
			.RunAsync();
	}
}
