﻿using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyInstanceToAttachedFixProvider() : BaseCodeFixProvider<PropertyDeclarationSyntax>("Make property type nullable", DiagnosticDescriptors.BindablePropertyInstanceToAttached)
{
	private static SyntaxTokenList AlterModifiers(SyntaxTokenList modifiers)
	{
		if (!modifiers.Contains(SyntaxKind.StaticKeyword))
			modifiers = modifiers.Add(Keywords.Static);

		return modifiers.Add(Keywords.Partial);
	}

	protected override bool Fix(DocumentEditor editor, PropertyDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		var propertyType = node.Type;
		var propertyName = node.Identifier.Text;
		var generator = editor.Generator;
		var attachedType = editor.SemanticModel.GetTypeIdentifier(generator, IdentifierNames.BindableObject);
		var getMethod = ((MethodDeclarationSyntax)generator.MethodDeclaration("Get" + propertyName))
			.WithSemicolonToken()
			.WithBody(null)
			.WithReturnType(propertyType)
			.WithModifiers(AlterModifiers(node.Modifiers))
			.WithAttributeLists(node.AttributeLists)
			.AddParameterListParameters(
				Parameter(Identifier("obj")).WithType(attachedType));

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

		editor.ReplaceNode(node, getMethod);

		return true;
	}
}
