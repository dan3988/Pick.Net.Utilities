using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyDefaultValueFieldFixProvider() : BaseCodeFixProvider<MemberDeclarationSyntax>("Add static default value", DiagnosticDescriptors.BindablePropertyNoDefaultValue)
{
	protected override bool CanFix(MemberDeclarationSyntax node)
		=> node.IsKind(SyntaxKind.PropertyDeclaration) || node.IsKind(SyntaxKind.MethodDeclaration);

	protected override bool Fix(DocumentEditor editor, MemberDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		string name;
		TypeSyntax type;

		AttributeListSyntax? attributeList;
		int attributeIndex;

		if (node.IsKind(SyntaxKind.PropertyDeclaration))
		{
			var prop = (PropertyDeclarationSyntax)node;
			attributeList = BindablePropertyFixHelper.GetAttribute(editor.SemanticModel, prop, out attributeIndex, token);
			name = prop.Identifier.Text;
			type = prop.Type;
		}
		else
		{
			var method = (MethodDeclarationSyntax)node;
			if (method.ParameterList.Parameters.Count == 0)
				return false;

			attributeList = BindablePropertyFixHelper.GetAttribute(editor.SemanticModel, method, out attributeIndex, token);
			name = Identifiers.GetAttachedPropertyName(method.Identifier.Text);
			type = method.ReturnType;
		}

		if (attributeList == null)
			return false;

		name += "DefaultValue";

		var attribute = attributeList.Attributes[attributeIndex];
		var attributeArg = editor.Generator.AttributeArgument(nameof(BindablePropertyAttribute.DefaultValue), SyntaxHelper.NameOf(name));
		editor.ReplaceNode(attributeList, editor.Generator.AddAttributeArguments(attribute, [attributeArg]));

		var typeSymbolInfo = editor.SemanticModel.GetSymbolInfo(type);
		var field = typeSymbolInfo.Symbol is INamedTypeSymbol symbol && symbol.IsPrimitiveType(out var constType)
			? (FieldDeclarationSyntax)editor.Generator.FieldDeclaration(name, constType.ToIdentifier(false), Accessibility.Private, DeclarationModifiers.Const)
			: (FieldDeclarationSyntax)editor.Generator.FieldDeclaration(name, type, Accessibility.Private, DeclarationModifiers.ReadOnly.WithIsStatic(true));

		editor.InsertBefore(node, field);
		return true;
	}
}
