using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class DiagnosticDescriptors
{
	private const string Prefix = "PNUM";

	private const string Category = "Pick.Net.Utilities.Maui.SourceGenerators.Generators.BindablePropertyGenerator";

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

	public static readonly DiagnosticDescriptor BindablePropertyNoDefaultValue = new(
		Prefix + "1004",
		"Add a default value",
		"Add a default value for property {0}",
		Category,
		DiagnosticSeverity.Hidden,
		true);

	public static readonly DiagnosticDescriptor BindablePropertyDuplicateName = new(
		Prefix + "0001",
		"Duplicate [BindableProperty] property name",
		"Duplicate [BindableProperty]: '{0}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"[BindableProperty] attribute cannot be used to generate more than one attached or instance properties with the same name on the same type.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodName = new(
		Prefix + "0002",
		"Method name should start with 'Get'",
		"Attached property accessor '{0}' should start with 'Get'",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Methods annotated with the [BindableProperty] attribute should be have a 'Get' prefix.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodReturn = new(
		Prefix + "0003",
		"Attached property accessor does not return a value",
		"Attached property accessor '{0}' does not return a value",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Methods annotated with [BindableProperty] must retrun a value.");

	public static readonly DiagnosticDescriptor BindablePropertyInvalidAttachedMethodSignature = new(
		Prefix + "0004",
		"Attached property accessors must have a single parameter",
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
		"Property '{0}' has specified an invalid DefaultMode value: {1}",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"Must use a valid BindingMode value ({EnumText<BindingMode>()}).");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueNull = new(
		Prefix + "0008",
		"No default value or value generator for non-nullable property",
		"The default value of non-nullable property '{0}' will be null",
		Category,
		DiagnosticSeverity.Warning,
		true,
		"A property that is a non-nullable reference should specify a default value or use a default value generator.");

	public static readonly DiagnosticDescriptor BindablePropertyInstancePropertyNotUsed = new(
		Prefix + "0009",
		"Use generated BindableProperty in property accessors",
		"Bindable property '{0}' does not use generated BindableProperty",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Properties with [BindableProperty] attribute should use the BindableProperty instance that was generated.");

	public static readonly DiagnosticDescriptor BindablePropertyAttachedPropertyNotUsed = new(
		Prefix + "0010",
		"Use generated BindableProperty in attached property accessors",
		"Attached property accessor method '{0}' does not use generated BindableProperty",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"Methods with [BindableProperty] attribute should use the BindableProperty instance that was generated.");

	public static readonly DiagnosticDescriptor BindablePropertyAttachedPropertyNullabilityMismatch = new(
		Prefix + "0011",
		"Nullability of attached property get and set methods do not match",
		"Nullability get and set methods for attached property '{0}' do not match",
		Category,
		DiagnosticSeverity.Warning,
		true,
		$"The nullability of the value parameter of set methods for attached properties should match the return value of the get method.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueMemberNotFound = new(
		Prefix + "0012",
		"DefaultValue member not found",
		"DefaultValue member '{0}' not found in type '{1}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The value of BindablePropertyAttribute.DefaultValue should be a member of the current type.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueMemberAmbiguous = new(
		Prefix + "0013",
		"Multiple members found for DefaultValue",
		"Multiple members with name '{0}' found in type '{1}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The provided value of BindablePropertyAttribute.DefaultValue had multiple matches.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueFieldNonStatic = new(
		Prefix + "0014",
		"DefaultValue field or property must be static",
		"DefaultValue field or property '{0}' is not static",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"A field or property used for a default [BindableProperty] value must be static to be accessed when the property is created.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueWrongType = new(
		Prefix + "0015",
		"DefaultValue member type is not assignable to property type",
		"The type of member '{0}' is not assignable to the property type '{1}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The type of default value fields/properties must be assignable to their corresponding property type.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueGeneratorNoReturnType = new(
		Prefix + "0016",
		"DefaultValue generator return type is not assignable to property type",
		"The default value generator '{0}' does not return a value",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The return type of default value generator methods must be assignable to their corresponding property type.");

	public static readonly DiagnosticDescriptor BindablePropertyDefaultValueGeneratorWrongReturnType = new(
		Prefix + "0017",
		"DefaultValue generator return type is not assignable to property type",
		"The return type of default value generator '{0}' is not assignable to the property type '{1}'",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The return type of default value generator methods must be assignable to their corresponding property type.");

	public static readonly DiagnosticDescriptor BindablePropertyInstanceDefaultValueGeneratorInvalidSignature = new(
		Prefix + "0018",
		"DefaultValue generator is not a valid default value or generator",
		"DefaultValue generator '{0}' is not a valid default value generator",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"A default value generator for an instance [BindableProperty] must be parameterless or be static with a single parameter that is assignable to the declaring type.");

	public static readonly DiagnosticDescriptor BindablePropertyAttachedDefaultValueGeneratorInvalidSignature = new(
		Prefix + "0019",
		"DefaultValue generator is not a valid default value or generator for an attached property",
		"DefaultValue generator '{0}' is not a valid default value generator for an attached property",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"A default value generator for an attached [BindableProperty] must be static and either parameterless or have a single parameter that is assignable to the attached type.");

	public static readonly DiagnosticDescriptor BindablePropertyAttributeOnIndexer = new(
		Prefix + "0020",
		"[BindableProperty] cannot be used on indexer declarations",
		"[BindableProperty] cannot be used on indexer declarations",
		Category,
		DiagnosticSeverity.Error,
		true,
		$"The [BindableProperty] attribute cannot be used on an indexer declaration.");

	public static Location ToLocation(this SyntaxNode node)
		=> Location.Create(node.SyntaxTree, node.Span);

	public static Location ToLocation(this SyntaxReference node)
		=> Location.Create(node.SyntaxTree, node.Span);

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, SyntaxNode? owner, params object?[] messageArgs)
		=> Diagnostic.Create(descriptor, owner?.ToLocation(), messageArgs);

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, SyntaxReference? owner, params object?[] messageArgs)
		=> Diagnostic.Create(descriptor, owner?.ToLocation(), messageArgs);

	public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, ISymbol owner, params object?[] messageArgs)
	{
		var location = owner.Locations.FirstOrDefault();
		return Diagnostic.Create(descriptor, location, messageArgs);
	}

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, SyntaxNode? owner, params object?[] messageArgs)
		=> Add(builder, descriptor, owner?.ToLocation(), messageArgs);

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, SyntaxReference? owner, params object?[] messageArgs)
		=> Add(builder, descriptor, owner?.ToLocation(), messageArgs);

	public static void Add(this ImmutableArray<Diagnostic>.Builder builder, DiagnosticDescriptor descriptor, Location? location, params object?[] messageArgs)
	{
		var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
		builder.Add(diagnostic);
	}
}
