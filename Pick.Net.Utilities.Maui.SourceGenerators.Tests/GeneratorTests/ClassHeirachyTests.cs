using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class ClassHeirachyTests : CodeGeneratorTest<BindablePropertyGenerator>
{
	[TestMethod]
	public void TestRootLevelClass()
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

		var compilation = CreateCompilation(code, out var tree);
		var driver = CreateDriver(tree.Options);

		driver.RunGeneratorsAndUpdateCompilation(compilation, out var generatorCompilation, out var generatorDiagnostics);

		CheckDiagnostics(generatorDiagnostics);
		CheckGeneratedCode(generatorCompilation, "RootLevelClass", output);
	}

	[TestMethod]
	public void NestedClasses()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			namespace Test;

			public partial class RootClass : BindableObject
			{
				public partial class FirstNestedClass
				{
					[BindableProperty]
					public object Value
					{
						get => GetValue(ValueProperty);
						set => SetValue(ValueProperty, value);
					}

					public partial class SecondNestedClass
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

		var compilation = CreateCompilation(code, out var tree);
		var driver = CreateDriver(tree.Options);

		driver.RunGeneratorsAndUpdateCompilation(compilation, out var generatorCompilation, out var generatorDiagnostics);

		CheckDiagnostics(generatorDiagnostics);
		CheckGeneratedCode(generatorCompilation, "Test.RootClass+FirstNestedClass", firstOutput);
		CheckGeneratedCode(generatorCompilation, "Test.RootClass+FirstNestedClass+SecondNestedClass", secondOutput);
	}
}
