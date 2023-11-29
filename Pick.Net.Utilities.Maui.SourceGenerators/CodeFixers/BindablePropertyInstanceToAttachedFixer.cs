using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyInstanceToAttachedFixer() : BaseCodeFixProvider<PropertyDeclarationSyntax>(DiagnosticDescriptors.BindablePropertyInstanceToAttached)
{
	protected override CodeAction? CreateAction(Document document, SyntaxNode root, PropertyDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("To attached property", token => DoFix(document, root, node, token));

	private static SyntaxTokenList AlterModifiers(SyntaxTokenList modifiers)
	{
		if (!modifiers.Contains(SyntaxKind.StaticKeyword))
			modifiers = modifiers.Add(SyntaxKind.StaticKeyword);

		return modifiers.Add(SyntaxKind.PartialKeyword);
	}

	private static async Task<TypeSyntax> GetTypeIdentifierAsync(Document document, SyntaxGenerator generator, string fullName, CancellationToken token)
	{
		var model = await document.GetSemanticModelAsync(token);
		return BindablePropertyFixHelper.GetTypeIdentifier(model, generator, fullName);

	}

	public static async Task<Document> DoFix(Document document, SyntaxNode root, PropertyDeclarationSyntax node, CancellationToken token)
	{
		var propertyType = node.Type;
		var propertyName = node.Identifier.Text;
		var members = new List<SyntaxNode>();
		var generator = SyntaxGenerator.GetGenerator(document);
		var attachedType = await GetTypeIdentifierAsync(document, generator, Identifiers.BindableObject, token);
		var getMethod = ((MethodDeclarationSyntax)generator.MethodDeclaration("Get" + propertyName))
			.WithSemicolonToken()
			.WithBody(null)
			.WithReturnType(propertyType)
			.WithModifiers(AlterModifiers(node.Modifiers))
			.WithAttributeLists(node.AttributeLists)
			.AddParameterListParameters(
				Parameter(Identifier("obj")).WithType(attachedType));

		members.Add(getMethod);

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

			members.Add(setMethod);
		}

		root = root.ReplaceNode(node, members);
		document = document.WithSyntaxRoot(root);

		return document;
	}
}
