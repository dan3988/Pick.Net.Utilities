using Microsoft.CodeAnalysis.Testing;

using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class DefaultValueTests : CodeGeneratorTests<BindablePropertyGenerator>
{
	[TestMethod]
	public async Task DefaultValueField()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private const int IntegerDefaultValue = 44;

				[BindableProperty(DefaultValue = nameof(IntegerDefaultValue))]
				public int Integer
				{
					get => (int)GetValue(IntegerProperty);
					set => SetValue(IntegerProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				partial void OnIntegerChanging(int oldValue, int newValue);

				partial void OnIntegerChanged(int oldValue, int newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty IntegerProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"Integer",
					typeof(int),
					typeof(global::TestClass),
					IntegerDefaultValue,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanging((int)oldValue, (int)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanged((int)oldValue, (int)newValue),
					null,
					null);

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task DefaultImplicitConversion()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private const byte IntegerDefaultValue = 44;

				[BindableProperty(DefaultValue = nameof(IntegerDefaultValue))]
				public int Integer
				{
					get => (int)GetValue(IntegerProperty);
					set => SetValue(IntegerProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				partial void OnIntegerChanging(int oldValue, int newValue);

				partial void OnIntegerChanged(int oldValue, int newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty IntegerProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"Integer",
					typeof(int),
					typeof(global::TestClass),
					(int)IntegerDefaultValue,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanging((int)oldValue, (int)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanged((int)oldValue, (int)newValue),
					null,
					null);

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task DefaultExplicitConversion()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private const ConsoleColor IntegerDefaultValue = ConsoleColor.Red;

				[BindableProperty(DefaultValue = nameof(IntegerDefaultValue))]
				public int Integer
				{
					get => (int)GetValue(IntegerProperty);
					set => SetValue(IntegerProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				partial void OnIntegerChanging(int oldValue, int newValue);

				partial void OnIntegerChanged(int oldValue, int newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty IntegerProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"Integer",
					typeof(int),
					typeof(global::TestClass),
					(int)IntegerDefaultValue,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanging((int)oldValue, (int)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanged((int)oldValue, (int)newValue),
					null,
					null);

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForNotFound()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				[BindableProperty(DefaultValue = "this does not exist")]
				public object Value { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueMemberNotFound, 7, 16, 5, "this does not exist", "TestClass")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForDefaultInstanceField()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private int InvalidDefaultValue;

				[BindableProperty(DefaultValue = nameof(InvalidDefaultValue))]
				public int Integer { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueMemberInvalid, 9, 13, 7, "InvalidDefaultValue")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForAmbiguous()
	{
		const string code = """
			using Pick.Net.Utilities.Maui.Helpers;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private static int AmbiguousDefaultValue() => 1;
			
				private static int AmbiguousDefaultValue(int value) => value;
			
				[BindableProperty(DefaultValue = nameof(AmbiguousDefaultValue))]
				public int Value { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueMemberAmbiguous, 11, 13, 5, "AmbiguousDefaultValue", "TestClass")
			.RunAsync();
	}
}
