using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class InstancePropertyTests : CodeGeneratorTests<BindablePropertyGenerator>
{
	[TestMethod]
	public async Task TestSimpleInstanceProperty()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class Class : BindableObject
			{
				[BindableProperty]
				public string Text
				{
					get => (string)GetValue(TextProperty);
					set => SetValue(TextProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class Class
				{
					partial void OnTextChanging(string oldValue, string newValue);

					partial void OnTextChanged(string oldValue, string newValue);

					/// <summary>Bindable property for <see cref="Text"/>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					public static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanging((string)oldValue, (string)newValue),
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanged((string)oldValue, (string)newValue),
						null,
						null);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestReadOnlyInstanceProperty()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class Class : BindableObject
			{
				[BindableProperty]
				protected string Text
				{
					get => (string)GetValue(TextProperty);
					private set => SetValue(TextPropertyKey, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			namespace Test.Namespace
			{
				partial class Class
				{
					partial void OnTextChanging(string oldValue, string newValue);

					partial void OnTextChanged(string oldValue, string newValue);

					/// <summary>Bindable property key for <see cref="Text"/>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					private static readonly global::Microsoft.Maui.Controls.BindablePropertyKey TextPropertyKey = global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanging((string)oldValue, (string)newValue),
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanged((string)oldValue, (string)newValue),
						null,
						null);
			
					/// <summary>Bindable property for <see cref="Text"/>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					protected static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = TextPropertyKey.BindableProperty;

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task TestInstancePropertyChangeHandlerSignatureCopying()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			namespace Test.Namespace;

			public partial class Class : BindableObject
			{
				[BindableProperty]
				public string? Text
				{
					get => (string?)GetValue(TextProperty);
					set => SetValue(TextProperty, value);
				}
			
				private partial void OnTextChanging(string? oldText, string? newText)
				{
				}

				protected virtual partial void OnTextChanged(string aaaa, string bbbb)
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
					private partial void OnTextChanging(string? oldText, string? newText);

					protected virtual partial void OnTextChanged(string? aaaa, string? bbbb);

					/// <summary>Bindable property for <see cref="Text"/>.</summary>
					[global::System.CodeDom.Compiler.GeneratedCode("Pick.Net.Utilities.Maui.SourceGenerators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "1.0.0.0")]
					public static readonly global::Microsoft.Maui.Controls.BindableProperty TextProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
						"Text",
						typeof(string),
						typeof(global::Test.Namespace.Class),
						null,
						global::Microsoft.Maui.Controls.BindingMode.OneWay,
						null,
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanging((string? )oldValue, (string? )newValue),
						(bindable, oldValue, newValue) => ((global::Test.Namespace.Class)bindable).OnTextChanged((string? )oldValue, (string? )newValue),
						null,
						null);

				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.Namespace.Class", output)
			.RunAsync();
	}
}
