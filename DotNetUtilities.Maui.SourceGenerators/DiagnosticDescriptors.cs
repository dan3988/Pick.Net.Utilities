﻿using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace DotNetUtilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	public static DiagnosticDescriptor BindablePropertyEmptyPropertyName = new(
		"DNU0001",
		"PropertyName cannot be null or empty",
		"PropertyName cannot be null or empty",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyName cannot be null or empty.");

	public static DiagnosticDescriptor BindablePropertyNullPropertyType = new(
		"DNU0002",
		"PropertyType cannot be null",
		"PropertyType cannot be null",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"PropertyType cannot be null.");

	public static DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		"DNU0003",
		"Invalid BindingMode value for [BindableProperty]",
		"Invalid BindingMode value: {0}",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid BindingMode value.");

	public static DiagnosticDescriptor BindablePropertyInvalidVisibility = new(
		"DNU0004",
		"Invalid PropertyVisibility value for [BindableProperty]",
		"Invalid PropertyVisibility value: {0}",
		typeof(BindablePropertyGenerator).FullName,
		DiagnosticSeverity.Error,
		true,
		"Invalid PropertyVisibility value.");

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
