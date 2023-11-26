namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct SyntaxGeneratorSharedProperties(
	string PropertyName,
	TypeSyntax DeclaringType,
	TypeSyntax PropertyType,
	TypeSyntax AnnotatedPropertyType,
	Accessibility Accessibility,
	Accessibility WriteAccessibility,
	ExpressionSyntax DefaultValueSyntax,
	ExpressionSyntax DefaultModeSyntax,
	bool DefaultValueFactory,
	bool CoerceValueCallback,
	bool ValidateValueCallback);