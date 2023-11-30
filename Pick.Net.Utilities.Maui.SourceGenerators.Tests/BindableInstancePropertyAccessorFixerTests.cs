using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

using CodeFixTest = CSharpCodeFixTest<BindableInstancePropertyAccessorAnalyzer, BindableInstancePropertyAutoPropertyFixer, MSTestVerifier>;
using CodeFixVerifier = CSharpCodeFixVerifier<BindableInstancePropertyAccessorAnalyzer, BindableInstancePropertyAutoPropertyFixer, MSTestVerifier>;

[TestClass]
public class BindableInstancePropertyAccessorFixerTests
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
                CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed).WithSpan(11, 19, 11, 24).WithArguments("Value")
            }
        };

        await test.RunAsync();
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
                CodeFixVerifier.Diagnostic(DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed).WithSpan(9, 19, 9, 24).WithArguments("Value")
            }
        };

        await test.RunAsync();
    }
}