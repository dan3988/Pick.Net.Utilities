﻿using Pick.Net.Utilities.Maui.SourceGenerators.Analyzers;
using Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Tests;

[TestClass]
public class BindableAttachedPropertyAccessorFixerTests
{
	private static readonly FixTestFactory<BindableAttachedPropertyMethodAnalyzer, BindableAttachedPropertyMethodToPartialFixProvider> Factory = new();

	[TestMethod]
	public async Task ConvertPropertyGetMethod()
	{
		const string original = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;
	
	namespace Test;
	
	partial class TestClass : BindableObject
	{
		public static readonly BindableProperty WrongProperty;
	
		[BindableProperty]
		public static string GetValue(BindableObject obj)
			=> (string)obj.GetValue(WrongProperty);
	
		public static void SetValue(BindableObject obj, string value)
			=> obj.SetValue(WrongProperty, value);
	}
	""";

		const string expected = """
	using Pick.Net.Utilities.Maui.Helpers;
	using Microsoft.Maui.Controls;

	namespace Test;

	partial class TestClass : BindableObject
	{
		public static readonly BindableProperty WrongProperty;
	
		[BindableProperty]
		public static partial string GetValue(BindableObject obj);

		public static partial void SetValue(BindableObject obj, string value);
	}
	""";

		await Factory.CreateTest(original, expected)
			.ExpectTestDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedPropertyNotUsed, 11, 23, 8, "Value")
			.ExpectTestDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance, 11, 23, 8, "Value")
			.ExpectFixDiagnostic(DiagnosticDescriptors.BindablePropertyAttachedToInstance, 11, 31, 8, "Value")
			.RunAsync();
	}
}