using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using DotNetUtilities.Maui.Helpers;
using DotNetUtilities.Maui.SourceGenerators.Syntax;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators;

using DiagnosticsBuilder = ImmutableArray<Diagnostic>.Builder;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly IdentifierNameSyntax nameBindingMode = IdentifierName("global::Microsoft.Maui.Controls.BindingMode");
	private static readonly IdentifierNameSyntax nameBindingModeOneWay = IdentifierName(nameof(BindingMode.OneWay));

	private static readonly Type attributeType = typeof(BindablePropertyAttribute);
	private static readonly string attributeName = attributeType.FullName;

	private static readonly SyntaxTokenList tokensPublic = CreateTokenList(SyntaxKind.PublicKeyword);

	private static readonly IReadOnlyDictionary<PropertyVisibility, SyntaxTokenList> visibilityTokens = new Dictionary<PropertyVisibility, SyntaxTokenList>()
	{
		[PropertyVisibility.Public] = tokensPublic,
		[PropertyVisibility.Protected] = CreateTokenList(SyntaxKind.ProtectedKeyword),
		[PropertyVisibility.Internal] = CreateTokenList(SyntaxKind.InternalKeyword),
		[PropertyVisibility.Private] = CreateTokenList(SyntaxKind.PrivateKeyword),
		[PropertyVisibility.ProtectedInternal] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword),
		[PropertyVisibility.ProtectedPrivate] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword),
	};

	private static SyntaxTokenList CreateTokenList(SyntaxKind kind)
		=> new(Token(kind));

	private static SyntaxTokenList CreateTokenList(params SyntaxKind[] kinds)
		=> new(kinds.Select(Token));

	private static void RegisterEntry(SourceProductionContext context, (ClassEntry Class, ImmutableArray<Diagnostic> Diagnostics) item)
	{
		var (clazz, diagnostics) = item;
		foreach (var diagnostic in diagnostics)
			context.ReportDiagnostic(diagnostic);

		var identifier = Identifier(clazz.TypeName);
		var type = TypeDeclaration(SyntaxKind.ClassDeclaration, identifier)
			.AddModifier(SyntaxKind.PartialKeyword);

		foreach (var property in clazz.Properties)
			type = property.Generate(type);

		var ns = NamespaceDeclaration(IdentifierName(clazz.Namespace)).AddMembers(type);
		var unit = CompilationUnit().AddMembers(ns).AddFormatting();

		context.AddSource(clazz.FileName, unit);
#if DEBUG
		var text = unit.ToFullString();
		Console.WriteLine(clazz.FileName);
		Console.WriteLine(text);
#endif
	}

	private static bool ToSyntaxTokens(SyntaxReference? reference, in TypedConstant value, out SyntaxTokenList tokens, [MaybeNullWhen(true)] out Diagnostic diagnostic)
	{
		var level = (PropertyVisibility)Convert.ToInt32(value.Value);

		if (visibilityTokens.TryGetValue(level, out tokens))
		{
			diagnostic = null;
			return true;
		}
		else
		{
			var location = reference == null ? null : Location.Create(reference.SyntaxTree, reference.Span);
			diagnostic = Diagnostic.Create(DiagnosticDescriptors.BindablePropertyInvalidVisibility, location, level);
			return false;
		}
	}

	private static bool TryParseAttributePositionalArgs(AttributeData attribute, DiagnosticsBuilder builder, [MaybeNullWhen(false)] out string name, [MaybeNullWhen(false)] out IdentifierNameSyntax type)
	{
		var arguments = attribute.ConstructorArguments;
		if (arguments.Length < 2)
		{
			name = null;
			type = null;
			return false;
		}

		var canConstruct = true;

		name = (string?)arguments[0].Value ?? "";

		if (string.IsNullOrEmpty(name))
		{
			builder.Add(DiagnosticDescriptors.BindablePropertyEmptyPropertyName, attribute.ApplicationSyntaxReference);
			canConstruct = false;
		}

		var symbol = (ITypeSymbol?)arguments[1].Value;
		if (symbol == null)
		{
			builder.Add(DiagnosticDescriptors.BindablePropertyNullPropertyType, attribute.ApplicationSyntaxReference);
			type = IdentifierName("object");
		}
		else
		{
			type = IdentifierName(symbol.GetFullTypeName());
		}

		return canConstruct;
	}

	private static ExpressionSyntax ParseDefaultBindingMode(AttributeData attribute, TypedConstant value, DiagnosticsBuilder diagnostics)
	{
		var enumValue = Enum.ToObject(typeof(BindingMode), value.Value);
		if (!Enum.IsDefined(typeof(BindingMode), enumValue))
		{
			diagnostics.Add(DiagnosticDescriptors.BindablePropertyInvalidDefaultMode, attribute.ApplicationSyntaxReference, enumValue);
			return CastExpression(nameBindingMode, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)enumValue)));
		}
		else
		{
			return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nameBindingMode, IdentifierName(enumValue.ToString()));
		}
	}

	private static (ClassEntry Class, ImmutableArray<Diagnostic> Diagnostics) MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var type = (ITypeSymbol)context.TargetSymbol;
		var ns = type.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
		var declaringTypeSyntax = IdentifierName(type.GetFullTypeName());
		var attributes = context.Attributes;
		var properties = ImmutableArray.CreateBuilder<BindablePropertySyntaxGenerator>(attributes.Length);
		var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

		Diagnostic? diagnostic;

		for (var i = 0; i < attributes.Length; i++)
		{
			var attribute = attributes[i];
			if (!TryParseAttributePositionalArgs(attribute, diagnostics, out var name, out var propType))
				continue;

			var getterAccessors = tokensPublic;
			var setterAccessors = default(SyntaxTokenList);
			var attachedType = default(string);
			var defaultModeSyntax = (ExpressionSyntax)MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nameBindingMode, nameBindingModeOneWay);

			foreach (var (key, value) in attribute.NamedArguments)
			{
				switch (key)
				{
					case nameof(BindablePropertyAttribute.DefaultMode):
						defaultModeSyntax = ParseDefaultBindingMode(attribute, value, diagnostics);
						break;
					case nameof(BindablePropertyAttribute.Visibility):
						if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out getterAccessors, out diagnostic))
							diagnostics.Add(diagnostic);

						break;
					case nameof(BindablePropertyAttribute.WriteVisibility):
						if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out setterAccessors, out diagnostic))
							diagnostics.Add(diagnostic);

						break;
					case nameof(BindablePropertyAttribute.AttachedType):
						attachedType = ((ITypeSymbol?)value.Value)?.GetFullTypeName();
						break;
				}
			}

			BindablePropertySyntaxGenerator generator = attachedType == null
				? InstanceBindablePropertySyntaxGenerator.Create(name, propType, declaringTypeSyntax, defaultModeSyntax, getterAccessors, setterAccessors)
				: AttachedBindablePropertySyntaxGenerator.Create(name, propType, declaringTypeSyntax, defaultModeSyntax, IdentifierName(attachedType), getterAccessors, setterAccessors);

			properties.Add(generator);
		}

		var classEntry = new ClassEntry(ns, type.Name, $"{ns}.{type.MetadataName}.g.cs", properties.ToImmutable());
		return (classEntry, diagnostics.ToImmutable());
	}

	private static bool MetadataPredictate(SyntaxNode node, CancellationToken token)
		=> node.IsKind(SyntaxKind.ClassDeclaration);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var info = context.SyntaxProvider.ForAttributeWithMetadataName(attributeName, MetadataPredictate, MetadataTransform);
		context.RegisterSourceOutput(info, RegisterEntry);
	}
}