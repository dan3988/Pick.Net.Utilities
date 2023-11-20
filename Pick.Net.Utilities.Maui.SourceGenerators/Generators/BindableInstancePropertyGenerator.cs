using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Pick.Net.Utilities.Maui.Helpers;
using Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

[Generator]
public sealed class BindableInstancePropertyGenerator : BaseBindablePropertyGenerator
{
	private sealed class InstanceAttributeParser : AttributeParser
	{
		public InstanceAttributeParser(INamedTypeSymbol declaringType, AttributeData attribute, string name, INamedTypeSymbol propertyType) : base(declaringType, attribute, name, propertyType)
		{
		}

		internal override BindablePropertySyntaxGenerator CreateGenerator()
			=> BindableInstancePropertySyntaxGenerator.Create(CreateParameters(), GetterAccessors, SetterAccessors);
	}

	private static bool TryParseAttributeGenericArgs(AttributeData attribute, [MaybeNullWhen(false)] out INamedTypeSymbol typeSymbol)
	{
		var attrType = attribute.AttributeClass;
		if (attrType is { IsGenericType: true })
		{
			var args = attrType.TypeArguments;
			if (args.Length >= 1)
			{
				typeSymbol = (INamedTypeSymbol)args[0];
				return true;
			}
		}

		typeSymbol = null;
		return false;
	}

	public override Type AttributeType => typeof(BindablePropertyAttribute<>);

	public override AttributeParser? CreateParser(INamedTypeSymbol declaringType, AttributeData data, DiagnosticsBuilder diagnostics)
	{
		if (TryParseAttributePositionalArgs(data, diagnostics, out var name) && TryParseAttributeGenericArgs(data, out var propertyType))
			return new InstanceAttributeParser(declaringType, data, name, propertyType);

		return null;
	}
}
