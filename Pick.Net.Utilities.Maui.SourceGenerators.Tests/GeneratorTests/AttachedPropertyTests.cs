using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class AttachedPropertyTests : CodeGeneratorTests<BindablePropertyGenerator>
{
	[TestMethod]
	public async Task TestSimpleAttachedProperty()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public static partial class Class
			{
				[BindableProperty]
				public static partial string GetText(Element element);
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class Class
				{
					static partial void OnTextChanging(global::Microsoft.Maui.Controls.Element bindable, string oldValue, string newValue);

					static partial void OnTextChanged(global::Microsoft.Maui.Controls.Element bindable, string oldValue, string newValue);
			
					/// <summary>Bindable property for the attached property <c>Text</c>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					public static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => OnTextChanging((global::Microsoft.Maui.Controls.Element)bindable, (string)oldValue, (string)newValue),
						(bindable, oldValue, newValue) => OnTextChanged((global::Microsoft.Maui.Controls.Element)bindable, (string)oldValue, (string)newValue),
						null,
						null);

					public static partial string GetText(global::Microsoft.Maui.Controls.Element element) 
						=> (string)element.GetValue(TextProperty);

					public static void SetText(global::Microsoft.Maui.Controls.Element element, string value) 
						=> element.SetValue(TextProperty, value);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestReadOnlyAttachedProperty()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public static partial class Class
			{
				[BindableProperty]
				protected static partial string GetText(Element element);

				private static partial void SetText(Element element, string value);
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class Class
				{
					static partial void OnTextChanging(global::Microsoft.Maui.Controls.Element bindable, string oldValue, string newValue);
			
					static partial void OnTextChanged(global::Microsoft.Maui.Controls.Element bindable, string oldValue, string newValue);
			
					/// <summary>Bindable property key for the attached property <c>Text</c>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					private static readonly global::Microsoft.Maui.Controls.BindablePropertyKey TextPropertyKey = global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => OnTextChanging((global::Microsoft.Maui.Controls.Element)bindable, (string)oldValue, (string)newValue),
						(bindable, oldValue, newValue) => OnTextChanged((global::Microsoft.Maui.Controls.Element)bindable, (string)oldValue, (string)newValue),
						null,
						null);
			
					/// <summary>Bindable property for the attached property <c>Text</c>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					protected static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = TextPropertyKey.BindableProperty;

					protected static partial string GetText(global::Microsoft.Maui.Controls.Element element) 
						=> (string)element.GetValue(TextProperty);

					private static partial void SetText(global::Microsoft.Maui.Controls.Element element, string value) 
						=> element.SetValue(TextPropertyKey, value);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestAttachedPropertyChangeHandlerSignatureCopying()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class Class : BindableObject
			{
				[BindableProperty]
				public static partial string? GetText(Element element);

				private static partial void OnTextChanging(Element element, string? oldText, string? newText)
				{
				}

				protected static partial void OnTextChanged(Element e, string aaaa, string bbbb)
				{
				}
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class Class
				{
					private static partial void OnTextChanging(global::Microsoft.Maui.Controls.Element element, string? oldText, string? newText);

					protected static partial void OnTextChanged(global::Microsoft.Maui.Controls.Element e, string? aaaa, string? bbbb);

					/// <summary>Bindable property for the attached property <c>Text</c>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					public static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => OnTextChanging((global::Microsoft.Maui.Controls.Element)bindable, (string? )oldValue, (string? )newValue),
						(bindable, oldValue, newValue) => OnTextChanged((global::Microsoft.Maui.Controls.Element)bindable, (string? )oldValue, (string? )newValue),
						null,
						null);

					public static partial string? GetText(global::Microsoft.Maui.Controls.Element element) 
						=> (string? )element.GetValue(TextProperty);

					public static void SetText(global::Microsoft.Maui.Controls.Element element, string? value) 
						=> element.SetValue(TextProperty, value);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}
}
