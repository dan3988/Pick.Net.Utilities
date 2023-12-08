# Pick.Net.Utilities.Maui.SourceGenerators Diagnostics
## PNUM1001
Used to show code fixer to convert instance properties to attached properties.

## PNUM1002
Used to show code fixer to convert attached properties to instance properties.

## PNUM1003
Used to show code fixer to convert attached property accessors to partial methods.

## PNUM1004
Used to show code fixers for adding a default value to a bindable property.

## PNUM0001
Indicates that a `[BindableProperty]` attribute has been specified on multiple properties with the same name.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static partial string GetValue(BindableObject obj);
	// PNUM0001: Duplicate [BindableProperty]: 'Value'

	[BindableProperty]
	public string Value { get; set; }
	// PNUM0001: Duplicate [BindableProperty]: 'Value'
}
```

## PNUM0002
Indicates that an attached property get method does not have a `Get` prefix.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static partial string Value(BindableObject obj);
	// PNUM0002: Attached property accessor 'Value' should start with 'Get'
}
```

## PNUM0003
Indicates that an attached property get method does not have a return type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static partial void GetValue(BindableObject obj);
	// PNUM0003: Attached property accessor 'GetValue' does not return a value
}
```

## PNUM0004
Indicates that an attached property get does not have a single parameter representing the attached type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static partial void GetValue1();
	// PNUM0004: Attached property accessor 'GetValue1' must have a single parameter

	[BindableProperty]
	public static partial void GetValue2(BindableObject obj, string invalid); 
	// PNUM0004: Attached property accessor 'GetValue2' must have a single parameter
}
```

## PNUM0005
Indicates that a static property has been declared with a `[BindableProperty]` attribute.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static string Value { get; set; }
	// PNUM0005: Property with [BindableProperty] must be an instance property
}
```

## PNUM0006
Indicates that a non-static method has been declared with a `[BindableProperty]` attribute.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public string GetValue();
	// PNUM0006: Methods with [BindableProperty] must be a static
}
```

## PNUM0007
Indicates that an invalid value has been specified for the value of the DefaultMode property.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty(DefaultMode = (BindingMode)(-1))]
	public string Value { get; set; }
	// PNUM0007: Property 'Value' has specified an invalid DefaultMode value: -1
}
```

## PNUM0008
Indicates that bindable property has a non-nullable reference type but has not specified a default value.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public string Value { get; set; }
	// PNUM0008: The default value of non-nullable property 'Value' will be null
}
```

## PNUM0009
Indicates that an accessor of a bindable property does not use the correct bindable property.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	public static readonly BindableProperty DifferentProperty;

	[BindableProperty]
	public string Value
	// PNUM0009: Bindable property 'Value' does not use generated BindableProperty
	{
		get => (string)GetValue(DifferentProperty);
	}

	public static readonly BindableProperty DifferentProperty;

	[BindableProperty]
	public string ReadOnlyValue
	// PNUM0009: Bindable property 'ReadOnlyValue' does not use generated BindableProperty
	{
		get => (string)GetValue(ReadOnlyValueProperty);
		private set => SetValue(ReadOnlyValueProperty, value);
	}
}
```

## PNUM0010
Indicates that an accessor of an attached bindable property does not use the correct bindable property.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	public static readonly BindableProperty DifferentProperty;

	[BindableProperty]
	public static string GetValue(BindableProperty obj) => GetValue(DifferentProperty);
	// PNUM0010: Attached property accessor method '{0}' does not use generated BindableProperty
}
```

## PNUM0011
Indicates that an the nullability of attached property accessors are different.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty]
	public static partial string GetValue(BindableProperty obj);
	// PNUM0011: Nullability get and set methods for attached property 'Value' do not match

	public static partial void SetValue(BindableProperty obj, string? value);
}
```

## PNUM0012
Indicates the value of DefaultValue is not a member of the declaring type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	[BindableProperty(DefaultValue = "ValueDefaultValue")]
	public string Value { get; set; }
	// PNUM0012: DefaultValue member 'ValueDefaultValue' not found in type 'SampleView'
}
```

## PNUM0013
Indicates the value of DefaultValue points to multiple members in declaring type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private const string ValueDefaultValue = "Test";

	private static string ValueDefaultValue() => "Test";

	[BindableProperty(DefaultValue = "ValueDefaultValue")]
	public string Value { get; set; }
	// PNUM0013: Multiple members with name 'ValueDefaultValue' found in type 'SampleView'
}
```

## PNUM0014
Indicates that a DefaultValue field is not assignable to the property return type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private readonly int ValueDefaultValue = 5;

	[BindableProperty(DefaultValue = nameof(ValueDefaultValue))]
	public string Value { get; set; }
	// PNUM0014: DefaultValue field or property 'ValueDefaultValue' is not static
}
```

## PNUM0015
Indicates that a DefaultValue field is not assignable to the property return type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private const int ValueDefaultValue = 5;

	[BindableProperty(DefaultValue = nameof(ValueDefaultValue))]
	public string Value { get; set; }
	// PNUM0015: The type of member 'ValueDefaultValue' is not assignable to the property type 'string'
}
```

## PNUM0016
Indicates that a DefaultValue generator does not return a value
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private static void CreateDefaultValue()
	{
	}

	[BindableProperty(DefaultValue = nameof(ValueDefaultValue))]
	public string Value { get; set; }
	// PNUM0016: The default value generator 'CreateDefaultValue' does not return a value
}
```

## PNUM0017
Indicates that a DefaultValue generator's return type is not assignable to the property return type.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private static int CreateDefaultValue() => 5;

	[BindableProperty(DefaultValue = nameof(CreateDefaultValue))]
	public string Value { get; set; }
	// PNUM0017: The type of member 'CreateDefaultValue' is not assignable to the property type 'string'
}
```

## PNUM0018
Indicates that a DefaultValue generator method for an instance property has an invalid signature.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private static string CreateDefaultValue(string parameter) => "";

	[BindableProperty(DefaultValue = nameof(CreateDefaultValue))]
	public string Value { get; set; }
	// PNUM0018: DefaultValue generator 'CreateDefaultValue' is not a valid default value generator

	private static string CreateAttachedValue(View obj, string parameter) => "";

	[BindableProperty(DefaultValue = nameof(CreateAttachedValue))]
	public static partial string GetAttachedValue(View obj);
	// PNUM0018: DefaultValue generator 'CreateDefaultValue' is not a valid default value generator
}
```

## PNUM0019
Indicates that a DefaultValue generator method for an attached property has an invalid signature.
```C#
using Pick.Net.Utilities.Maui.Helpers;

public partial class SampleView : View
{
	private string CreateDefaultValue() => "";

	[BindableProperty(DefaultValue = nameof(CreateDefaultValue))]
	public string Value { get; set; }
	// PNUM0019: DefaultValue generator 'CreateDefaultValue' is not a valid default value generator for an attached property

	private static string CreateAttachedValue(View obj, string parameter) => "";

	[BindableProperty(DefaultValue = nameof(CreateAttachedValue))]
	public static partial string GetAttachedValue(View obj);
	// PNUM0018: DefaultValue generator 'CreateDefaultValue' is not a valid default value generator
}
```
