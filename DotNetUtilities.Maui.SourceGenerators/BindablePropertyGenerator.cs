using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using DotNetUtilities.Maui.SourceGenerators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration.SourceGenerators
{
	[Generator]
	public class BindablePropertyGenerator : IIncrementalGenerator
	{
		private const string attributeName = "DotNetUtilities.Maui.Helpers.BindablePropertyAttribute";

		private static readonly IdentifierNameSyntax nameValue = IdentifierName("value");
		private static readonly IdentifierNameSyntax nameGetValue = IdentifierName("GetValue");
		private static readonly IdentifierNameSyntax nameSetValue = IdentifierName("SetValue");
		private static readonly IdentifierNameSyntax nameBindableProperty = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty");
		private static readonly IdentifierNameSyntax nameBindablePropertyKey = IdentifierName("global::Microsoft.Maui.Controls.BindablePropertyKey");
		private static readonly IdentifierNameSyntax nameBindablePropertyKeyProperty = IdentifierName("BindableProperty");
		private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");
		private static readonly IdentifierNameSyntax nameCreateReadOnly = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly");

		private static readonly SyntaxTokenList tokensPublic = CreateTokenList(SyntaxKind.PublicKeyword);

		private static readonly IReadOnlyDictionary<PropertyAccessLevel, SyntaxTokenList> accessLevelTokens = new Dictionary<PropertyAccessLevel, SyntaxTokenList>()
		{
			[PropertyAccessLevel.Public] = tokensPublic,
			[PropertyAccessLevel.Protected] = CreateTokenList(SyntaxKind.ProtectedKeyword),
			[PropertyAccessLevel.Internal] = CreateTokenList(SyntaxKind.InternalKeyword),
			[PropertyAccessLevel.Private] = CreateTokenList(SyntaxKind.PrivateKeyword),
			[PropertyAccessLevel.ProtectedInternal] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword),
			[PropertyAccessLevel.ProtectedPrivate] = CreateTokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword),
		};

		private static SyntaxTokenList CreateTokenList(SyntaxKind kind)
			=> new(Token(kind));

		private static SyntaxTokenList CreateTokenList(params SyntaxKind[] kinds)
			=> new(kinds.Select(Token));

		private static FieldDeclarationSyntax GenerateReadOnlyBindablePropertyKeyDeclaration(BindablePropertyEntry entry, TypeSyntax declaringType, IdentifierNameSyntax propertyType, out IdentifierNameSyntax bindablePropertyKeyField)
		{
			bindablePropertyKeyField = IdentifierName(entry.PropertyName + "PropertyKey");
			var propertyInitializer = InvocationExpression(nameCreateReadOnly)
				.AddArgumentListLiteralArgument(entry.PropertyName)
				.AddArgumentListTypeOfArgument(declaringType)
				.AddArgumentListTypeOfArgument(propertyType)
				.AddArgumentListNullArgument();

			var declarator = VariableDeclarator(bindablePropertyKeyField.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

			return FieldDeclaration(VariableDeclaration(nameBindablePropertyKey).AddVariables(declarator))
				.WithModifiers(entry.SetModifiers)
				.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
		}

		private static FieldDeclarationSyntax GenerateReadOnlyBindablePropertyDeclaration(BindablePropertyEntry entry, IdentifierNameSyntax bindablePropertyKeyField, out IdentifierNameSyntax bindablePropertyField)
		{
			bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
			var propertyInitializer = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, bindablePropertyKeyField, nameBindablePropertyKeyProperty);
			var declarator = VariableDeclarator(bindablePropertyField.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

			return FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
				.WithModifiers(entry.GetModifiers)
				.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
		}

		private static PropertyDeclarationSyntax GenerateReadOnlyBindablePropertyAccessors(BindablePropertyEntry entry, TypeSyntax propertyType, TypeSyntax bindablePropertyKeyField, TypeSyntax bindablePropertyField)
		{
			return PropertyDeclaration(propertyType, entry.PropertyName)
				.WithModifiers(entry.GetModifiers)
				.AddAccessorListAccessors(
					GenerateGetter(propertyType, bindablePropertyField),
					GenerateSetter(bindablePropertyKeyField, entry.SetModifiers));
		}

		private static FieldDeclarationSyntax GenerateBindablePropertyDeclaration(BindablePropertyEntry entry, TypeSyntax declaringType, IdentifierNameSyntax propertyType, out IdentifierNameSyntax bindablePropertyField)
		{
			bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
			var propertyInitializer = InvocationExpression(nameCreate)
				.AddArgumentListLiteralArgument(entry.PropertyName)
				.AddArgumentListTypeOfArgument(declaringType)
				.AddArgumentListTypeOfArgument(propertyType);

			var declarator = VariableDeclarator(bindablePropertyField.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

			return FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
				.WithModifiers(entry.GetModifiers)
				.AddModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
		}

		private static AccessorDeclarationSyntax GenerateGetter(TypeSyntax propertyType, TypeSyntax bindablePropertyField)
		{
			var expression = CastExpression(propertyType, InvocationExpression(nameGetValue).AddArgumentListArguments(Argument(bindablePropertyField)));

			return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
				.WithExpressionBody(ArrowExpressionClause(expression))
				.WithSemicolonToken();
		}

		private static AccessorDeclarationSyntax GenerateSetter(TypeSyntax bindablePropertyField, in SyntaxTokenList accessors)
		{
			var expression = InvocationExpression(nameSetValue)
				.AddArgumentListArguments(
					Argument(bindablePropertyField),
					Argument(nameValue));

			return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
				.WithModifiers(accessors)
				.WithExpressionBody(ArrowExpressionClause(expression))
				.WithSemicolonToken();
		}

		private static PropertyDeclarationSyntax GenerateBindablePropertyAccessors(BindablePropertyEntry entry, TypeSyntax propertyType, TypeSyntax bindablePropertyField)
		{
			return PropertyDeclaration(propertyType, entry.PropertyName)
				.WithModifiers(entry.GetModifiers)
				.AddAccessorListAccessors(
					GenerateGetter(propertyType, bindablePropertyField),
					GenerateSetter(bindablePropertyField, entry.SetModifiers));
		}

		private static void RegisterEntry(SourceProductionContext context, (ClassEntry Class, ImmutableArray<Diagnostic> Diagnostics) item)
		{
			var (clazz, diagnostics) = item;
			foreach (var diagnostic in diagnostics)
				context.ReportDiagnostic(diagnostic);

			var identifier = Identifier(clazz.TypeName);
			var fullIdentifier = IdentifierName(clazz.FullName);
			var type = TypeDeclaration(SyntaxKind.ClassDeclaration, identifier)
				.AddModifier(SyntaxKind.PartialKeyword);

			foreach (var entry in clazz.Properties)
			{
				var propertyType = IdentifierName(entry.PropertyType);
				if (entry.SetModifiers.Count == 0)
				{
					type = type.AddMembers(
						GenerateBindablePropertyDeclaration(entry, fullIdentifier, propertyType, out var bindablePropertyField),
						GenerateBindablePropertyAccessors(entry, propertyType, bindablePropertyField));
				}
				else
				{
					type = type.AddMembers(
						GenerateReadOnlyBindablePropertyKeyDeclaration(entry, fullIdentifier, propertyType, out var bindablePropertyKeyField),
						GenerateReadOnlyBindablePropertyDeclaration(entry, bindablePropertyKeyField, out var bindablePropertyField),
						GenerateReadOnlyBindablePropertyAccessors(entry, propertyType, bindablePropertyKeyField, bindablePropertyField));
				}

			}

			var ns = NamespaceDeclaration(IdentifierName(clazz.Namespace)).AddMembers(type);
			var unit = CompilationUnit().AddMembers(ns).NormalizeWhitespace();

			context.AddSource(clazz.FileName, unit);
		}

		private static bool ToSyntaxTokens(SyntaxReference? reference, in TypedConstant value, out SyntaxTokenList tokens, [MaybeNullWhen(true)] out Diagnostic diagnostic)
		{
			var level = (PropertyAccessLevel)Convert.ToInt32(value.Value);

			if (accessLevelTokens.TryGetValue(level, out tokens))
			{
				diagnostic = null;
				return true;
			}
			else
			{
				var location = reference == null ? null : Location.Create(reference.SyntaxTree, reference.Span);
				diagnostic = Diagnostic.Create(DiagnosticDescriptors.BindablePropertyInvalidAccessor, location, level);
				return false;
			}
		}

		private static (ClassEntry Class, ImmutableArray<Diagnostic> Diagnostics) MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
		{
			var type = (ITypeSymbol)context.TargetSymbol;
			var ns = type.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
			var typeReference = type.GetFullTypeName();
			var attributes = context.Attributes;
			var entries = new BindablePropertyEntry[attributes.Length];
			var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

			Diagnostic? diagnostic;

			for (var i = 0; i < entries.Length; i++)
			{
				var attribute = attributes[i];
				var arguments = attribute.ConstructorArguments;
				var name = (string?)arguments[0].Value ?? "";
				var propType = (ITypeSymbol)arguments[1].Value!;

				SyntaxTokenList getterAccessors = tokensPublic;
				SyntaxTokenList setterAccessors = default;

				foreach (var (key, value) in attribute.NamedArguments)
				{
					switch (key)
					{
						case "AccessLevel":
							if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out getterAccessors, out diagnostic))
								diagnostics.Add(diagnostic);

							break;
						case "WriteAccessLevel":
							if (!ToSyntaxTokens(attribute.ApplicationSyntaxReference!, value, out setterAccessors, out diagnostic))
								diagnostics.Add(diagnostic);

							break;
					}
				}

				entries[i] = new(name, propType.GetFullTypeName(), getterAccessors, setterAccessors);
			}

			var classEntry = new ClassEntry(ns, type.Name, typeReference, $"{ns}.{type.MetadataName}.g.cs", ImmutableArray.Create(entries));
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
}