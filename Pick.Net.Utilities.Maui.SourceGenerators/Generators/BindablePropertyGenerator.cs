﻿using System.Collections.Immutable;
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

		var declaringTypeSyntax = declaringType.ToIdentifier();
		var annotatedPropertyTypeSyntax = propertyType.ToIdentifier(true);
		var propertyTypeSyntax = propertyType.ToIdentifier();

		result = new SyntaxGeneratorSharedProperties(propertyName, declaringTypeSyntax, propertyTypeSyntax, annotatedPropertyTypeSyntax, accessibility, writeAccessibility,
			defaultValueSyntax, defaultModeSyntax, defaultValueFactory, coerceValueCallback, validateValueCallback);
	}

	private static void CheckBindablePropertyIsUsed(DiagnosticsBuilder diagnostics, PropertyDeclarationSyntax prop, SyntaxKind accessorKind, string propertyName, DiagnosticDescriptor descriptor)
	{
		var accessor = prop.AccessorList?.Accessors.FirstOrDefault(v => v.IsKind(accessorKind));
		if (accessor == null)
			return;

		var suffix = accessor.Modifiers.Count == 0 ? "Property" : "PropertyKey";
		if (!accessor.SearchRecursive<IdentifierNameSyntax>(v => StringStartsAndEndsWith(v.Identifier.Text, propertyName, suffix)))
			diagnostics.Add(descriptor, accessor, propertyName);
	}

	private static bool StringStartsAndEndsWith(string value, string start, string end, StringComparison comparison = StringComparison.Ordinal)
		=> value.StartsWith(start) && value.AsSpan(start.Length).Equals(end.AsSpan(), comparison);

	private static CreateResult CreateForProperty(IPropertySymbol symbol, PropertyDeclarationSyntax node, AttributeData attribute)
	{
		if (symbol.GetMethod == null)
		{
			var error = DiagnosticDescriptors.BindablePropertyNoGetter.CreateDiagnostic(node, symbol.Name);
			return new(node.GetReference(), symbol.ContainingType, error);
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
		return new(node.GetReference(), symbol.ContainingType, generator, diagnostics.ToImmutable());
	}

	private static CreateResult CreateForMethod(IMethodSymbol symbol, MethodDeclarationSyntax node, SemanticModel model, AttributeData attribute, CancellationToken token)
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
			return new(node.GetReference(), symbol.ContainingType, diagnostics);
		}

		var arguments = symbol.Parameters;
		if (arguments.Length != 1)
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, node, name);
			return new(node.GetReference(), symbol.ContainingType, diagnostics);
		}

		var attachedType = arguments[0].Type;
		var propertyType = symbol.ReturnType;
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = accessibility;
		var generatedGetterInfo = default(AttachedPropertyGetterInfo);
		var generatedSetterInfo = default(AttachedPropertySetterInfo);

		if (symbol.IsPartialDefinition)
		{
			generatedGetterInfo = new(node.ParameterList, node.Modifiers);
		}

		var setMethod = symbol.ContainingType.GetMembers().SelectMethods().Where(IsSetMethod).FirstOrDefault();
		if (setMethod == null)
		{
			var modifiers = node.Modifiers;
			var partialIndex = modifiers.IndexOf(SyntaxKind.PartialKeyword);
			if (partialIndex >= 0)
				modifiers = modifiers.RemoveAt(partialIndex);

			generatedSetterInfo = new(node.ParameterList, modifiers);
		}
		else
		{
			writeAccessibility = setMethod.DeclaredAccessibility;

			if (setMethod.IsPartialDefinition)
			{
				var location = setMethod.Locations.FirstOrDefault();
				if (location != null)
				{
					var root = location.SourceTree?.GetRoot(token);
					var setterNode = root?.FindNode(location.SourceSpan) as MethodDeclarationSyntax;
					if (setterNode != null)
						generatedSetterInfo = new(setterNode.ParameterList, setterNode.Modifiers);
				}
			}
		}

		ParseAttribute(attribute, diagnostics, symbol.ContainingType, name, propertyType, accessibility, writeAccessibility, out var props);

		var generator = new BindableAttachedPropertySyntaxGenerator(in props, attachedType.ToIdentifier(), generatedGetterInfo, generatedSetterInfo);
		return new(node.GetReference(), symbol.ContainingType, generator, diagnostics);

		bool IsSetMethod(IMethodSymbol symbol)
		{
			if (!StringStartsAndEndsWith(symbol.Name, "Set", name))
				return false;

			if (!symbol.ReturnsVoid)
				return false;

			if (symbol.Parameters.Length != 2)
				return false;

			var objParam = symbol.Parameters[0];
			var objValue = symbol.Parameters[1];
			return SymbolEqualityComparer.Default.Equals(objParam.Type, attachedType) && SymbolEqualityComparer.Default.Equals(objValue.Type, propertyType);
		}
	}

	private static CreateResult TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		return context.TargetSymbol.Kind switch
		{
			SymbolKind.Property => CreateForProperty((IPropertySymbol)context.TargetSymbol, (PropertyDeclarationSyntax)context.TargetNode, context.Attributes[0]),
			SymbolKind.Method => CreateForMethod((IMethodSymbol)context.TargetSymbol, (MethodDeclarationSyntax)context.TargetNode, context.SemanticModel, context.Attributes[0], token),
			_ => throw new InvalidOperationException("Unexpected syntax node: " + context.TargetSymbol.Kind)
		};
	}

	private static GenerationOutput GroupGenerators(ImmutableArray<CreateResult> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<PropertyCollection>();
		var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, Dictionary<string, BindablePropertySyntaxGenerator>>(SymbolEqualityComparer.Default);

		//foreach (var value in values)
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