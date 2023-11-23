namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public sealed record InstancePropertySyntaxGenerator(INamedTypeSymbol DeclaringType, IPropertySymbol DeclaringProperty) : ISyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameBindableProperty = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	private static readonly IdentifierNameSyntax NameBindablePropertyKey = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");
	private static readonly IdentifierNameSyntax NameCreate = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
	private static readonly IdentifierNameSyntax NameCreateReadOnly = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");

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
	private void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, TypeSyntax createMethod)
	{
		var arguments = new ExpressionSyntax[4];
		arguments[0] = SyntaxHelper.Literal(DeclaringProperty.Name);
		arguments[1] = GetTypeOfExpression(DeclaringProperty.Type);
		arguments[2] = GetTypeOfExpression(DeclaringType);
		arguments[3] = SyntaxHelper.Null;
		//arguments[3] = DefaultValueExpression;
		//arguments[4] = DefaultModeExpression;

		//if (ValidateValueCallback)
		//{
		//	arguments[5] = CreateValidateValueHandler(out var method);
		//	members.Add(method);
		//}
		//else
		//{
		//	arguments[5] = SyntaxHelper.Null;
		//}

		//arguments[6] = CreateChangeHandler($"On{PropertyName}Changing", out var onChanging);
		//arguments[7] = CreateChangeHandler($"On{PropertyName}Changed", out var onChanged);

		//members.Add(onChanging);
		//members.Add(onChanged);

		//if (CoerceValueCallback)
		//{
		//	arguments[8] = CreateCoerceValueHandler(out var method);
		//	members.Add(method);
		//}
		//else
		//{
		//	arguments[8] = SyntaxHelper.Null;
		//}

		//if (DefaultValueFactory)
		//{
		//	arguments[9] = CreateDefaultValueGenerator(out var method);
		//	members.Add(method);
		//}
		//else
		//{
		//	arguments[9] = SyntaxHelper.Null;
		//}

		var propertyInitializer = SyntaxFactory.InvocationExpression(createMethod, SyntaxHelper.ArgumentList(arguments));
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(fieldType).AddVariables(declaration))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	public void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
	{
		var prop = DeclaringProperty;
		var getModifiers = prop.DeclaredAccessibility.ToSyntaxList();
		var bindablePropertyField = SyntaxFactory.IdentifierName(prop.Name + "Property");
		if (prop.SetMethod == null || prop.SetMethod.DeclaredAccessibility != prop.DeclaredAccessibility)
		{
			var setModifiers = prop.SetMethod == null ? SymbolHelper.TokensPrivate : prop.SetMethod.DeclaredAccessibility.ToSyntaxList();
			var bindablePropertyKeyField = SyntaxFactory.IdentifierName(prop.Name + "PropertyKey");
			GenerateBindablePropertyDeclaration(members, setModifiers, bindablePropertyKeyField, NameBindablePropertyKey, NameCreateReadOnly);
			GenerateReadOnlyBindablePropertyDeclaration(members, getModifiers, bindablePropertyField, bindablePropertyKeyField);
		}
		else
		{
			GenerateBindablePropertyDeclaration(members, getModifiers, bindablePropertyField, NameBindableProperty, NameCreate);
		}
	}
}