namespace Pick.Net.Utilities.Maui.SourceGenerators.Syntax;

using System.Collections.Generic;

using static SyntaxFactory;

internal abstract class BindableInstancePropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax NameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
	private static readonly IdentifierNameSyntax NameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");

	public static BindableInstancePropertySyntaxGenerator Create(in BindablePropertySyntaxGeneratorConstructorParameters values, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(in values, getModifiers)
			: new ReadOnlyGenerator(in values, getModifiers, setModifiers);
	}

	private static AccessorDeclarationSyntax GenerateGetter(TypeSyntax propertyType, TypeSyntax bindablePropertyField)
	{
		var expression = CastExpression(propertyType, InvocationExpression(NameGetValue).AddArgumentListArguments(Argument(bindablePropertyField)));

		return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private static AccessorDeclarationSyntax GenerateSetter(TypeSyntax bindablePropertyField, SyntaxTokenList accessors)
	{
		var expression = InvocationExpression(NameSetValue)
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(NameValue));

		return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
			.WithModifiers(accessors)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private BindableInstancePropertySyntaxGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values)
		: base(in values)
	{
	}

	private void GenerateBindablePropertyAccessors(ICollection<MemberDeclarationSyntax> members, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers, TypeSyntax readablePropertyField, TypeSyntax writablePropertyField)
	{
		var prop = PropertyDeclaration(PropertyType, PropertyName)
			.WithModifiers(getModifiers)
			.AddAccessorListAccessors(
				GenerateGetter(PropertyType, readablePropertyField),
				GenerateSetter(writablePropertyField, setModifiers));

		members.Add(prop);
	}

	protected override LambdaExpressionSyntax CreateDefaultValueGenerator(out MethodDeclarationSyntax method)
	{
		method = MethodDeclaration(PropertyType, $"Generate{PropertyName}DefaultValue")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.WithSemicolonToken();

		var paramBindable = Parameter(Identifier("bindable"));
		var parameters = ParameterList(SeparatedList(new[] { paramBindable}));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateValidateValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(SyntaxHelper.TypeBoolean, $"Validate{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(PropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateCoerceValueHandler(out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramValue = Parameter(Identifier("value"));

		method = MethodDeclaration(PropertyType, $"Coerce{PropertyName}Value")
			.AddModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(paramValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramValue }));
		var methodReference = MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(),
			IdentifierName(method.Identifier));

		var methodArguments = SyntaxHelper.ArgumentList(CastExpression(PropertyType, IdentifierName(paramValue.Identifier)));
		var body = InvocationExpression(methodReference, methodArguments);

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));

		method = MethodDeclaration(SyntaxHelper.TypeVoid, name)
			.AddModifier(SyntaxKind.PartialKeyword)
			.AddParameterListParameters(
				paramOldValue.WithType(PropertyType),
				paramNewValue.WithType(PropertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(DeclaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(method.Identifier)))
			.AddArgumentListArguments(
				Argument(CastExpression(PropertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(PropertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	private sealed class WritableGenerator : BindableInstancePropertySyntaxGenerator
	{
		private readonly SyntaxTokenList _modifiers;

		internal WritableGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values, SyntaxTokenList modifiers)
			: base(in values)
		{
			_modifiers = modifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyField = IdentifierName(PropertyName + "Property");
			GenerateBindablePropertyDeclaration(members, _modifiers, bindablePropertyField, NameBindableProperty, NameCreate);
			GenerateBindablePropertyAccessors(members, _modifiers, default, bindablePropertyField, bindablePropertyField);
		}
	}

	private sealed class ReadOnlyGenerator : BindableInstancePropertySyntaxGenerator
	{
		private readonly SyntaxTokenList _getModifiers;
		private readonly SyntaxTokenList _setModifiers;

		internal ReadOnlyGenerator(in BindablePropertySyntaxGeneratorConstructorParameters values, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(in values)
		{
			_getModifiers = getModifiers;
			_setModifiers = setModifiers;
		}

		protected override void GenerateMembers(ICollection<MemberDeclarationSyntax> members)
		{
			var bindablePropertyKeyField = IdentifierName(PropertyName + "PropertyKey");
			var bindablePropertyField = IdentifierName(PropertyName + "Property");
			GenerateBindablePropertyDeclaration(members, _setModifiers, bindablePropertyKeyField, NameBindablePropertyKey, NameCreateReadOnly);
			GenerateReadOnlyBindablePropertyDeclaration(members, _getModifiers, bindablePropertyField, bindablePropertyKeyField);
			GenerateBindablePropertyAccessors(members, _getModifiers, _setModifiers, bindablePropertyField, bindablePropertyKeyField);
		}
	}
}
