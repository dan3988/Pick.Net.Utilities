namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal abstract class BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameBindableProperty = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	private static readonly IdentifierNameSyntax NameBindablePropertyKey = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	private static readonly IdentifierNameSyntax NameBindablePropertyKeyProperty = SyntaxFactory.IdentifierName("BindableProperty");

	private static ExpressionSyntax GetTypeOfExpression(ITypeSymbol typeSymbol)
	{
		var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		var identifier = SyntaxFactory.IdentifierName(name);
		return SyntaxFactory.TypeOfExpression(identifier);
	}

	private static void GenerateReadOnlyBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField)
	{
		var propertyInitializer = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, NameBindablePropertyKeyProperty);
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(NameBindableProperty).AddVariables(declaration))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	public TypeSyntax DeclaringType { get; }

	public string PropertyName { get; }

	public TypeSyntax PropertyType2 { get; }

	public TypeSyntax AnnotatedPropertyType { get; }

	public Accessibility Accessibility { get; }

	public Accessibility WriteAccessibility { get; }

	public ExpressionSyntax DefaultValueSyntax { get; }

	public ExpressionSyntax DefaultModeSyntax { get; }

	public bool DefaultValueFactory { get; }

	public bool CoerceValueCallback { get; }

	public bool ValidateValueCallback { get; }

	protected abstract string CreateMethod { get; }

	protected abstract string CreateReadOnlyMethod { get; }

	private protected BindablePropertySyntaxGenerator(in SyntaxGeneratorSharedProperties properties)
	{
		(PropertyName, DeclaringType, PropertyType2, AnnotatedPropertyType, Accessibility, WriteAccessibility, DefaultValueSyntax, DefaultModeSyntax, DefaultValueFactory, CoerceValueCallback, ValidateValueCallback) = properties;
	}

	private void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, string createMethod)
	{
		var arguments = new ExpressionSyntax[10];
		arguments[0] = SyntaxHelper.Literal(PropertyName);
		arguments[1] = SyntaxFactory.TypeOfExpression(PropertyType2);
		arguments[2] = SyntaxFactory.TypeOfExpression(DeclaringType);
		arguments[3] = DefaultValueSyntax;
		arguments[4] = DefaultModeSyntax;

		if (ValidateValueCallback)
		{
			arguments[5] = CreateValidateValueHandler(out var method);
			members.Add(method);
		}
		else
		{
			arguments[5] = SyntaxHelper.Null;
		}

		arguments[6] = CreateChangeHandler($"On{PropertyName}Changing", out var onChanging);
		arguments[7] = CreateChangeHandler($"On{PropertyName}Changed", out var onChanged);

		members.Add(onChanging);
		members.Add(onChanged);

		if (CoerceValueCallback)
		{
			arguments[8] = CreateCoerceValueHandler(out var method);
			members.Add(method);
		}
		else
		{
			arguments[8] = SyntaxHelper.Null;
		}

		if (DefaultValueFactory)
		{
			arguments[9] = CreateDefaultValueGenerator(out var method);
			members.Add(method);
		}
		else
		{
			arguments[9] = SyntaxHelper.Null;
		}

		var create = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindableProperty, SyntaxFactory.IdentifierName(createMethod));
		var propertyInitializer = SyntaxFactory.InvocationExpression(create, SyntaxHelper.ArgumentList(arguments));
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(fieldType).AddVariables(declaration))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	protected abstract LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method);

	protected abstract void GenerateExtraMembers(ICollection<MemberDeclarationSyntax> members, TypeSyntax propertyField, TypeSyntax propertyKeyField);

	public void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
	{
		var readModifiers = Accessibility.ToSyntaxList();
		var bindablePropertyField = SyntaxFactory.IdentifierName(PropertyName + "Property");
		if (Accessibility != WriteAccessibility)
		{
			var writeModifiers = WriteAccessibility.ToSyntaxList();
			var bindablePropertyKeyField = SyntaxFactory.IdentifierName(PropertyName + "PropertyKey");
			GenerateBindablePropertyDeclaration(members, writeModifiers, bindablePropertyKeyField, NameBindablePropertyKey, CreateReadOnlyMethod);
			GenerateReadOnlyBindablePropertyDeclaration(members, readModifiers, bindablePropertyField, bindablePropertyKeyField);
			GenerateExtraMembers(members, bindablePropertyField, bindablePropertyKeyField);
		}
		else
		{
			GenerateBindablePropertyDeclaration(members, readModifiers, bindablePropertyField, NameBindableProperty, CreateMethod);
			GenerateExtraMembers(members, bindablePropertyField, bindablePropertyField);
		}
	}
}