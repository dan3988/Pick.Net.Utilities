namespace Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

using static SyntaxFactory;

internal readonly record struct BindablePropertySyntaxGeneratorConstructorParameters(
	string PropertyName,
	TypeSyntax PropertyType,
	TypeSyntax DeclaringType,
	ExpressionSyntax DefaultValueExpression,
	ExpressionSyntax DefaultModeExpression,
	bool DefaultValueFactory,
	bool CoerceValueCallback,
	bool ValidateValueCallback);

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
	protected readonly bool CoerceValueCallback;
	protected readonly bool ValidateValueCallback;

	protected BindablePropertySyntaxGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values)
	{
		(PropertyName, PropertyType, DeclaringType, DefaultValueExpression, DefaultModeExpression, DefaultValueFactory, CoerceValueCallback, ValidateValueCallback) = values;
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

	protected abstract LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method);

	protected abstract LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method);

	protected void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, TypeSyntax createMethod)
	{
		var arguments = new ExpressionSyntax[10];
		arguments[0] = SyntaxHelper.Literal(PropertyName);
		arguments[1] = SyntaxHelper.TypeOf(PropertyType);
		arguments[2] = SyntaxHelper.TypeOf(DeclaringType);
		arguments[3] = DefaultValueExpression;
		arguments[4] = DefaultModeExpression;

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

		var propertyInitializer = InvocationExpression(createMethod, SyntaxHelper.ArgumentList(arguments));
		var declarator = VariableDeclarator(fieldName.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));
		var field = FieldDeclaration(VariableDeclaration(fieldType).AddVariables(declarator))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}
}
