using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class HandlerGeneratorTests : CodeGeneratorTests<HandlerMapperGenerator>
{
	[TestMethod]
	public async Task TestSimpleInstanceProperty()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class TestClass : VisualElement
			{
				[HandlerProperty]
				public string Text { get; set; }
			
				[HandlerProperty]
				public int Value { get; set; }

				public bool NotMapped { get; set; }
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class TestClassHandler
				{
					private static readonly global::Microsoft.Maui.IPropertyMapper<global::Test.Namespace.TestClass, global::Test.Namespace.TestClassHandler> GeneratedMapper = new global::Microsoft.Maui.PropertyMapper<global::Test.Namespace.TestClass, global::Test.Namespace.TestClassHandler>
					{
						["Text"] = MapText,
						["Value"] = MapValue
					};

					private static partial void MapText(global::Test.Namespace.TestClassHandler handler, global::Test.Namespace.TestClass view);

					private static partial void MapValue(global::Test.Namespace.TestClassHandler handler, global::Test.Namespace.TestClass view);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.TestClassHandler", output)
			.RunAsync();
	}
}
