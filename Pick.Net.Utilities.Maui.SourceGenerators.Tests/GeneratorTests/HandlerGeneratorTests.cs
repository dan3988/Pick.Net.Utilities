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
			using Microsoft.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public interface ICustomView : IView
			{
				[HandlerProperty]
				bool IsChecked { get; set; }

				[HandlerProperty]
				bool IsReadOnly { get; set; }

				[HandlerProperty]
				string? Title { get; set; }
			}
			
			[HandlerMapper(typeof(ICustomView))]
			public partial class CustomViewHandler
			{
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class CustomViewHandler
				{
					private static readonly global::Microsoft.Maui.IPropertyMapper<global::Test.Namespace.ICustomView, global::Test.Namespace.CustomViewHandler> GeneratedMapper = new global::Microsoft.Maui.PropertyMapper<global::Test.Namespace.ICustomView, global::Test.Namespace.CustomViewHandler>
					{
						["IsChecked"] = MapIsChecked,
						["IsReadOnly"] = MapIsReadOnly,
						["Title"] = MapTitle
					};

					private static partial void MapIsChecked(global::Test.Namespace.CustomViewHandler handler, global::Test.Namespace.ICustomView view);

					private static partial void MapIsReadOnly(global::Test.Namespace.CustomViewHandler handler, global::Test.Namespace.ICustomView view);

					private static partial void MapTitle(global::Test.Namespace.CustomViewHandler handler, global::Test.Namespace.ICustomView view);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.CustomViewHandler", output)
			.RunAsync();
	}
}
