namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

internal abstract class DefaultValueGenerator
{
	public static DefaultValueGenerator None = new NoDefaultValue();

	public static DefaultValueGenerator StaticValue(SyntaxToken identifier, bool requiresConvert)
		=> new StaticDefaultValue(identifier, requiresConvert);

	public static DefaultValueGenerator StaticGenerator(SyntaxToken identifier, TypeSyntax? attachedType = null)
		=> new StaticGeneratorMethod(identifier, attachedType);

	public static DefaultValueGenerator InstanceGenerator(SyntaxToken identifier)
		=> new InstanceGeneratorMethod(identifier);

	private DefaultValueGenerator()
	{
	}

	public abstract void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator);

	private sealed class NoDefaultValue : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			defaultValue = SyntaxHelper.Null;
			defaultValueGenerator = SyntaxHelper.Null;
		}
	}

	private sealed class StaticDefaultValue(SyntaxToken identifier, bool requiresConvert) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			defaultValueGenerator = SyntaxHelper.Null;
			defaultValue = IdentifierName(identifier);

			if (requiresConvert)
				defaultValue = CastExpression(propertyType, defaultValue);
		}
	}

	private sealed class StaticGeneratorMethod(SyntaxToken identifier, TypeSyntax? attachedType) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			var paramBindable = Parameter(Identifier("bindable"));
			var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
			var body = InvocationExpression(IdentifierName(identifier));

			if (attachedType != null)
			{
				body = body.AddArgumentListArguments(
						Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))));
			}

			defaultValue = SyntaxHelper.Null;
			defaultValueGenerator = ParenthesizedLambdaExpression(parameters, null, body);
		}
	}

	private sealed class InstanceGeneratorMethod(SyntaxToken identifier) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			var paramBindable = Parameter(Identifier("bindable"));
			var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
			var body = InvocationExpression(
					MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(identifier)));

			defaultValue = SyntaxHelper.Null;
			defaultValueGenerator = ParenthesizedLambdaExpression(parameters, null, body);
		}
	}
}
