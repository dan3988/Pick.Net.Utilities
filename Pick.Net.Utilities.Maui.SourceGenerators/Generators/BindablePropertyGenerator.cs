using System.Collections.Immutable;
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

		var hasDefaultValue = !defaultValueSyntax.IsKind(SyntaxKind.NullLiteralExpression);
		if (hasDefaultValue)
		{
			if (defaultValueFactory)
			{
				diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueAndFactory, attribute.ApplicationSyntaxReference);
			}
		}
		else if (!defaultValueFactory && propertyType is { IsValueType: false, NullableAnnotation: NullableAnnotation.Annotated })
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyDefaultValueNull, attribute.ApplicationSyntaxReference);
		}

		result = new SyntaxGeneratorSharedProperties(attribute.ApplicationSyntaxReference, declaringType, propertyName, propertyType, accessibility, writeAccessibility,
			defaultValueSyntax, defaultModeSyntax, defaultValueFactory, coerceValueCallback, validateValueCallback);
	}

	private static CreateResult CreateForProperty(IPropertySymbol symbol, PropertyDeclarationSyntax node, AttributeData attribute)
	{
		if (symbol.GetMethod == null)
		{
			var error = DiagnosticDescriptors.BindablePropertyNoGetter.CreateDiagnostic(node, symbol.Name);
			return new CreateResult(error);
		}

		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = symbol.SetMethod?.DeclaredAccessibility ?? Accessibility.Private;
		var isAutoProperty = symbol.IsAutoProperty();
		if (isAutoProperty)
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInstanceAutoProperty, node);
		}
		else
		{
			CheckBindablePropertyIsUsed(diagnostics, node, SyntaxKind.GetAccessorDeclaration, symbol.Name, DiagnosticDescriptors.BindablePropertyNotReferencedInGetter);
			CheckBindablePropertyIsUsed(diagnostics, node, SyntaxKind.SetAccessorDeclaration, symbol.Name, DiagnosticDescriptors.BindablePropertyNotReferencedInSetter);
		}

		ParseAttribute(attribute, diagnostics, symbol.ContainingType, symbol.Name, symbol.Type, accessibility, writeAccessibility, out var props);
		var generator = new BindableInstancePropertySyntaxGenerator(in props);
		return new(generator, diagnostics.ToImmutable());
	}

	private static void CheckBindablePropertyIsUsed(DiagnosticsBuilder diagnostics, PropertyDeclarationSyntax prop, SyntaxKind accessorKind, string propertyName, DiagnosticDescriptor descriptor)
	{
		var accessor = prop.AccessorList?.Accessors.FirstOrDefault(v => v.IsKind(accessorKind));
		if (accessor == null)
			return;

		var suffix = accessor.Modifiers.Count == 0 ? "Property" : "PropertyKey";
		if (!accessor.SearchRecursive<IdentifierNameSyntax>(v => IsBindablePropertyReference(v, propertyName, suffix)))
			diagnostics.Add(descriptor, accessor, propertyName);
	}

	private static bool IsBindablePropertyReference(IdentifierNameSyntax syntax, string name, string suffix = "Property")
	{
		var text = syntax.Identifier.Text;
		return text.StartsWith(name) && text.AsSpan(name.Length).Equals(suffix.AsSpan(), StringComparison.Ordinal);
	}

	private static CreateResult CreateForMethod(IMethodSymbol symbol, MethodDeclarationSyntax node, AttributeData attribute)
	{
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
		var name = symbol.Name;
		if (name.StartsWith("Get"))
		{
			name = name.Substring(3);
		}
		else
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodName, node, name);
		}

		if (symbol.ReturnsVoid)
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn, node, name);
			return new(diagnostics);
		}

		var arguments = symbol.Parameters;
		if (arguments.Length != 1)
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, node, name);
			return new(diagnostics);
		}

		var accessibility = symbol.DeclaredAccessibility;
		ParseAttribute(attribute, diagnostics, symbol.ContainingType, name, symbol.ReturnType, accessibility, accessibility, out var props);
		var generatedGetterModifiers = symbol.IsPartialDefinition ? node.Modifiers : default;
		var generator = new BindableAttachedPropertySyntaxGenerator(in props, arguments[0].Type, generatedGetterModifiers, generatedGetterModifiers);
		return new(generator, diagnostics);
	}

	private static CreateResult TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return context.TargetSymbol.Kind switch
		{
			SymbolKind.Property => CreateForProperty((IPropertySymbol)context.TargetSymbol, (PropertyDeclarationSyntax)context.TargetNode, context.Attributes[0]),
			SymbolKind.Method => CreateForMethod((IMethodSymbol)context.TargetSymbol, (MethodDeclarationSyntax)context.TargetNode, context.Attributes[0]),
			_ => throw new InvalidOperationException("Unexpected syntax node: " + context.TargetSymbol.Kind)
		};
	}

	private static GenerationOutput GroupGenerators(ImmutableArray<CreateResult> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<PropertyCollection>();
		var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, Dictionary<string, BindablePropertySyntaxGenerator>>(SymbolEqualityComparer.Default);

		//foreach (var value in values)
		foreach (var (generator, diagnostics) in values)
		{
			if (!diagnostics.IsDefaultOrEmpty)
				diagnosticsBuilder.AddRange(diagnostics);
			
			if (generator == null)
				continue;

			if (!map.TryGetValue(generator.DeclaringType, out var namedProperties))
			{
				map[generator.DeclaringType] = namedProperties = new();
				types.Add(new(generator.DeclaringType, namedProperties.Values));
			}
			else if (namedProperties.ContainsKey(generator.PropertyName))
			{
				diagnosticsBuilder.Add(DiagnosticDescriptors.BindablePropertyDuplicateName, generator.Owner);
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

		//var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		foreach (var type in generationOutput.Types)
		{
			var classInfo = ClassInfo.Create(type.DeclaringType);
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, classInfo.TypeName).AddModifier(SyntaxKind.PartialKeyword);
			var members = new List<MemberDeclarationSyntax>();

			foreach (var generator in type.Properties)
				generator.GenerateMembers(members);

			declaration = declaration.AddMembers(members.ToArray());
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