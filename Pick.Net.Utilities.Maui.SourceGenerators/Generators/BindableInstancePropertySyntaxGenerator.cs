namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal sealed class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	protected override string CreateMethod => "Create";

	protected override string CreateReadOnlyMethod => "CreateReadOnly";

	internal BindableInstancePropertySyntaxGenerator(SyntaxGeneratorSharedProperties properties) : base(properties)
	{
	}
}