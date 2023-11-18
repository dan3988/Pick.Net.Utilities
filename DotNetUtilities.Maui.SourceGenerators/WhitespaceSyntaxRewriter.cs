namespace DotNetUtilities.Maui.SourceGenerators;

internal class WhitespaceSyntaxReWriter : CSharpSyntaxRewriter
{
	private static int GetDeclarationDepth(SyntaxToken token)
		=> GetDeclarationDepth(token.Parent);

	private static int GetDeclarationDepth(SyntaxTrivia trivia)
		=> SyntaxFacts.IsPreprocessorDirective(trivia.Kind()) ? 0 : GetDeclarationDepth(trivia.Token);

	private static int GetDeclarationDepth(SyntaxNode? node)
	{
		if (node is null)
		{
			return 0;
		}

		if (node.IsStructuredTrivia)
		{
			var tr = ((StructuredTriviaSyntax)node).ParentTrivia;
			return GetDeclarationDepth(tr);
		}
		else if (node.Parent != null)
		{
			if (node.Parent.IsKind(SyntaxKind.CompilationUnit))
			{
				return 0;
			}

			int parentDepth = GetDeclarationDepth(node.Parent);

			if (node.Parent.Kind() is SyntaxKind.GlobalStatement or SyntaxKind.FileScopedNamespaceDeclaration)
			{
				return parentDepth;
			}

			if (node.IsKind(SyntaxKind.IfStatement) && node.Parent.IsKind(SyntaxKind.ElseClause))
			{
				return parentDepth;
			}

			if (node.Parent is BlockSyntax)
			{
				return parentDepth + 1;
			}

			if (node is { Parent: InitializerExpressionSyntax or AnonymousObjectMemberDeclaratorSyntax } ||
				node is AssignmentExpressionSyntax { Parent: InitializerExpressionSyntax })
			{
				if (!IsSingleLineInitializerContext(node.Parent))
				{
					return parentDepth + 1;
				}
			}

			if (node is StatementSyntax and not BlockSyntax)
			{
				// Nested statements are normally indented one level.
				//
				// However, for chains of using-statements or fixed-statements, we'd like to follow the
				// idiomatic pattern of:
				//
				//      using ...
				//      using ...
				//          .. embedded statement ..
				return node switch
				{
					UsingStatementSyntax { Parent: UsingStatementSyntax } => parentDepth,
					FixedStatementSyntax { Parent: FixedStatementSyntax } => parentDepth,
					_ => parentDepth + 1
				};
			}

			if (node is MemberDeclarationSyntax or AccessorDeclarationSyntax or TypeParameterConstraintClauseSyntax or SwitchSectionSyntax or SwitchExpressionArmSyntax or UsingDirectiveSyntax or ExternAliasDirectiveSyntax or QueryExpressionSyntax or QueryContinuationSyntax)
			{
				return parentDepth + 1;
			}

			return parentDepth;
		}

		return 0;
	}

	private static bool IsSingleLineInitializerContext(SyntaxNode? node)
	{
		if (node is null)
			return false;

		for (var currentParent = node.Parent;  currentParent is not null; currentParent = currentParent.Parent)
		{
			if (currentParent is InterpolationSyntax or AttributeArgumentSyntax or ArgumentSyntax)
				return true;

			if (currentParent is StatementSyntax or MemberDeclarationSyntax)
				return false;
		}

		return false;
	}

	public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
	{
		var declaration = node.Declaration;
		var variables = declaration.Variables.Select(FixFieldDeclarationVariables);
		declaration = declaration.WithVariables(variables);
		node = node.WithDeclaration(declaration).AddTrailingLineBreak();

		return base.VisitFieldDeclaration(node);
	}

	private static VariableDeclaratorSyntax FixFieldDeclarationVariables(VariableDeclaratorSyntax syntax)
	{
		if (syntax.Initializer is { Value: InvocationExpressionSyntax invocation } equals)
		{
			var depth = GetDeclarationDepth(invocation) + 1;
			var arguments = invocation.ArgumentList.Arguments.Select(v => v.AddLeadingLineBreak(depth));
			var argumentsList = invocation.ArgumentList.WithArguments(arguments);
			invocation = invocation.WithArgumentList(argumentsList);
			equals = equals.WithValue(invocation);
			syntax = syntax.WithInitializer(equals);
		}

		return syntax;
	}

	public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
	{
		if (node.ExpressionBody != null)
		{
			var depth = GetDeclarationDepth(node);
			var body = node.ExpressionBody.AddLeadingLineBreak(depth + 1);
			node = node.WithExpressionBody(body);
		}

		node = node.AddTrailingLineBreak();

		return base.VisitMethodDeclaration(node);
	}

	public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
	{
		var depth = GetDeclarationDepth(node);
		var accessorList = node.AccessorList;
		if (accessorList != null)
		{
			var accessors = accessorList.Accessors.ToArray();

			for (var i = 0; ;)
			{
				ref var accessor = ref accessors[i];

				if (accessor.ExpressionBody != null)
					accessor = accessor.WithLeadingLineBreak(depth + 1).WithoutTrailingTrivia();

				if (accessors.Length == ++i)
				{
					accessor = accessor.WithTrailingLineBreak(depth);
					break;
				}
			}

			accessorList = accessorList.WithAccessors(new(accessors)).AddLeadingLineBreak(depth);
			node = node.WithAccessorList(accessorList);
		}

		return base.VisitPropertyDeclaration(node);
	}
}
