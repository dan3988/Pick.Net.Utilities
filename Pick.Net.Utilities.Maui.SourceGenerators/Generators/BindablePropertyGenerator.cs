using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

using static SyntaxFactory;

using UngroupedCreateResult = (INamedTypeSymbol Owner, CreateGeneratorResult Result);

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly Type AttributeType = typeof(BindablePropertyAttribute);

	private static readonly IdentifierNameSyntax NameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax NameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

	private static readonly ExpressionSyntax DefaultDefaultModeSyntax = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, NameBindingModeOneWay);

	private static ExpressionSyntax ParseDefaultBindingMode(object? value)
	{
		var enumValue = value == null ? 0 : Enum.ToObject(typeof(BindingMode), value);
		if (!Enum.IsDefined(typeof(BindingMode), enumValue))
		{
			return CastExpression(NameBindingMode, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)enumValue)));
		}
		else
		{
			return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, NameBindingMode, IdentifierName(enumValue.ToString()));
		}
	}

	private static bool ParseDefaultValue(SemanticModel model, ISymbol owner, ITypeSymbol propertyType, ITypeSymbol attachedType, object? value, ref DefaultValueGenerator defaultValue, [MaybeNullWhen(true)] out Diagnostic error)
	{
		error = null;

		if (value is not string name)
			return true;

		var parentType = owner.ContainingType;
		var matches = parentType.GetMembers(name);
		if (matches.Length == 0)
		{
			error = DiagnosticDescriptors.BindablePropertyDefaultValueMemberNotFound.CreateDiagnostic(owner, name, parentType.Name);
			return false;
		}

		if (matches.Length > 1)
		{
			error = DiagnosticDescriptors.BindablePropertyDefaultValueMemberAmbiguous.CreateDiagnostic(owner, name, parentType.Name);
			return false;
		}

		var member = matches[0];
		if (member.Kind == SymbolKind.Property)
		{
			var prop = (IPropertySymbol)member;
			return CheckFieldOrProperty(model, owner, prop, prop.Type, propertyType, ref defaultValue, out error);
		}
		else if (member.Kind == SymbolKind.Field)
		{
			var field = (IFieldSymbol)member;
			return CheckFieldOrProperty(model, owner, field, field.Type, propertyType, ref defaultValue, out error);
		}
		else if (member.Kind == SymbolKind.Method)
		{
			var method = (IMethodSymbol)member;
			if (method.ReturnsVoid)
			{
				error = DiagnosticDescriptors.BindablePropertyDefaultValueGeneratorNoReturnType.CreateDiagnostic(owner, name, propertyType.ToIdentifier());
				return false;
			}

			var returnTypeConversion = model.Compilation.ClassifyConversion(method.ReturnType, propertyType);
			if (!AllowPropertyTypeConversion(returnTypeConversion, out var requireConvert))
			{
				error = DiagnosticDescriptors.BindablePropertyDefaultValueGeneratorWrongReturnType.CreateDiagnostic(owner, name, propertyType.ToIdentifier());
				return false;
			}

			if (owner.Kind == SymbolKind.Method)
			{
				if (!member.IsStatic)
					goto InvalidSignature;

				if (method.Parameters.Length == 0)
				{
					defaultValue = DefaultValueGenerator.StaticGenerator(Identifier(name), requireConvert);
					return true;
				}

				if (method.Parameters.Length > 1)
					goto InvalidSignature;

				var paramType = method.Parameters[0].Type;
				var conversion = model.Compilation.ClassifyConversion(attachedType, paramType);
				if (conversion.IsImplicit || conversion.IsIdentity)
				{
					defaultValue = DefaultValueGenerator.StaticGenerator(Identifier(name), requireConvert, paramType.ToIdentifier());
					return true;
				}

			InvalidSignature:
				error = DiagnosticDescriptors.BindablePropertyAttachedDefaultValueGeneratorInvalidSignature.CreateDiagnostic(owner, method.Name);
				return false;
			}
			else
			{
				if (method.Parameters.Length == 0)
				{
					var id = Identifier(name);
					defaultValue = method.IsStatic ? DefaultValueGenerator.StaticGenerator(id, requireConvert) : DefaultValueGenerator.InstanceGenerator(id, requireConvert);
					return true;
				}

				if (method.Parameters.Length > 1 || !method.IsStatic)
				{
					error = DiagnosticDescriptors.BindablePropertyInstanceDefaultValueGeneratorInvalidSignature.CreateDiagnostic(owner, method.Name);
					return false;
				}

				var paramType = method.Parameters[0].Type;
				var conversion = model.Compilation.ClassifyConversion(attachedType, paramType);
				if (!conversion.Exists)
				{
					error = DiagnosticDescriptors.BindablePropertyInstanceDefaultValueGeneratorInvalidSignature.CreateDiagnostic(owner, method.Name);
					return false;
				}

				defaultValue = DefaultValueGenerator.StaticGenerator(Identifier(name), !returnTypeConversion.IsIdentity, paramType.ToIdentifier());
				return true;
			}
		}
		else
		{
			error = DiagnosticDescriptors.BindablePropertyDefaultValueMemberNotFound.CreateDiagnostic(owner, name, parentType.Name);
			return false;
		}

		static bool CheckFieldOrProperty(SemanticModel model, ISymbol owner, ISymbol symbol, ITypeSymbol returnType, ITypeSymbol propertyType, ref DefaultValueGenerator defaultValue, [MaybeNullWhen(true)] out Diagnostic error)
		{
			var conversion = model.Compilation.ClassifyConversion(returnType, propertyType);
			if (!AllowPropertyTypeConversion(conversion, out var requireConvert))
			{
				error = DiagnosticDescriptors.BindablePropertyDefaultValueWrongType.CreateDiagnostic(owner, symbol.Name, propertyType.Name);
				return false;
			}

			if (!symbol.IsStatic)
			{
				error = DiagnosticDescriptors.BindablePropertyDefaultValueFieldNonStatic.CreateDiagnostic(owner, symbol.Name);
				return false;
			}

			error = null;
			defaultValue = DefaultValueGenerator.StaticValue(Identifier(symbol.Name), requireConvert);
			return true;
		}
	}

	private static bool AllowPropertyTypeConversion(Conversion conversion, out bool requireExplicitCast)
	{
		if (conversion.IsIdentity || conversion.IsBoxing)
		{
			requireExplicitCast = false;
			return true;
		}

		if (conversion.IsImplicit || conversion.IsEnumeration || conversion.IsUserDefined)
		{
			requireExplicitCast = true;
			return true;
		}

		requireExplicitCast = false;
		return false;
	}

	private static bool ParseAttribute(SemanticModel model, ISymbol symbol, AttributeData attribute, string propertyName, ITypeSymbol propertyType, ITypeSymbol attachedType, Accessibility accessibility, Accessibility writeAccessibility, out SyntaxGeneratorSharedProperties result, [MaybeNullWhen(true)] out Diagnostic error)
	{
		error = null;

		var defaultModeSyntax = DefaultDefaultModeSyntax;
		var defaultValue = DefaultValueGenerator.None;
		var coerceValueCallback = false;
		var validateValueCallback = false;

		foreach (var (name, constant) in attribute.NamedArguments)
		{
			switch (name)
			{
				case nameof(BindablePropertyAttribute.DefaultValue):
					if (!ParseDefaultValue(model, symbol, propertyType, attachedType, constant.Value, ref defaultValue, out error))
					{
						result = default;
						return false;
					}

					break;
				case nameof(BindablePropertyAttribute.DefaultMode):
					defaultModeSyntax = ParseDefaultBindingMode(constant.Value);
					break;
				case nameof(BindablePropertyAttribute.CoerceValueCallback):
					coerceValueCallback = Equals(constant.Value, true);
					break;
				case nameof(BindablePropertyAttribute.ValidateValueCallback):
					validateValueCallback = Equals(constant.Value, true);
					break;
			}
		}

		var declaringTypeSyntax = symbol.ContainingType.ToIdentifier();
		var annotatedPropertyTypeSyntax = propertyType.ToIdentifier(true);
		var propertyTypeSyntax = propertyType.ToIdentifier();

		result = new SyntaxGeneratorSharedProperties(propertyName, declaringTypeSyntax, propertyTypeSyntax, annotatedPropertyTypeSyntax, accessibility, writeAccessibility,
			defaultValue, defaultModeSyntax, coerceValueCallback, validateValueCallback);

		return true;
	}


	private static bool StringStartsAndEndsWith(string value, string start, string end, StringComparison comparison = StringComparison.Ordinal)
		=> value.StartsWith(start) && value.AsSpan(start.Length).Equals(end.AsSpan(), comparison);

	private static CreateGeneratorResult CreateForProperty(IPropertySymbol symbol, SemanticModel model, AttributeData attribute)
	{
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = symbol.SetMethod?.DeclaredAccessibility ?? Accessibility.Private;

		if (!ParseAttribute(model, symbol, attribute, symbol.Name, symbol.Type, symbol.Type, accessibility, writeAccessibility, out var props, out var error))
			return new(error);

		var generator = new BindableInstancePropertySyntaxGenerator(in props);
		return new(generator);
	}

	private static CreateGeneratorResult CreateForMethod(IMethodSymbol symbol, SemanticModel model, AttributeData attribute, CancellationToken token)
	{
		var name = Identifiers.GetAttachedPropertyName(symbol.Name);

		if (symbol.ReturnsVoid)
			return new(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodReturn, symbol, name);

		var arguments = symbol.Parameters;
		if (arguments.Length == 0)
			return new(DiagnosticDescriptors.BindablePropertyInvalidAttachedMethodSignature, symbol, name);

		var attachedType = arguments[0].Type;
		var propertyType = symbol.ReturnType;
		var accessibility = symbol.DeclaredAccessibility;
		var writeAccessibility = accessibility;
		var generatedGetterInfo = default(AttachedPropertyGetterInfo);
		var generatedSetterInfo = default(AttachedPropertySetterInfo);

		if (symbol.IsPartialDefinition)
		{
			generatedGetterInfo = new(symbol);
		}

		var setMethod = symbol.ContainingType.GetAttachedSetMethod(propertyType, attachedType, name);
		if (setMethod == null)
		{
			generatedSetterInfo = new(false, symbol.Parameters[0].Name, "value", symbol.DeclaredAccessibility);
		}
		else
		{
			writeAccessibility = setMethod.DeclaredAccessibility;

			if (setMethod.IsPartialDefinition)
				generatedSetterInfo = new(setMethod);
		}

		if (!ParseAttribute(model, symbol, attribute, name, propertyType, attachedType, accessibility, writeAccessibility, out var props, out var error))
			return new(error);

		var generator = new BindableAttachedPropertySyntaxGenerator(in props, attachedType.ToIdentifier(), generatedGetterInfo, generatedSetterInfo);
		return new(generator);
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

	private static UngroupedCreateResult TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var result = context.TargetSymbol.Kind switch
		{
			SymbolKind.Property => CreateForProperty((IPropertySymbol)context.TargetSymbol, context.SemanticModel, context.Attributes[0]),
			SymbolKind.Method => CreateForMethod((IMethodSymbol)context.TargetSymbol, context.SemanticModel, context.Attributes[0], token),
			_ => throw new InvalidOperationException("Unexpected syntax node: " + context.TargetSymbol.Kind)
		};

		return (context.TargetSymbol.ContainingType, result);
	}

	private static GenerationOutput GroupGenerators(ImmutableArray<UngroupedCreateResult> values, CancellationToken token)
	{
		var types = ImmutableArray.CreateBuilder<PropertyCollection>();
		var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
		var map = new Dictionary<INamedTypeSymbol, List<BindablePropertySyntaxGenerator>>(SymbolEqualityComparer.Default);

		foreach (var (declaringType, result) in values)
		{
			if (!result.IsSuccessful(out var generator, out var error))
			{
				diagnosticsBuilder.Add(error);
				continue;
			}

			if (!map.TryGetValue(declaringType, out var properties))
			{
				map[declaringType] = properties = [];
				types.Add(new(declaringType, properties));
			}

			properties.Add(generator);
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
			var declaration = TypeDeclaration(SyntaxKind.ClassDeclaration, classInfo.TypeName).WithModifiers(ModifierLists.Partial);
			var members = new List<MemberDeclarationSyntax>();

			foreach (var generator in type.Properties)
				generator.GenerateMembers(members);

			declaration = declaration.AddMembers([.. members]);
			declaration = classInfo.ParentTypes.Aggregate(declaration, (current, t) => TypeDeclaration(SyntaxKind.ClassDeclaration, t).WithModifiers(ModifierLists.Partial).AddMembers(current));

			var nullableEnable = NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true);
			var root = classInfo.Namespace == "" ? (MemberDeclarationSyntax)declaration : NamespaceDeclaration(IdentifierName(classInfo.Namespace)).AddMembers(declaration);
			var unit = CompilationUnit()
				.AddMembers(root)
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