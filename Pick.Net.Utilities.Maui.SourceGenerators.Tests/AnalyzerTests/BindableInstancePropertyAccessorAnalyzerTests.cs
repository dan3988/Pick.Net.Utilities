using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class BindableInstancePropertyAccessorAnalyzerTests : CodeAnalyzerTests<BindableInstancePropertyAccessorAnalyzer>
{
	[TestMethod]
	public async Task TestErrorOnIndexer()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class Class : BindableObject
			{
				[BindableProperty]
				public string this[string param] { get => ""; set { } }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyAttributeOnIndexer, 9, 16, 4)
			.RunAsync();
	}
}