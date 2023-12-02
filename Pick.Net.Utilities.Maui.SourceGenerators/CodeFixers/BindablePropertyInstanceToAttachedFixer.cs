using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyInstanceToAttachedFixer() : BaseCodeFixProvider<PropertyDeclarationSyntax>("Make property type nullable", DiagnosticDescriptors.BindablePropertyInstanceToAttached)
{
	private static SyntaxTokenList AlterModifiers(SyntaxTokenList modifiers)
	{
		if (!modifiers.Contains(SyntaxKind.StaticKeyword))
			modifiers = modifiers.Add(SyntaxKind.StaticKeyword);

		return modifiers.Add(SyntaxKind.PartialKeyword);
	}

	protected override bool Fix(DocumentEditor editor, PropertyDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var propertyType = node.Type;
		var propertyName = node.Identifier.Text;
		var generator = editor.Generator;
		var attachedType = BindablePropertyFixHelper.GetTypeIdentifier(editor.SemanticModel, generator, Identifiers.BindableObject);
		var getMethod = ((MethodDeclarationSyntax)generator.MethodDeclaration("Get" + propertyName))
			.WithSemicolonToken()
			.WithBody(null)
			.WithReturnType(propertyType)
			.WithModifiers(AlterModifiers(node.Modifiers))
			.WithAttributeLists(node.AttributeLists)
			.AddParameterListParameters(
				Parameter(Identifier("obj")).WithType(attachedType));

		editor.ReplaceNode(node, getMethod);

		var setAccessor = node.AccessorList?.Accessors.FirstOrDefault(v => v.Kind() is SyntaxKind.SetAccessorDeclaration or SyntaxKind.InitAccessorDeclaration);
		if (setAccessor != null)
		{
			var setMethod = ((MethodDeclarationSyntax)generator.MethodDeclaration("Set" + propertyName))
				.WithSemicolonToken()
				.WithBody(null)
				.WithModifiers(AlterModifiers(setAccessor.Modifiers.Count == 0 ? node.Modifiers : setAccessor.Modifiers))
				.AddParameterListParameters(
					Parameter(Identifier("obj")).WithType(attachedType),
					Parameter(Identifier("value")).WithType(node.Type));

			editor.InsertAfter(node, setMethod);
		}

		return true;
	}
}
