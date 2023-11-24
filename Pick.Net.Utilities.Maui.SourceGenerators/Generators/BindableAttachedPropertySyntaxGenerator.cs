namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	protected override string CreateMethod => "CreateAttached";

	protected override string CreateReadOnlyMethod => "CreateAttachedReadOnly";

	public ITypeSymbol AttachedType { get; }

	internal BindableAttachedPropertySyntaxGenerator(SyntaxGeneratorSharedProperties properties, ITypeSymbol attachedType) : base(properties)
	{
		AttachedType = attachedType;
	}
}