using System.Composition;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

using static SyntaxFactory;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class BindableInstancePropertyAutoPropertyFixer() : BaseCodeFixProvider<PropertyDeclarationSyntax>("Use BindableProperty in accessors", DiagnosticDescriptors.BindablePropertyInstancePropertyNotUsed)
{
	private static AccessorDeclarationSyntax CopyAttributes(AccessorListSyntax? existing, AccessorDeclarationSyntax syntax)
	{
		var other = existing?.Accessors.OfKind(existing.Kind()).FirstOrDefault();
		return other == null ? syntax : syntax.WithAttributeLists(other.AttributeLists).WithModifiers(other.Modifiers);
	}

	private static SyntaxTriviaList GetIndentation(PropertyDeclarationSyntax prop, out SyntaxTrivia last)
	{
		last = default;

		var trivia = prop.GetLeadingTrivia();
		var list = SyntaxTriviaList.Empty;

		foreach (var item in trivia)
			if (item.IsKind(SyntaxKind.WhitespaceTrivia))
				list = list.Add(last = item);

		return list;
	}

	protected override bool Fix(DocumentEditor editor, PropertyDeclarationSyntax prop, Diagnostic diagnostic, CancellationToken token)
	{
		var list = prop.AccessorList;
		if (list == null)
			return false;

		var ogProp = prop;
		var indent = GetIndentation(prop, out var indentTrivia);
		var accessorIndent = indent.Add(indentTrivia);
		var gen = editor.Generator;
		var name = prop.Identifier.Text;
		var field = IdentifierName(name + "Property");
		var accessors = new List<AccessorDeclarationSyntax>(2);

		foreach (var accessor in list.Accessors)
		{
			switch (accessor.Kind())
			{
				case SyntaxKind.GetAccessorDeclaration:
					GenerateGetter(accessors, gen, accessorIndent, accessor, prop.Type, field);
					break;
				case SyntaxKind.SetAccessorDeclaration:
				case SyntaxKind.InitAccessorDeclaration:
					GenerateSetter(accessors, gen, accessorIndent, accessor, accessor.Modifiers.Count == 0 ? field : IdentifierName(name + "PropertyKey"));
					break;
			}
		}

		var open = Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(indent).WithTrailingTrivia(CarriageReturnLineFeed);
		var close = Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(indent).WithTrailingTrivia(CarriageReturnLineFeed);

		list = AccessorList(open, new(accessors), close);

		prop = MoveInitializerToAttribute(gen, editor.SemanticModel, prop, token);
		prop = prop.WithAccessorList(list);

		if (!prop.Identifier.TrailingTrivia.Any(v => v.IsKind(SyntaxKind.EndOfLineTrivia)))
			prop = prop.WithIdentifier(prop.Identifier.WithTrailingTrivia(CarriageReturnLineFeed));

		editor.ReplaceNode(ogProp, prop);

		return true;
	}

	private static void GenerateGetter(List<AccessorDeclarationSyntax> accessors, SyntaxGenerator gen, SyntaxTriviaList leadingTrivia, AccessorDeclarationSyntax existing, TypeSyntax propertyType, TypeSyntax bindablePropertyField)
	{
		var expression = CastExpression(propertyType.WithoutTrivia(), ((InvocationExpressionSyntax)gen.InvocationExpression(Identifiers.GetValue)).AddArgumentListArguments((ArgumentSyntax)gen.Argument(bindablePropertyField)));
		var accessor = ((AccessorDeclarationSyntax)gen.GetAccessorDeclaration())
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithAttributeLists(existing.AttributeLists)
			.WithModifiers(existing.Modifiers)
			.WithLeadingTrivia(leadingTrivia)
			.WithTrailingLineBreak();

		accessors.Add(accessor);
	}

	private static void GenerateSetter(List<AccessorDeclarationSyntax> accessors, SyntaxGenerator gen, SyntaxTriviaList leadingTrivia, AccessorDeclarationSyntax existing, TypeSyntax bindablePropertyField)
	{
		var expression = ((InvocationExpressionSyntax)gen.InvocationExpression(Identifiers.SetValue))
			.AddArgumentListArguments(
				(ArgumentSyntax)gen.Argument(bindablePropertyField),
				(ArgumentSyntax)gen.Argument(Identifiers.Value));

		var accessor = ((AccessorDeclarationSyntax)gen.SetAccessorDeclaration())
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithAttributeLists(existing.AttributeLists)
			.WithModifiers(existing.Modifiers)
			.WithLeadingTrivia(leadingTrivia)
			.WithTrailingLineBreak();

		accessors.Add(accessor);
	}

	public static PropertyDeclarationSyntax MoveInitializerToAttribute(SyntaxGenerator gen, SemanticModel model, PropertyDeclarationSyntax prop, CancellationToken token)
	{
		if (prop.Initializer == null)
			return prop;

		var bp = BindablePropertyFixHelper.GetAttribute(model, prop, token);
		if (bp == null)
			return prop;

		var (list, attr) = bp.Value;
		var nameEquals = NameEquals(nameof(BindablePropertyAttribute.DefaultValue));
		var argument = AttributeArgument(nameEquals, null, prop.Initializer.Value);
		var newList = (AttributeListSyntax)gen.AddAttributeArguments(attr, [argument]);

		return prop
			.WithAttributeLists(prop.AttributeLists.Replace(list, newList))
			.WithInitializer(null)
			.WithSemicolonToken(default);
	}
}
