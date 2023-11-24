﻿using System.Collections.Immutable;
using System.Text;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Prefix = "PNUM";
	private const string Category = "Pick.Net.Utilities.Maui.SourceGenerators.BindablePropertyGenerator";

	private static string EnumText<T>() where T : unmanaged, Enum
	{
		var names = Enum.GetNames(typeof(T));
		return ListText(names);
	}

	private static string ListText(string[] values)
	{
		if (values.Length < 2)
			throw new ArgumentException("Must supply at leas 2 values", nameof(values));
		
		var sb = new StringBuilder();
		var i = 1;

		sb.Append(values[0]);

		while (true)
		{
			var value = values[i];
			if (++i == values.Length)
				return sb.Append(" or ").Append(value).ToString();

			sb.Append(", ").Append(value);
		}
	}

	public static readonly DiagnosticDescriptor BindablePropertyDuplicateName = new(
		Prefix + "0001",
		"placeholder",
		"Duplicate BindableProperty: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid BindingMode value ({EnumText<BindingMode>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		Prefix + "0002",
		"Supplied DefaultMode value is not a known value",
		"Unknown DefaultMode value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid BindingMode value ({EnumText<BindingMode>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueAndFactory = new(
		Prefix + "0003",
		"Value of DefaultValue will not be used",
		"Setting DefaultValue will have no effect when DefaultValueFactory is true",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"The value passed into DefaultValue will be ignored if DefaultValueFactory is true.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNull = new(
		Prefix + "0004",
		"No default value or value generator for non-nullable property",
		"The default value of non-nullable property {0} will be null",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"A property that is a non-nullable reference should specify a default value or use a default value generator.");

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, SyntaxReference? owner, params object?[] messageArgs)
	{
		var location = owner == null ? null : Location.Create(owner.SyntaxTree, owner.Span);
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, Location? location, params object?[] messageArgs)
	{
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}
}
