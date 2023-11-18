namespace DotNetUtilities.Maui.SourceGenerators.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal abstract class BindableAttachedPropertySyntaxGenerator : BindablePropertySyntaxGenerator
{
	private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttached");
	private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateAttachedReadOnly");

	public static BindableAttachedPropertySyntaxGenerator Create(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, TypeSyntax attachedType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
	{
		return setModifiers.Count == 0
			? new WritableGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, attachedType, getModifiers)
			: new ReadOnlyGenerator(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, attachedType, getModifiers, setModifiers);
	}

	private readonly TypeSyntax attachedType;

	private BindableAttachedPropertySyntaxGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType)
		: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression)
	{
		this.attachedType = attachedType;
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertyGetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var expression = CastExpression(propertyType,
				InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), nameGetValue))
					.AddArgumentListArguments(Argument(bindablePropertyField)));

		return MethodDeclaration(propertyType, "Get" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj })))
			.WithReturnType(propertyType)
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	private MethodDeclarationSyntax GenerateAttachedBindablePropertySetMethod(SyntaxTokenList modifiers, TypeSyntax bindablePropertyField)
	{
		var paramObj = Parameter(Identifier("obj")).WithType(attachedType);
		var paramValue = Parameter(nameValue.Identifier).WithType(propertyType);
		var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(paramObj.Identifier), nameSetValue))
			.AddArgumentListArguments(
				Argument(bindablePropertyField),
				Argument(nameValue));

		return MethodDeclaration(SyntaxHelper.TypeVoid, "Set" + propertyName)
			.WithParameterList(ParameterList(SeparatedList(new[] { paramObj, paramValue })))
			.WithModifiers(modifiers)
			.AddModifier(SyntaxKind.StaticKeyword)
			.WithExpressionBody(ArrowExpressionClause(expression))
			.WithSemicolonToken();
	}

	protected override LambdaExpressionSyntax CreateChangeHandler(string name, out MethodDeclarationSyntax method)
	{
		var paramBindable = Parameter(Identifier("bindable"));
		var paramOldValue = Parameter(Identifier("oldValue"));
		var paramNewValue = Parameter(Identifier("newValue"));

		method = MethodDeclaration(SyntaxHelper.TypeVoid, name)
			.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
			.AddParameterListParameters(
				paramBindable.WithType(attachedType),
				paramOldValue.WithType(propertyType),
				paramNewValue.WithType(propertyType))
			.WithSemicolonToken();

		var parameters = ParameterList(SeparatedList(new ParameterSyntax[] { paramBindable, paramOldValue, paramNewValue }));
		var body = InvocationExpression(IdentifierName(method.Identifier))
			.AddArgumentListArguments(
				Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramOldValue.Identifier))),
				Argument(CastExpression(propertyType, IdentifierName(paramNewValue.Identifier))));

		return ParenthesizedLambdaExpression(parameters, null, body);
	}

	private sealed class WritableGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList modifiers;

		internal WritableGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType, SyntaxTokenList modifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, attachedType)
		{
			this.modifiers = modifiers;
		}

		protected override MemberDeclarationSyntax[] GenerateMembers()
		{
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			return new MemberDeclarationSyntax[]
			{
				GenerateBindablePropertyDeclaration(modifiers, bindablePropertyField, nameBindableProperty, nameCreate, out var onChanging, out var onChanged),
				GenerateAttachedBindablePropertyGetMethod(modifiers, bindablePropertyField),
				GenerateAttachedBindablePropertySetMethod(modifiers, bindablePropertyField),
				onChanging,
				onChanged
			};
		}
	}

	private sealed class ReadOnlyGenerator : BindableAttachedPropertySyntaxGenerator
	{
		private readonly SyntaxTokenList getModifiers;
		private readonly SyntaxTokenList setModifiers;

		internal ReadOnlyGenerator(string propertyName, TypeSyntax propertyType, TypeSyntax declaringType, ExpressionSyntax defaultValueExpression, ExpressionSyntax defaultModeExpression, TypeSyntax attachedType, SyntaxTokenList getModifiers, SyntaxTokenList setModifiers)
			: base(propertyName, propertyType, declaringType, defaultValueExpression, defaultModeExpression, attachedType)
		{
			this.getModifiers = getModifiers;
			this.setModifiers = setModifiers;
		}

		protected override MemberDeclarationSyntax[] GenerateMembers()
		{
			var bindablePropertyKeyField = IdentifierName(propertyName + "PropertyKey");
			var bindablePropertyField = IdentifierName(propertyName + "Property");
			return new MemberDeclarationSyntax[]
			{
				GenerateBindablePropertyDeclaration(setModifiers, bindablePropertyKeyField, nameBindablePropertyKey, nameCreateReadOnly, out var onChanging, out var onChanged),
				GenerateReadOnlyBindablePropertyDeclaration(getModifiers, bindablePropertyField, bindablePropertyKeyField),
				GenerateAttachedBindablePropertyGetMethod(getModifiers, bindablePropertyField),
				GenerateAttachedBindablePropertySetMethod(setModifiers, bindablePropertyKeyField),
				onChanging,
				onChanged
			};
		}
	}
}