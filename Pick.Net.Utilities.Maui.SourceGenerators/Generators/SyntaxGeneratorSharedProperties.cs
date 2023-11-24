namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct SyntaxGeneratorSharedProperties(
	SyntaxReference? Owner,
	INamedTypeSymbol DeclaringType,
	string PropertyName,
	ITypeSymbol PropertyType,
	Accessibility Accessibility,
	Accessibility WriteAccessibility,
	ExpressionSyntax DefaultValueSyntax,
	ExpressionSyntax DefaultModeSyntax,
	bool DefaultValueFactory,
	bool CoerceValueCallback,
	bool ValidateValueCallback);