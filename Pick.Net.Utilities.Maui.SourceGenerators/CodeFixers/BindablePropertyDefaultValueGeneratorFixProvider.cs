using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindablePropertyDefaultValueGeneratorFixProvider() : BaseCodeFixProvider<MemberDeclarationSyntax>("Add default value generator", DiagnosticDescriptors.BindablePropertyNoDefaultValue)
{
	protected override bool CanFix(MemberDeclarationSyntax node)
		=> node.IsKind(SyntaxKind.PropertyDeclaration) || node.IsKind(SyntaxKind.MethodDeclaration);

	protected override bool Fix(DocumentEditor editor, MemberDeclarationSyntax node, Diagnostic diagnostic, CancellationToken token)
	{
		string name;
		MethodDeclarationSyntax method;
		AttributeListSyntax? attributeList;
		int attributeIndex;

		var exceptionType = editor.SemanticModel.GetTypeIdentifier(editor.Generator, "System.NotImplementedException");
		var notnull = (ObjectCreationExpressionSyntax)editor.Generator.ObjectCreationExpression(exceptionType);
		var body = (ThrowStatementSyntax)editor.Generator.ThrowStatement(notnull);

		if (node.IsKind(SyntaxKind.PropertyDeclaration))
		{
			var prop = (PropertyDeclarationSyntax)node;
			name = "Create" + prop.Identifier.Text;
			method = (MethodDeclarationSyntax)editor.Generator.MethodDeclaration(name, null, null, prop.Type, Accessibility.Private, default, [body]);
			attributeList = BindablePropertyFixHelper.GetAttribute(editor.SemanticModel, prop, out attributeIndex, token);
		}
		else
		{
			var m = (MethodDeclarationSyntax)node;
			if (m.ParameterList.Parameters.Count == 0)
				return false;

			var attachedTypeParam = m.ParameterList.Parameters[0];
			var parameter = editor.Generator.ParameterDeclaration(attachedTypeParam.Identifier.Text, attachedTypeParam.Type);

			name = "Create" + Identifiers.GetAttachedPropertyName(m.Identifier.Text);
			method = (MethodDeclarationSyntax)editor.Generator.MethodDeclaration(name, [parameter], null, m.ReturnType, Accessibility.Private, DeclarationModifiers.Static, [body]);
			attributeList = BindablePropertyFixHelper.GetAttribute(editor.SemanticModel, m, out attributeIndex, token);
		}

		if (attributeList == null)
			return false;

		var attribute = attributeList.Attributes[attributeIndex];
		var attributeArg = editor.Generator.AttributeArgument(nameof(BindablePropertyAttribute.DefaultValue), SyntaxHelper.NameOf(name));
		editor.ReplaceNode(attributeList, editor.Generator.AddAttributeArguments(attribute, [attributeArg]));
		editor.InsertBefore(node, method);
		return true;
	}
}
