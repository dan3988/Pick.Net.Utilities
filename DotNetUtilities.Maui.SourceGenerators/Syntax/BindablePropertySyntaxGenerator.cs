namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal readonly record struct BindablePropertySyntaxGeneratorConstructorParameters(string PropertyName, TypeSyntax PropertyType, TypeSyntax DeclaringType, ExpressionSyntax DefaultValueExpression, ExpressionSyntax DefaultModeExpression, bool DefaultValueFactory);

internal abstract class BindablePropertySyntaxGenerator
{
	protected static readonly IdentifierNameSyntax NameValue = IdentifierName("value");
	protected static readonly IdentifierNameSyntax NameGetValue = IdentifierName("GetValue");
	protected static readonly IdentifierNameSyntax NameSetValue = IdentifierName("SetValue");
	protected static readonly IdentifierNameSyntax NameBindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	protected static readonly IdentifierNameSyntax NameBindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	private static readonly IdentifierNameSyntax nameBindablePropertyKeyProperty = IdentifierName("BindableProperty");

	protected static void GenerateReadOnlyBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, IdentifierNameSyntax bindablePropertyKeyField)
	{
		var propertyInitializer = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, nameBindablePropertyKeyProperty);
		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));
		var field = FieldDeclaration(VariableDeclaration(NameBindableProperty).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	protected readonly string PropertyName;
	protected readonly TypeSyntax PropertyType;
	protected readonly TypeSyntax DeclaringType;
	protected readonly ExpressionSyntax DefaultValueExpression;
	protected readonly ExpressionSyntax DefaultModeExpression;
	protected readonly bool DefaultValueFactory;

	protected BindablePropertySyntaxGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values)
	{
		(PropertyName, PropertyType, DeclaringType, DefaultValueExpression, DefaultModeExpression, DefaultValueFactory) = values;
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
			Argument(SyntaxHelper.Literal(PropertyName)),
			Argument(SyntaxHelper.TypeOf(PropertyType)),
			Argument(SyntaxHelper.TypeOf(DeclaringType)),
			Argument(DefaultValueExpression),
			Argument(DefaultModeExpression),
			Argument(SyntaxHelper.Null),
			Argument(CreateChangeHandler($"On{PropertyName}Changing", out var onChanging)),
			Argument(CreateChangeHandler($"On{PropertyName}Changed", out var onChanged))));

		members.Add(onChanging);
		members.Add(onChanged);

		if (DefaultValueFactory)
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
