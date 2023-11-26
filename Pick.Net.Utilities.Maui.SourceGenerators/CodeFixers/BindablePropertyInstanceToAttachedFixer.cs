using System.Composition;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Simplification;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyInstanceToAttachedFixer : BaseCodeFixProvider<PropertyDeclarationSyntax>
{
	public BindablePropertyInstanceToAttachedFixer() : base(DiagnosticDescriptors.BindablePropertyInstanceToAttached)
	{
	}

	protected override CodeAction? CreateAction(Document document, SyntaxNode root, PropertyDeclarationSyntax node, Diagnostic diagnostic)
		=> CodeAction.Create("To attached property", _ => DoFix(document, root, node));

	private static SyntaxTokenList AlterModifiers(SyntaxTokenList modifiers)
	{
		if (!modifiers.Contains(SyntaxKind.StaticKeyword))
			modifiers = modifiers.Add(SyntaxKind.StaticKeyword);

		return modifiers.Add(SyntaxKind.PartialKeyword);
	}

	private static Task<Document> DoFix(Document document, SyntaxNode root, PropertyDeclarationSyntax node)
	{
		var propertyType = node.Type;
		var propertyName = node.Identifier.Text;
		var members = new List<SyntaxNode>();
		var attachedType = BindablePropertyNames.BindableObject.WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
		var getMethod = MethodDeclaration(propertyType, "Get" + propertyName)
			.WithSemicolonToken()
			.WithModifiers(AlterModifiers(node.Modifiers))
			.AddParameterListParameters(
				Parameter(Identifier("obj")).WithType(attachedType));

		getMethod = getMethod.WithAttributeLists(node.AttributeLists);
		members.Add(getMethod);

		var setAccessor = node.AccessorList?.Accessors.FirstOrDefault(v => v.Kind() is SyntaxKind.SetAccessorDeclaration or SyntaxKind.InitAccessorDeclaration);
		if (setAccessor != null)
		{
			var setMethod = MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + propertyName)
				.WithSemicolonToken()
				.WithModifiers(AlterModifiers(setAccessor.Modifiers.Count == 0 ? node.Modifiers : setAccessor.Modifiers))
				.AddParameterListParameters(
					Parameter(Identifier("obj")).WithType(attachedType),
					Parameter(Identifier("value")).WithType(node.Type));

			members.Add(setMethod);
		}

		root = root.ReplaceNode(node, members);
		document = document.WithSyntaxRoot(root);

		return Task.FromResult(document);
	}
}
