using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using DotNetUtilities.Maui.Helpers;
using DotNetUtilities.Maui.SourceGenerators.Syntax;

namespace DotNetUtilities.Maui.SourceGenerators.Generators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

[Generator]
public sealed class BindableAttachedPropertyGenerator : BaseBindablePropertyGenerator
{
	public sealed class AttachedAttributeParser : AttributeParser
	{
		public INamedTypeSymbol AttachedType { get; }

		public AttachedAttributeParser(INamedTypeSymbol declaringType, AttributeData attribute, string name, INamedTypeSymbol propertyType, INamedTypeSymbol attachedType) : base(declaringType, attribute, name, propertyType)
		{
			AttachedType = attachedType;
		}

		internal override BindablePropertySyntaxGenerator CreateGenerator()
			=> BindableAttachedPropertySyntaxGenerator.Create(CreateParameters(), AttachedType.ToIdentifier(), GetterAccessors, SetterAccessors);
	}

	private static bool TryParseAttributeGenericArgs(AttributeData attribute, [MaybeNullWhen(false)] out INamedTypeSymbol typeSymbol, [MaybeNullWhen(false)] out INamedTypeSymbol attachedType)
	{
		var attrType = attribute.AttributeClass;
		if (attrType is { IsGenericType: true })
		{
			var args = attrType.TypeArguments;
			if (args.Length >= 2)
			{
				typeSymbol = (INamedTypeSymbol)args[0];
				attachedType = (INamedTypeSymbol)args[1];
				return true;
			}
		}

		typeSymbol = null;
		attachedType = null;
		return false;
	}

	public override Type AttributeType => typeof(AttachedBindablePropertyAttribute<,>);

	public override string? FileNameSuffix => "Attached";

	public override AttributeParser? CreateParser(INamedTypeSymbol declaringType, AttributeData data, DiagnosticsBuilder diagnostics)
	{
		if (TryParseAttributePositionalArgs(data, diagnostics, out var name) && TryParseAttributeGenericArgs(data, out var propertyType, out var attachedType))
			return new AttachedAttributeParser(declaringType, data, name, propertyType, attachedType);

		return null;
	}
}
