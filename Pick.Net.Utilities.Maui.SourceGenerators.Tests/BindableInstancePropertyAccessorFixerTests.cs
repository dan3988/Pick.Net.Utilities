using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

[TestClass]
public class BindableInstancePropertyAccessorFixerTests
{
    private static readonly FixTestFactory<BindableInstancePropertyAccessorAnalyzer, BindableInstancePropertyAutoPropertyFixer> Factory = new();

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

        await Factory.CreateTest(original, expected)
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

        await Factory.CreateTest(original, expected)
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
        [BindableProperty(DefaultValue = "test")]
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
    """;

        await Factory.CreateTest(original, expected)
            .ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed, 9, 19, 5, "Value")
            .ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 9, 19, 5, "Value")
            .RunAsync();
    }
}