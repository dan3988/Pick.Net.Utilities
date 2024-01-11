using Microsoft.CodeAnalysis.Testing;

using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.AnalyzerTests;

[TestClass]
public class HandlerAddPartialPlatformClassFixerTests : CodeFixTests<HandlerClassAnalyzer, HandlerAddPartialPlatformClassFixer>
{
	private static async Task RunForPlatform(string platform, ReferenceAssemblies assemblies, string code)
	{
		const string fix = """
			namespace Test.Namespace;

			partial class CustomViewHandler
			{
			    private static partial void MapIsChecked(CustomViewHandler handler, ICustomView view)
			    {
			    }

			    private static partial void MapIsReadOnly(CustomViewHandler handler, ICustomView view)
			    {
			    }

			    private static partial void MapTitle(CustomViewHandler handler, ICustomView view)
			    {
			    }
			}
			""";

		var test = CreateTest(code);
		test.ReferenceAssemblies = assemblies;
		test.ExpectDiagnostic(DiagnosticDescriptors.HandlerAddPartialPlatformClass, 22, 22, 22, 39, platform);
		test.TestState.ExpectedDiagnostics.AddRange(
		[
			DiagnosticResult.CompilerError("CS0762").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 8, 20, 8, 32).WithArguments("Test.Namespace.CustomViewHandler.MapIsChecked(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
			DiagnosticResult.CompilerError("CS0762").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 9, 21, 9, 34).WithArguments("Test.Namespace.CustomViewHandler.MapIsReadOnly(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
			DiagnosticResult.CompilerError("CS0762").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 10, 16, 10, 24).WithArguments("Test.Namespace.CustomViewHandler.MapTitle(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
			DiagnosticResult.CompilerError("CS8795").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 13, 31, 13, 43).WithArguments("Test.Namespace.CustomViewHandler.MapIsChecked(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
			DiagnosticResult.CompilerError("CS8795").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 15, 31, 15, 44).WithArguments("Test.Namespace.CustomViewHandler.MapIsReadOnly(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
			DiagnosticResult.CompilerError("CS8795").WithSpan(@"Pick.Net.Utilities.Maui.SourceGenerators\Pick.Net.Utilities.Maui.SourceGenerators.Generators.HandlerMapperGenerator\Test.Namespace.CustomViewHandler.g.cs", 17, 31, 17, 39).WithArguments("Test.Namespace.CustomViewHandler.MapTitle(Test.Namespace.CustomViewHandler, Test.Namespace.ICustomView)"),
		]);

		test.CodeActionEquivalenceKey = $"HandlerAddPartialPlatformClassFixer.{platform}";
		test.FixedState.Sources.Add(($"/0/Test0.{platform}.cs", fix));
		test.ExpectFixDiagnostic(DiagnosticDescriptors.HandlerAddPartialPlatformClass, 22, 22, 22, 39, platform);

		await test.RunAsync();
	}

	[TestMethod]
	public async Task TestAndroid()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui;
			using Microsoft.Maui.Controls;
			using Microsoft.Maui.Handlers;

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
			public partial class CustomViewHandler : ViewHandler<ICustomView, Android.Views.View>
			{
				public static readonly IPropertyMapper<ICustomView, CustomViewHandler> Mapper = new PropertyMapper<ICustomView, CustomViewHandler>(ViewMapper, GeneratedMapper);

				public CustomViewHandler() : base(Mapper)
				{
				}
			
				protected override Android.Views.View CreatePlatformView()
				{
					throw new NotImplementedException();
				}
			}
			""";

		await RunForPlatform("Android", TestHelper.Net80Android, code);
	}

	[TestMethod]
	public async Task TestIos()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui;
			using Microsoft.Maui.Controls;
			using Microsoft.Maui.Handlers;

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
			public partial class CustomViewHandler : ViewHandler<ICustomView, UIKit.UIView>
			{
				public static readonly IPropertyMapper<ICustomView, CustomViewHandler> Mapper = new PropertyMapper<ICustomView, CustomViewHandler>(ViewMapper, GeneratedMapper);

				public CustomViewHandler() : base(Mapper)
				{
				}
			
				protected override UIKit.UIView CreatePlatformView()
				{
					throw new NotImplementedException();
				}
			}
			""";

		await RunForPlatform("iOS", TestHelper.Net80iOS, code);
	}
}
