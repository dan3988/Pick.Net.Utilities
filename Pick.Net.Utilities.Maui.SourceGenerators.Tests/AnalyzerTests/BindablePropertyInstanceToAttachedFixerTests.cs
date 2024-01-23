using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindablePropertyInstanceToAttachedFixerTests : CodeFixTests<BindableInstancePropertyAccessorAnalyzer, BindablePropertyInstanceToAttachedFixProvider>
{
	[TestMethod]
	public async Task TestSimpleProperty()
	{
		const string original = """
    using Pick.Net.Utilities.Maui;
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
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public static partial string GetValue(BindableObject obj);
        public static partial void SetValue(BindableObject obj, string value);
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 9, 19, 5, "Value")
			.RunAsync();
	}

	[TestMethod]
	public async Task TestReadOnlyProperty()
	{
		const string original = """
    using Pick.Net.Utilities.Maui;
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

		const string expected = """
    using Pick.Net.Utilities.Maui;
    using Microsoft.Maui.Controls;
    
    namespace Test;
    
    partial class TestClass : BindableObject
    {
        [BindableProperty]
        public static partial string GetValue(BindableObject obj);
        private static partial void SetValue(BindableObject obj, string value);
    }
    """;

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceToAttached, 9, 19, 5, "Value")
			.RunAsync();
	}
}
