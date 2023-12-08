using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests.GeneratorTests;

[TestClass]
public class DefaultValueTests : CodeGeneratorTests<BindablePropertyGenerator>
{
	[TestMethod]
	public async Task DefaultValueField()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
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
			using Pick.Net.Utilities.Maui;
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
			using Pick.Net.Utilities.Maui;
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
	public async Task DefaultGeneratorConversion()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private byte CreateInteger() => 5;

				[BindableProperty(DefaultValue = nameof(CreateInteger))]
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
					null,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanging((int)oldValue, (int)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanged((int)oldValue, (int)newValue),
					null,
					(bindable) => (int)((global::TestClass)bindable).CreateInteger());

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task DefaultGeneratorBoxingConversion()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private int CreateInteger() => 5;

				[BindableProperty(DefaultValue = nameof(CreateInteger))]
				public IConvertible Integer
				{
					get => (IConvertible)GetValue(IntegerProperty);
					set => SetValue(IntegerProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				partial void OnIntegerChanging(global::System.IConvertible oldValue, global::System.IConvertible newValue);

				partial void OnIntegerChanged(global::System.IConvertible oldValue, global::System.IConvertible newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty IntegerProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"Integer",
					typeof(global::System.IConvertible),
					typeof(global::TestClass),
					null,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanging((global::System.IConvertible)oldValue, (global::System.IConvertible)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnIntegerChanged((global::System.IConvertible)oldValue, (global::System.IConvertible)newValue),
					null,
					(bindable) => ((global::TestClass)bindable).CreateInteger());

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task DefaultGeneratorImplititOperatorConversion()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public class TestValue(int value)
			{
				private readonly int _value = value;

				public static implicit operator TestValue(int value) => new(value);
			}

			public partial class TestClass : BindableObject
			{
				private int CreateTestValue() => 5;

				[BindableProperty(DefaultValue = nameof(CreateTestValue))]
				public TestValue TestValue
				{
					get => (TestValue)GetValue(TestValueProperty);
					set => SetValue(TestValueProperty, value);
				}
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				partial void OnTestValueChanging(global::TestValue oldValue, global::TestValue newValue);

				partial void OnTestValueChanged(global::TestValue oldValue, global::TestValue newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty TestValueProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(
					"TestValue",
					typeof(global::TestValue),
					typeof(global::TestClass),
					null,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnTestValueChanging((global::TestValue)oldValue, (global::TestValue)newValue),
					(bindable, oldValue, newValue) => ((global::TestClass)bindable).OnTestValueChanged((global::TestValue)oldValue, (global::TestValue)newValue),
					null,
					(bindable) => (global::TestValue)((global::TestClass)bindable).CreateTestValue());

			}
			""";

		await CreateTest(code)
			.ExpectOutput("TestClass", output)
			.RunAsync();
	}

	[TestMethod]
	public async Task DefaultGeneratorDifferentAttachedType()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private static string CreateValue(Element view) => "";

				[BindableProperty(DefaultValue = nameof(CreateValue))]
				public static partial string GetValue(Entry entry);
			}
			""";

		const string output = """
			#nullable enable
			partial class TestClass
			{
				static partial void OnValueChanging(global::Microsoft.Maui.Controls.Entry bindable, string oldValue, string newValue);

				static partial void OnValueChanged(global::Microsoft.Maui.Controls.Entry bindable, string oldValue, string newValue);

				public static readonly global::Microsoft.Maui.Controls.BindableProperty ValueProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(
					"Value",
					typeof(string),
					typeof(global::TestClass),
					null,
					global::Microsoft.Maui.Controls.BindingMode.OneWay,
					null,
					(bindable, oldValue, newValue) => OnValueChanging((global::Microsoft.Maui.Controls.Entry)bindable, (string)oldValue, (string)newValue),
					(bindable, oldValue, newValue) => OnValueChanged((global::Microsoft.Maui.Controls.Entry)bindable, (string)oldValue, (string)newValue),
					null,
					(bindable) => CreateValue((global::Microsoft.Maui.Controls.Element)bindable));

				public static partial string GetValue(global::Microsoft.Maui.Controls.Entry entry) 
					=> (string)entry.GetValue(ValueProperty);

				public static void SetValue(global::Microsoft.Maui.Controls.Entry entry, string value) 
					=> entry.SetValue(ValueProperty, value);

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
			using Pick.Net.Utilities.Maui;
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
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private int InvalidDefaultValue;

				[BindableProperty(DefaultValue = nameof(InvalidDefaultValue))]
				public int Integer { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueFieldNonStatic, 9, 13, 7, "InvalidDefaultValue")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForAmbiguous()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
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

	[TestMethod]
	public async Task ErrorForVoidReturnType()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private void CreateValue()
				{
				}

				[BindableProperty(DefaultValue = nameof(CreateValue))]
				public int Value { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueGeneratorNoReturnType, 11, 13, 5, "CreateValue", "int")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForInvalidReturnType()
	{
		const string code = """
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;

			public partial class TestClass : BindableObject
			{
				private string CreateValue() => "";

				[BindableProperty(DefaultValue = nameof(CreateValue))]
				public int Value { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyDefaultValueGeneratorWrongReturnType, 9, 13, 5, "CreateValue", "int")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForInvalidAttachedType()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;
			
			public partial class TestClass : BindableObject
			{
				private static string CreateValue(Element view) => "";
			
				[BindableProperty(DefaultValue = nameof(CreateValue))]
				public static partial string GetValue(BindableObject element);
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedDefaultValueGeneratorInvalidSignature, 10, 31, 8, "CreateValue")
			.RunAsync();
	}

	[TestMethod]
	public async Task ErrorForInvalidInstanceType()
	{
		const string code = """
			using System;
			using Pick.Net.Utilities.Maui;
			using Microsoft.Maui.Controls;
			
			public partial class TestClass : BindableObject
			{
				private static string CreateValue(Element element) => "";
			
				[BindableProperty(DefaultValue = nameof(CreateValue))]
				public string Value { get; set; }
			}
			""";

		await CreateTest(code)
			.ExpectDiagnostic(DiagnosticDescriptors.BindablePropertyInstanceDefaultValueGeneratorInvalidSignature, 10, 16, 5, "CreateValue")
			.RunAsync();
	}
}
