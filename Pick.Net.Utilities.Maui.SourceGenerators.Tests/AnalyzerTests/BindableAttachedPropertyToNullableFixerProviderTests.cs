using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindableAttachedPropertyToNullableFixerProviderTests : CodeFixTests<BindablePropertyAttributeAnalyzer, BindableAttachedPropertyToNullableFixerProvider>
{
	[TestMethod]
	public async Task MakeAttachedPropertyNullable()
	{
		const string original = """
	#nullable enable
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		[BindableProperty]
		public static string GetValue(BindableObject obj)
		{
			return (string)obj.GetValue(ValueProperty);
		}
	
		[BindableProperty]
		public static void SetValue(BindableObject obj, string value)
		{
			obj.SetValue(ValueProperty, value);
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
		public static string? GetValue(BindableObject obj)
		{
			return (string?)obj.GetValue(ValueProperty);
		}
	
		[BindableProperty]
		public static void SetValue(BindableObject obj, string? value)
		{
			obj.SetValue(ValueProperty, value);
		}
	}
	""";

		await CreateTest(original, expected)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueNull, 10, 23, 8, "Value")
			.RunAsync();
	}
}
