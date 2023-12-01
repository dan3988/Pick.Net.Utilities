using System.Text;

using Pick.Net.Utilities.Maui.Helpers;
using Pick.Net.Utilities.Maui.SourceGenerators.Generators;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Prefix = "PNUM";

	private static readonly string Category = typeof(BindablePropertyGenerator).FullName;

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

	public static readonly DiagnosticDescriptor BindablePropertyInstanceToAttached = new(
		Prefix + "1001",
		"Convert instance property to an attached property",
		"Convert instance property to an attached property",
		Category,
		DiagnosticSeverity.Hidden,
		true);

	public static readonly DiagnosticDescriptor BindablePropertyAttachedToInstance = new(
		Prefix + "1002",
		"Convert attached property to an instance property",
		"Convert attached property to an instance property",
		Category,
		DiagnosticSeverity.Hidden,
		true);

	public static readonly DiagnosticDescriptor BindablePropertyAttachedMethodToPartial = new(
		Prefix + "1003",
		"Use partial methods for attached property accessors",
		"Attached property accessor method '{0}' can be auto-implemented",
		Category,
		DiagnosticSeverity.Hidden,
		true,
		$"Methods with [BindableProperty] attribute can be declared as partial methods.");

	public static readonly DiagnosticDescriptor BindablePropertyDuplicateName = new(
		Prefix + "0001",
		"Duplicate [BindableProperty] property name",
		"Duplicate [BindableProperty]: '{0}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"[BindableProperty] attribute cannot be used to generate more than one attached or instance properties with the same name.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodName = new(
		Prefix + "0002",
		"Method name should start with 'Get'",
		"Attached property accessor '{0}' should start with 'Get'",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"[BindableProperty] method names should be in the format 'Get<PropertyName>'.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodReturn = new(
		Prefix + "0003",
		"Method must have a return type",
		"Attached property accessor '{0}' does not have a return type",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Methods annotated with [BindableProperty] must have a return type.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodSignature = new(
		Prefix + "0004",
		"Method must have a return type",
		"Attached property accessor '{0}' must have a single parameter",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Methods annotated with [BindableProperty] must have a single parameter representing the attached type.");

	public static readonly DiagnosticDescriptor BindablePropertyStaticProperty = new(
		Prefix + "0005",
		"Property cannot be static",
		"Property with [BindableProperty] must be an instance property",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Properties annotated with [BindableProperty] cannot be static.");

	public static readonly DiagnosticDescriptor BindablePropertyInstanceMethod = new(
		Prefix + "0006",
		"Methods must be static",
		"Methods with [BindableProperty] must be a static",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Methods annotated with [BindableProperty] must be static.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidDefaultMode = new(
		Prefix + "0007",
		"Supplied DefaultMode value is not a known value",
		"Unknown DefaultMode value: {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid BindingMode value ({EnumText<BindingMode>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNotSupported = new(
		Prefix + "0008",
		"DefaultValue not supported for given property type",
		"Cannot specify DefaultValue for property type {0}",
		Category,
		DiagnosticSeverity.Error,
		true,
		"Cannot specify DefaultValue on for properties with types not supported by attributes.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueAndFactory = new(
		Prefix + "0009",
		"Value of DefaultValue will not be used",
		"Setting DefaultValue will have no effect when DefaultValueFactory is true",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"The value passed into DefaultValue will be ignored if DefaultValueFactory is true.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNull = new(
		Prefix + "0010",
		"No default value or value generator for non-nullable property",
		"The default value of non-nullable property '{0}' will be null",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"A property that is a non-nullable reference should specify a default value or use a default value generator.");

	public static readonly DiagnosticDescriptor BindablePropertyNoGetter = new(
		Prefix + "0011",
		"Property must have a getter",
		"Property '{0}' is write only",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Properties with [BindableProperty] attribute must have a get accessor.");

	public static readonly DiagnosticDescriptor BindablePropertyInstancePropertyNotUsed = new(
		Prefix + "0012",
		"Use generated BindableProperty in property accessors",
		"Bindable property '{0}' does not use generated BindableProperty",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Properties with [BindableProperty] attribute should use the BindableProperty instance that was generated.");

	public static readonly DiagnosticDescriptor BindablePropertyAttachedPropertyNotUsed = new(
		Prefix + "0013",
		"Use generated BindableProperty in attached property accessor",
		"Attached property accessor method '{0}' does not use generated BindableProperty",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Methods with [BindableProperty] attribute should use the BindableProperty instance that was generated.");

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, SyntaxNode owner, params object?[] messageArgs)
	{
		var location = Location.Create(owner.SyntaxTree, owner.Span);
		return Diagnostic.Create(descriptor, location, messageArgs);
	}

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, ISymbol owner, params object?[] messageArgs)
	{
		var location = owner.Locations.FirstOrDefault();
		return Diagnostic.Create(descriptor, location, messageArgs);
	}

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, SyntaxReference? owner, params object?[] messageArgs)
	{
		var location = owner == null ? null : Location.Create(owner.SyntaxTree, owner.Span);
		return Diagnostic.Create(descriptor, location, messageArgs);
	}

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, SyntaxNode owner, params object?[] messageArgs)
	{
		var location = Location.Create(owner.SyntaxTree, owner.Span);
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}

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
