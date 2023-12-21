namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public abstract class BindablePropertySyntaxGenerator
{
	private static void GenerateReadOnlyBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField, SyntaxTrivia comment)
	{
		var propertyInitializer = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, Identifiers.BindablePropertyKeyProperty);
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(Identifiers.BindableProperty).AddVariables(declaration))
			.WithAttributeLists(new(SyntaxHelper.GeneratedCodeAttributeList))
			.WithModifiers(modifiers.AddRange(ModifierLists.StaticReadOnly))
			.WithLeadingTrivia(comment);

		members.Add(field);
	}

	public TypeSyntax DeclaringType { get; }

	public string PropertyName { get; }

	public TypeSyntax PropertyType { get; }

	public TypeSyntax AnnotatedPropertyType { get; }

	public Accessibility Accessibility { get; }

	public Accessibility WriteAccessibility { get; }

	public DefaultValueGenerator DefaultValue { get; }

	public ExpressionSyntax DefaultModeSyntax { get; }

	public MethodSignature PropertyChangingSignature { get; }

	public MethodSignature PropertyChangedSignature { get; }

	public bool CoerceValueCallback { get; }

	public bool ValidateValueCallback { get; }

	protected abstract string CreateMethod { get; }

	protected abstract string CreateReadOnlyMethod { get; }

	protected abstract string CommentFormat { get; }

	private protected BindablePropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties)
	{
		(PropertyName, DeclaringType, PropertyType, AnnotatedPropertyType, Accessibility, WriteAccessibility, DefaultValue, DefaultModeSyntax, PropertyChangingSignature, PropertyChangedSignature, CoerceValueCallback, ValidateValueCallback) = properties;
	}

	private SyntaxTrivia CreateComment(string propertyDisplayType)
		=> SyntaxFactory.Comment($"/// <summary>" + string.Format(CommentFormat, propertyDisplayType, PropertyName) + "</summary>");

	private void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, string createMethod, SyntaxTrivia comment)
	{
		var arguments = new ExpressionSyntax[10];
		arguments[0] = SyntaxHelper.Literal(PropertyName);
		arguments[1] = SyntaxFactory.TypeOfExpression(PropertyType);
		arguments[2] = SyntaxFactory.TypeOfExpression(DeclaringType);
		arguments[4] = DefaultModeSyntax;
		arguments[6] = CreateChangeHandler(PropertyChangingSignature, out var onChanging);
		arguments[7] = CreateChangeHandler(PropertyChangedSignature, out var onChanged);

		members.Add(onChanging);
		members.Add(onChanged);

		DefaultValue.Generate(DeclaringType, PropertyType, out arguments[3], out arguments[9]);

		if (ValidateValueCallback)
		{
			arguments[5] = CreateValidateValueHandler(out var method);
			members.Add(method);
		}
		else
		{
			arguments[5] = SyntaxHelper.Null;
		}

		if (CoerceValueCallback)
		{
			arguments[8] = CreateCoerceValueHandler(out var method);
			members.Add(method);
		}
		else
		{
			arguments[8] = SyntaxHelper.Null;
		}

		var create = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Identifiers.BindableProperty, SyntaxFactory.IdentifierName(createMethod));
		var propertyInitializer = SyntaxFactory.InvocationExpression(create, SyntaxHelper.ArgumentList(arguments));
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(fieldType).AddVariables(declaration))
			.WithAttributeLists(new(SyntaxHelper.GeneratedCodeAttributeList))
			.WithModifiers(modifiers.AddRange(ModifierLists.StaticReadOnly))
			.WithLeadingTrivia(comment);

		members.Add(field);
	}

	protected abstract LambdaExpressionSyntax CreateChangeHandler(MethodSignature signature, out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method);

	protected abstract void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField);

	public void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
	{
		var readModifiers = Accessibility.ToSyntaxList();
		var bindablePropertyField = SyntaxFactory.IdentifierName(PropertyName + "Property");
		if (Accessibility != WriteAccessibility)
		{
			var writeModifiers = WriteAccessibility.ToSyntaxList();
			var bindablePropertyKeyField = SyntaxFactory.IdentifierName(PropertyName + "PropertyKey");
			GenerateBindablePropertyDeclaration(members, writeModifiers, bindablePropertyKeyField, Identifiers.BindablePropertyKey, CreateReadOnlyMethod, CreateComment("property key"));
			GenerateReadOnlyBindablePropertyDeclaration(members, readModifiers, bindablePropertyField, bindablePropertyKeyField, CreateComment("property"));
			GenerateExtraMembers(members, bindablePropertyField, bindablePropertyKeyField);
		}
		else
		{
			GenerateBindablePropertyDeclaration(members, readModifiers, bindablePropertyField, Identifiers.BindableProperty, CreateMethod, CreateComment("property"));
			GenerateExtraMembers(members, bindablePropertyField, bindablePropertyField);
		}
	}
}