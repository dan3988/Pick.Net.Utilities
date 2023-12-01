using System.Text;

using Pick.Net.Utilities.Maui.Helpers;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly Type AttributeType = typeof(BindablePropertyAttribute);

	private static readonly IdentifierNameSyntax NameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax NameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

	private static readonly ExpressionSyntax DefaultDefaultModeSyntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, NameBindingModeOneWay);

	private static ExpressionSyntax ParseDefaultBindingMode(AttributeData attribute, object? value, DiagnosticsBuilder diagnostics)
	{
		var enumValue = value == null ? 0 : Enum.ToObject(typeof(BindingMode), value);
		if (!Enum.IsDefined(typeof(BindingMode), enumValue))
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, attribute.ApplicationSyntaxReference, enumValue);
			return CastExpression(NameBindingMode, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)enumValue)));
		}
		else
		{
			return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, IdentifierName(enumValue.ToString()));
		}
	}

	private static ExpressionSyntax ParseDefaultValue(AttributeData attribute, DiagnosticsBuilder diagnostics, ITypeSymbol? propertyType, object? value)
	{
		static bool TryParseCore(AttributeData attribute, DiagnosticsBuilder diagnostics, ITypeSymbol propertyType, ITypeSymbol realPropertyType, object? value, out ExpressionSyntax expression)
		{
			if (propertyType.SpecialType.TryGetTypeCode(out var target))
			{
				expression = SyntaxHelper.Literal(value, target);
				return true;
			}

			diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueNotSupported, attribute.ApplicationSyntaxReference, realPropertyType);
			expression = SyntaxHelper.Null;
			return false;
		}

		if (propertyType == null)
			return SyntaxHelper.Null;

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			if (propertyType is not INamedTypeSymbol namedType)
			{
				diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueNotSupported, attribute.ApplicationSyntaxReference, propertyType);
				return SyntaxHelper.Null;
			}

			var typeName = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var member = propertyType.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(v => Equals(v.ConstantValue, value));
			if (member != null)
				return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(typeName), IdentifierName(member.Name));

			var underlyingType = namedType.EnumUnderlyingType!;
			if (TryParseCore(attribute, diagnostics, underlyingType, namedType, value, out var expression))
				expression = CastExpression(IdentifierName(typeName), expression);

			return expression;
		}
		else
		{
			TryParseCore(attribute, diagnostics, propertyType, propertyType, value, out var expression);
			return expression;
		}
	}

	private static void ParseAttribute(AttributeData attribute, DiagnosticsBuilder diagnostics, INamedTypeSymbol declaringType, string propertyName, ITypeSymbol propertyType, Accessibility accessibility, Accessibility writeAccessibility, out SyntaxGeneratorSharedProperties result)
	{
		var defaultModeSyntax = DefaultDefaultModeSyntax;
		var defaultValueSyntax = (ExpressionSyntax)SyntaxHelper.Null;
		var defaultValueFactory = false;
		var coerceValueCallback = false;
		var validateValueCallback = false;

		foreach (var (name, constant) in attribute.NamedArguments)
		{
			switch (name)
			{
				case nameof(BindablePropertyAttribute.DefaultValue):
					defaultValueSyntax = ParseDefaultValue(attribute, diagnostics, constant.Type, constant.Value);
					break;
				case nameof(BindablePropertyAttribute.DefaultMode):
					defaultModeSyntax = ParseDefaultBindingMode(attribute, constant.Value, diagnostics);
					break;
				case nameof(BindablePropertyAttribute.DefaultValueFactory):
					defaultValueFactory = Equals(constant.Value, true);
					break;
				case nameof(BindablePropertyAttribute.CoerceValueCallback):
					coerceValueCallback = Equals(constant.Value, true);
					break;
				case nameof(BindablePropertyAttribute.ValidateValueCallback):
					validateValueCallback = Equals(constant.Value, true);
					break;
			}
		}

		var declaringTypeSyntax = declaringType.ToIdentifier();
		var annotatedPropertyTypeSyntax = propertyType.ToIdentifier(true);
		var propertyTypeSyntax = propertyType.ToIdentifier();

		result = new SyntaxGeneratorSharedProperties(propertyName, declaringTypeSyntax, propertyTypeSyntax, annotatedPropertyTypeSyntax, accessibility, writeAccessibility,
			defaultValueSyntax, defaultModeSyntax, defaultValueFactory, coerceValueCallback, validateValueCallback);
	}


	private static bool StringStartsAndEndsWith(string value, string start, string end, StringComparison comparison = StringComparison.Ordinal)
		=> value.StartsWith(start) && value.AsSpan(start.Length).Equals(end.AsSpan(), comparison);

	private static CreateResult CreateForProperty(IPropertySymbol symbol, PropertyDeclarationSyntax node, AttributeData attribute)
	{
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = symbol.SetMethod?.DeclaredAccessibility ?? Accessibility.Private;

		ParseAttribute(attribute, diagnostics, symbol.ContainingType, symbol.Name, symbol.Type, accessibility, writeAccessibility, out var props);
		var generator = new BindableInstancePropertySyntaxGenerator(in props);
		return new(node.GetReference(), symbol.ContainingType, generator, diagnostics.ToImmutable());
	}

	private static CreateResult CreateForMethod(IMethodSymbol symbol, MethodDeclarationSyntax node, AttributeData attribute, CancellationToken token)
	{
		var name = Identifiers.GetAttachedPropertyName(symbol.Name);

		if (symbol.ReturnsVoid)
		{
			var error = DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn.CreateDiagnostic(node, symbol, name);
			return new(node.GetReference(), symbol.ContainingType, error);
		}

		var arguments = symbol.Parameters;
		if (arguments.Length == 0)
		{
			var error = DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature.CreateDiagnostic(node, symbol, name);
			return new(node.GetReference(), symbol.ContainingType, error);
		}

		var attachedType = arguments[0].Type;
		var propertyType = symbol.ReturnType;
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = accessibility;
		var generatedGetterInfo = default(AttachedPropertyGetterInfo);
		var generatedSetterInfo = default(AttachedPropertySetterInfo);

		if (symbol.IsPartialDefinition)
		{
			generatedGetterInfo = new(symbol.Name, node.ParameterList, node.Modifiers);
		}

		var setMethod = symbol.ContainingType.GetAttachedSetMethod(propertyType, attachedType, name);
		if (setMethod == null)
		{
			var modifiers = node.Modifiers.Remove(SyntaxKind.PartialKeyword);
			generatedSetterInfo = new(node.ParameterList.Parameters[0].Identifier, Identifier("value"), modifiers);
		}
		else
		{
			writeAccessibility = setMethod.DeclaredAccessibility;

			var setterNode = TryGetNode<MethodDeclarationSyntax>(setMethod, token);
			if (setterNode != null && setMethod.IsPartialDefinition)
				generatedSetterInfo = new(setterNode.ParameterList, setterNode.Modifiers);
		}

		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		ParseAttribute(attribute, diagnostics, symbol.ContainingType, name, propertyType, accessibility, writeAccessibility, out var props);

		var generator = new BindableAttachedPropertySyntaxGenerator(in props, attachedType.ToIdentifier(), generatedGetterInfo, generatedSetterInfo);
		return new(node.GetReference(), symbol.ContainingType, generator, diagnostics);
	}

	private static T? TryGetNode<T>(ISymbol symbol, CancellationToken token) where T : SyntaxNode
	{
		foreach (var location in symbol.Locations)
		{
			var root = location.SourceTree?.GetRoot(token);
			if (root?.FindNode(location.SourceSpan) is T node)
				return node;
		}

		return null;
	}

	private static CreateResult TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return context.TargetSymbol.Kind switch
		{
			SymbolKind.Property => CreateForProperty((IPropertySymbol)context.TargetSymbol, (PropertyDeclarationSyntax)context.TargetNode, context.Attributes[0]),
			SymbolKind.Method => CreateForMethod((IMethodSymbol)context.TargetSymbol, (MethodDeclarationSyntax)context.TargetNode, context.Attributes[0], token),
			_ => throw new InvalidOperationException("Unexpected syntax node: " + context.TargetSymbol.Kind)
		};
	}

	private static GenerationOutput GroupGenerators(ImmutableArray<CreateResult> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<PropertyCollection>();
		var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, Dictionary<string, BindablePropertySyntaxGenerator>>(SymbolEqualityComparer.Default);

		foreach (var (owner, declaringType, generator, diagnostics) in values)
		{
			if (!diagnostics.IsDefaultOrEmpty)
				diagnosticsBuilder.AddRange(diagnostics);
			
			if (generator == null)
				continue;

			if (!map.TryGetValue(declaringType, out var namedProperties))
			{
				map[declaringType] = namedProperties = new();
				types.Add(new(declaringType, namedProperties.Values));
			}
			else if (namedProperties.ContainsKey(generator.PropertyName))
			{
				diagnosticsBuilder.Add(DiagnosticDescriptors.BindablePropertyDuplicateName, owner);
				continue;
			}

			namedProperties[generator.PropertyName] = generator;
		}

		return new(diagnosticsBuilder.ToImmutable(), types.ToImmutable());
	}

	private static void GenerateOutput(SourceProductionContext context, GenerationOutput generationOutput)
	{
		foreach (var diagnostic in generationOutput.Diagnostics)
			context.ReportDiagnostic(diagnostic);

		foreach (var type in generationOutput.Types)
		{
			var classInfo = ClassInfo.Create(type.DeclaringType);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, classInfo.TypeName).AddModifier(SyntaxKind.PartialKeyword);
			var members = new List<MemberDeclarationSyntax>();

			foreach (var generator in type.Properties)
				generator.GenerateMembers(members);

			declaration = declaration.AddMembers([.. members]);
			declaration = classInfo.ParentTypes.Aggregate(declaration, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).AddModifier(SyntaxKind.PartialKeyword).AddMembers(current));

			var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
			var ns = NamespaceDeclaration(IdentifierName(classInfo.Namespace)).AddMembers(declaration);
			var unit = CompilationUnit()
				.AddMembers(ns)
				.WithLeadingTrivia(Trivia(nullableEnable))
				.AddFormatting();

			var fileName = classInfo.GetFileName();
			context.AddSource(fileName, unit, Encoding.UTF8);
			context.CancellationToken.ThrowIfCancellationRequested();
		}
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataType(AttributeType, TransformMember);
		var collected = info.Collect().Select(GroupGenerators);
		context.RegisterSourceOutput(collected, GenerateOutput);
	}
}