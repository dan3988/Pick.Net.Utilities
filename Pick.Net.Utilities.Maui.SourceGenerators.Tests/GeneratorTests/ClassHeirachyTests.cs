using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class ClassHeirachyTests : CodeGeneratorTests<BindablePropertyGenerator>
{
	[TestMethod]
	public async Task TestRootLevelClass()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class RootLevelClass : BindableObject
			{
				[BindableProperty]
				public int Integer
				{
					get => (int)GetValue(IntegerProperty);
					set => SetValue(IntegerProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class RootLevelClass
			{
				partial void OnIntegerChanging(int oldValue, int newValue);

				partial void OnIntegerChanged(int oldValue, int newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty IntegerProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"Integer",
					typeof(int),
					typeof(global::RootLevelClass),
					null,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::RootLevelClass)bindable).OnIntegerChanging((int)oldValue, (int)newValue),
					(bindable, oldValue, newValue) => ((global::RootLevelClass)bindable).OnIntegerChanged((int)oldValue, (int)newValue),
					null,
					null);

			}
			""";

		await CreateTest(code)
			.ExpectOutput("RootLevelClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task NestedClasses()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			namespace Test;

			public partial class RootClass : BindableObject
			{
				public partial class FirstNestedClass : BindableObject
				{
					[BindableProperty]
					public object Value
					{
						get => GetValue(ValueProperty);
						set => SetValue(ValueProperty, value);
					}

					public partial class SecondNestedClass : BindableObject
					{
						[BindableProperty]
						public object Value
						{
							get => GetValue(ValueProperty);
							set => SetValue(ValueProperty, value);
						}
					}
				}
			}
			""";

		const string firstOutput = """
			#nullable enable
			namespace Test
			{
				partial class RootClass
				{
					partial class FirstNestedClass
					{
						partial void OnValueChanging(object oldValue, object newValue);
			
						partial void OnValueChanged(object oldValue, object newValue);
			
						public static readonly global::Microsoft.Maui.Controls.BindableProperty ValueProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
							"Value",
							typeof(object),
							typeof(global::Test.RootClass.FirstNestedClass),
							null,
							global::Microsoft.Maui.Controls.BindingMode.OneWay,
							null,
							(bindable, oldValue, newValue) => ((global::Test.RootClass.FirstNestedClass)bindable).OnValueChanging((object)oldValue, (object)newValue),
							(bindable, oldValue, newValue) => ((global::Test.RootClass.FirstNestedClass)bindable).OnValueChanged((object)oldValue, (object)newValue),
							null,
							null);
			
					}
				}
			}
			""";

		const string secondOutput = """
			#nullable enable
			namespace Test
			{
				partial class RootClass
				{
					partial class FirstNestedClass
					{
						partial class SecondNestedClass
						{
							partial void OnValueChanging(object oldValue, object newValue);
			
							partial void OnValueChanged(object oldValue, object newValue);
			
							public static readonly global::Microsoft.Maui.Controls.BindableProperty ValueProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
								"Value",
								typeof(object),
								typeof(global::Test.RootClass.FirstNestedClass.SecondNestedClass),
								null,
								global::Microsoft.Maui.Controls.BindingMode.OneWay,
								null,
								(bindable, oldValue, newValue) => ((global::Test.RootClass.FirstNestedClass.SecondNestedClass)bindable).OnValueChanging((object)oldValue, (object)newValue),
								(bindable, oldValue, newValue) => ((global::Test.RootClass.FirstNestedClass.SecondNestedClass)bindable).OnValueChanged((object)oldValue, (object)newValue),
								null,
								null);
			
						}
					}
				}
			}
			""";

		await CreateTest(code)
			.ExpectOutput("Test.RootClass+FirstNestedClass", firstOutput)
			.ExpectOutput("Test.RootClass+FirstNestedClass+SecondNestedClass", secondOutput)
			.RunAsync();
	}
}
