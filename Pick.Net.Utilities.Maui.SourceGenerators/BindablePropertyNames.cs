namespace Pick.Net.Utilities.Maui.SourceGenerators;

using static SyntaxFactory;

internal static class BindablePropertyNames
{
	public static readonly IdentifierNameSyntax Value = IdentifierName("value");
	public static readonly IdentifierNameSyntax GetValue = IdentifierName("GetValue");
	public static readonly IdentifierNameSyntax SetValue = IdentifierName("SetValue");

	public const string BindableObject = "Microsoft.Maui.Controls.BindableObject";

	public static readonly IdentifierNameSyntax BindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	public static readonly IdentifierNameSyntax BindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	public static readonly IdentifierNameSyntax BindablePropertyKeyProperty = IdentifierName("BindableProperty");
}
