namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

public abstract class DefaultValueGenerator
{
	private static readonly IdentifierNameSyntax BooleanBoxIdentifier = IdentifierName("global::Pick.Net.Utilities.BooleanBox");

	public static DefaultValueGenerator None = new NoDefaultValue();

	public static DefaultValueGenerator BoxedBoolean(bool value)
		=> new BoxedBooleanConstantValue(value);

	public static DefaultValueGenerator BoxedBoolean(ExpressionSyntax expression)
		=> new BoxedBooleanValue(expression);

	public static DefaultValueGenerator StaticValue(SyntaxToken identifier, bool requiresConvert)
		=> new StaticDefaultValue(identifier, requiresConvert);

	public static DefaultValueGenerator StaticGenerator(SyntaxToken identifier, bool requiresConvert, TypeSyntax? attachedType = null)
		=> new StaticGeneratorMethod(identifier, requiresConvert, attachedType);

	public static DefaultValueGenerator InstanceGenerator(SyntaxToken identifier, bool requiresConvert)
		=> new InstanceGeneratorMethod(identifier, requiresConvert);

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

	private sealed class BoxedBooleanConstantValue(bool value) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			defaultValueGenerator = SyntaxHelper.Null;
			defaultValue = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, BooleanBoxIdentifier, IdentifierName($"{value}"));
		}
	}

	private sealed class BoxedBooleanValue(ExpressionSyntax expression) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			var accessor = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, BooleanBoxIdentifier, IdentifierName("Box"));
			var invocation = InvocationExpression(accessor).AddArgumentListArguments(Argument(expression));

			defaultValueGenerator = SyntaxHelper.Null;
			defaultValue = invocation;
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

	private sealed class StaticGeneratorMethod(SyntaxToken identifier, bool requiresConvert, TypeSyntax? attachedType) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			var paramBindable = Parameter(Identifier("bindable"));
			var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
			var invocation = InvocationExpression(IdentifierName(identifier));

			if (attachedType != null)
			{
				invocation = invocation.AddArgumentListArguments(
						Argument(CastExpression(attachedType, IdentifierName(paramBindable.Identifier))));
			}

			var body = requiresConvert ? (ExpressionSyntax)CastExpression(propertyType, invocation) : invocation;

			defaultValue = SyntaxHelper.Null;
			defaultValueGenerator = ParenthesizedLambdaExpression(parameters, null, body);
		}
	}

	private sealed class InstanceGeneratorMethod(SyntaxToken identifier, bool requiresConvert) : DefaultValueGenerator
	{
		public override void Generate(TypeSyntax declaringType, TypeSyntax propertyType, out ExpressionSyntax defaultValue, out ExpressionSyntax defaultValueGenerator)
		{
			var paramBindable = Parameter(Identifier("bindable"));
			var parameters = ParameterList(SeparatedList(new[] { paramBindable }));
			var invocation = InvocationExpression(
					MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, CastExpression(declaringType, IdentifierName(paramBindable.Identifier)).WithSurroundingParenthesis(), IdentifierName(identifier)));

			var body = requiresConvert ? (ExpressionSyntax)CastExpression(propertyType, invocation) : invocation;

			defaultValue = SyntaxHelper.Null;
			defaultValueGenerator = ParenthesizedLambdaExpression(parameters, null, body);
		}
	}
}
