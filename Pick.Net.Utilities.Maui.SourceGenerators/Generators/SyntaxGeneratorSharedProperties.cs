namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct SyntaxGeneratorSharedProperties(
	string PropertyName,
	TypeSyntax DeclaringType,
	TypeSyntax PropertyType,
	TypeSyntax AnnotatedPropertyType,
	Accessibility Accessibility,
	Accessibility WriteAccessibility,
	DefaultValueGenerator DefaultValue,
	ExpressionSyntax DefaultModeSyntax,
	MethodSignature PropertyChangingSignature,
	MethodSignature PropertyChangedSignature,
	bool CoerceValueCallback,
	bool ValidateValueCallback);