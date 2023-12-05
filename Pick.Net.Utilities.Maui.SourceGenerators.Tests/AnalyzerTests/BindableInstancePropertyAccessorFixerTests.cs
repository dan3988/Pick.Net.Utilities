using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindableInstancePropertyAccessorFixerTests : CodeFixTests<BindableInstancePropertyAccessorAnalyzer, BindableInstancePropertyAutoPropertyFixProvider>
{
	[TestMethod]
	public async Task FixPropertyAccessors()
	{
		const string original = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        public static readonly BindableProperty WrongProperty;

        [BindableProperty]
        public string Value
        {
            get => (string)GetValue(WrongProperty);
            set => SetValue(WrongProperty, value);
        }
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
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, 11, 19, 5, "Value")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 11, 19, 5, "Value")
			.RunAsync();
	}

	[TestMethod]
	public async Task FixReadOnlyPropertyAccessors()
	{
		const string original = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public string Value
        {
        	get => (string)GetValue(ValueProperty);
        	private set => SetValue(ValueProperty, value);
        }
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            private set => SetValue(ValuePropertyKey, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, 9, 19, 5, "Value")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 9, 19, 5, "Value")
			.RunAsync();
	}

	[TestMethod]
	public async Task FixAutoProperty()
	{
		const string original = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public string Value { get; set; } = "test";
    }
    """;

		const string expected = """
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private const string ValueDefaultValue = "test";

        [BindableProperty(DefaultValue = nameof(ValueDefaultValue))]
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, 9, 19, 5, "Value")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 9, 19, 5, "Value")
			.ExpectFixDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 11, 19, 5, "Value")
			.RunAsync();
	}

	[TestMethod]
	public async Task FixAutoPropertyNonConst()
	{
		const string original = """
    using System.Collections.Generic;
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public IList<Element> Values { get; set; } = new List<Element>();
    }
    """;

		const string expected = """
    using System.Collections.Generic;
    using Pick.Net.Utilities.Maui.Helpers;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        private static IList<Element> CreateValues() => new List<Element>();

        [BindableProperty(DefaultValue = nameof(CreateValues))]
        public IList<Element> Values
        {
            get => (IList<Element>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, 10, 27, 6, "Values")
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 10, 27, 6, "Values")
			.ExpectFixDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 12, 27, 6, "Values")
			.RunAsync();
	}
}