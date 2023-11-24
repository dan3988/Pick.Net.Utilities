using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal abstract class BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameBindableProperty = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
	private static readonly IdentifierNameSyntax NameBindablePropertyKey = SyntaxFactory.IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");

	private static readonly IdentifierNameSyntax NameBindablePropertyKeyProperty = SyntaxFactory.IdentifierName("BindableProperty");

	public SyntaxReference? Owner { get; }

	public INamedTypeSymbol DeclaringType { get; }

	public string PropertyName { get; }

	public ITypeSymbol PropertyType { get; }

	public Accessibility Accessibility { get; }

	public Accessibility WriteAccessibility { get; }

	public BindingMode DefaultMode { get; }

	public object DefaultValue { get; }

	public bool DefaultValueFactory { get; }

	public bool CoerceValueCallback { get; }

	public bool ValidateValueCallback { get; }

	private protected BindablePropertySyntaxGenerator(SyntaxGeneratorSharedProperties properties)
	{
		(Owner, DeclaringType, PropertyName, PropertyType, Accessibility, WriteAccessibility, DefaultMode, DefaultValue, DefaultValueFactory, CoerceValueCallback, ValidateValueCallback) = properties;
	}
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

	private void GenerateBindablePropertyDeclaration(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList modifiers, IdentifierNameSyntax fieldName, TypeSyntax fieldType, string createMethod)
	{
		var arguments = new ExpressionSyntax[4];
		arguments[0] = SyntaxHelper.Literal(PropertyName);
		arguments[1] = GetTypeOfExpression(PropertyType);
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

		var create = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindableProperty, SyntaxFactory.IdentifierName(createMethod));
		var propertyInitializer = SyntaxFactory.InvocationExpression(create, SyntaxHelper.ArgumentList(arguments));
		var declaration = SyntaxFactory.VariableDeclarator(fieldName.Identifier).WithInitializer(SyntaxFactory.EqualsValueClause(propertyInitializer));
		var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(fieldType).AddVariables(declaration))
			.WithModifiers(modifiers)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

		members.Add(field);
	}

	protected abstract string CreateMethod { get; }

	protected abstract string CreateReadOnlyMethod { get; }

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
		}
		else
		{
			GenerateBindablePropertyDeclaration(members, readModifiers, bindablePropertyField, NameBindableProperty, CreateMethod);
		}
	}
}