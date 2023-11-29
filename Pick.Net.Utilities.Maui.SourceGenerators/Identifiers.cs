namespace Pick.Net.Utilities.Maui.SourceGenerators;

using System.Xml.Linq;

using Microsoft.CodeAnalysis;

using static SyntaxFactory;

internal static class Identifiers
{
	public static readonly IdentifierNameSyntax Value = IdentifierName("value");
	public static readonly IdentifierNameSyntax GetValue = IdentifierName("GetValue");
	public static readonly IdentifierNameSyntax SetValue = IdentifierName("SetValue");

	public const string BindableObject = "Microsoft.Maui.Controls.BindableObject";

	public static readonly IdentifierNameSyntax BindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	public static readonly IdentifierNameSyntax BindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	public static readonly IdentifierNameSyntax BindablePropertyKeyProperty = IdentifierName("BindableProperty");

	public static string GetAttachedPropertyName(string name)
	{
		GetAttachedPropertyName(name, out name);
		return name;
	}

	public static bool GetAttachedPropertyName(string name, out string result)
	{
		if (name.StartsWith("Get"))
		{
			result = name.Substring(3);
			return true;
		}
		else
		{
			result = name;
			return false;
		}
	}

	public static bool StringStartsAndEndsWith(string value, string start, string end, StringComparison comparison = StringComparison.Ordinal)
		=> value.StartsWith(start) && value.AsSpan(start.Length).Equals(end.AsSpan(), comparison);

	//public static IMethodSymbol? GetAttachedSetMethod(this IMethodSymbol getter, string propertyName)
	//	=> GetAttachedSetMethod(getter, getter.ReturnType, getter.Parameters[0].Type, propertyName);

	public static IMethodSymbol? GetAttachedSetMethod(this ITypeSymbol declaringType, ITypeSymbol propertyType, ITypeSymbol attachedType, string propertyName)
	{
		return declaringType.GetMembers().SelectMethods().FirstOrDefault(IsSetMethod);

		bool IsSetMethod(IMethodSymbol symbol)
		{
			if (!StringStartsAndEndsWith(symbol.Name, "Set", propertyName))
				return false;

			//if (!symbol.ReturnsVoid)
			//	return false;

			if (symbol.Parameters.Length != 2)
				return false;

			var objParam = symbol.Parameters[0];
			var objValue = symbol.Parameters[1];
			return SymbolEqualityComparer.Default.Equals(objParam.Type, attachedType) && SymbolEqualityComparer.Default.Equals(objValue.Type, propertyType);
		}
	}
}
