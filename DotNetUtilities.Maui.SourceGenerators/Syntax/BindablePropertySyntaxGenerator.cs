namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal readonly record struct BindablePropertySyntaxGeneratorConstructorParameters(string PropertyName, TypeSyntax PropertyType, TypeSyntax DeclaringType, ExpressionSyntax DefaultValueExpression, ExpressionSyntax DefaultModeExpression, bool DefaultValueFactory);
internal abstract class BindablePropertySyntaxGenerator
{
	protected static readonly IdentifierNameSyntax nameValue = IdentifierName("value");
	protected static readonly IdentifierNameSyntax nameGetValue = IdentifierName("GetValue");
	protected static readonly IdentifierNameSyntax nameSetValue = IdentifierName("SetValue");
	protected static readonly IdentifierNameSyntax nameBindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	protected static readonly IdentifierNameSyntax nameBindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	private static readonly IdentifierNameSyntax nameBindablePropertyKeyProperty = IdentifierName("BindableProperty");

	protected static void GenerateReadOnlyBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField)
	{
		var propertyInitializer = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, nameBindablePropertyKeyProperty);
		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));
		var field = FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	protected readonly string propertyName;
	protected readonly TypeSyntax propertyType;
	protected readonly TypeSyntax declaringType;
	protected readonly ExpressionSyntax defaultValueExpression;
	protected readonly ExpressionSyntax defaultModeExpression;
	protected readonly bool defaultValueFactory;

	protected BindablePropertySyntaxGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values)
	{
		(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, defaultValueFactory) = values;
	}

	public TypeDeclarationSyntax Generate(TypeDeclarationSyntax syntax)
	{
		var members = syntax.Members;
		var builder = new List<MemberDeclarationSyntax>(members);
		GenerateMembers(builder);
		return syntax.WithMembers(new(builder));
	}

	protected abstract void GenerateMembers(ICollection<MemberDeclarationSyntax> members);

	protected abstract LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method);

	protected void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, TypeSyntax createMethod)
	{
		var propertyInitializer = InvocationExpression(createMethod, SyntaxHelper.ArgumentList(
			Argument(SyntaxHelper.Literal(propertyName)),
			Argument(SyntaxHelper.TypeOf(propertyType)),
			Argument(SyntaxHelper.TypeOf(declaringType)),
			Argument(defaultValueExpression),
			Argument(defaultModeExpression),
			Argument(SyntaxHelper.Null),
			Argument(CreateChangeHandler($"On{propertyName}Changing", out var onChanging)),
			Argument(CreateChangeHandler($"On{propertyName}Changed", out var onChanged))));

		members.Add(onChanging);
		members.Add(onChanged);

		if (defaultValueFactory)
		{
			propertyInitializer = propertyInitializer.AddArgumentListArguments(
				Argument(SyntaxHelper.Null),
				Argument(CreateDefaultValueGenerator(out var method)));

			members.Add(method);
		}

		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));
		var field = FieldDeclaration(VariableDeclaration(fieldType).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}
}
