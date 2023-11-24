using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct SyntaxGeneratorSharedProperties(
	SyntaxReference? Owner,
	INamedTypeSymbol DeclaringType,
	string PropertyName,
	ITypeSymbol PropertyType,
	Accessibility Accessibility,
	Accessibility WriteAccessibility,
	BindingMode DefaultMode,
	object? DefaultValue,
	bool DefaultValueFactory,
	bool CoerceValueCallback,
	bool ValidateValueCallback);