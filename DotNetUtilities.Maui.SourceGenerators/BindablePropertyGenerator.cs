using System;
using System.Collections.Immutable;
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
		private static readonly IdentifierNameSyntax nameCreate = IdentifierName("global::Microsoft.Maui.Controls.BindableProperty.Create");

		private static FieldDeclarationSyntax GenerateBindablePropertyDeclaration(BindablePropertyEntry entry, TypeSyntax declaringType, out IdentifierNameSyntax propertyType, out IdentifierNameSyntax bindablePropertyField)
		{
			propertyType = IdentifierName(entry.PropertyType);
			bindablePropertyField = IdentifierName(entry.PropertyName + "Property");
			var propertyInitializer = InvocationExpression(nameCreate)
				.AddArgumentListLiteralArgument(entry.PropertyName)
				.AddArgumentListTypeOfArgument(declaringType)
				.AddArgumentListTypeOfArgument(propertyType);

			var declarator = VariableDeclarator(bindablePropertyField.Identifier).WithInitializer(EqualsValueClause(propertyInitializer));

			return FieldDeclaration(VariableDeclaration(nameBindableProperty).AddVariables(declarator))
				.AddModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
		}

		private static AccessorDeclarationSyntax GenerateGetter(TypeSyntax propertyType, TypeSyntax bindablePropertyField)
		{
			var expression = CastExpression(propertyType, InvocationExpression(nameGetValue).AddArgumentListArguments(Argument(bindablePropertyField)));

			return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
				.WithExpressionBody(ArrowExpressionClause(expression))
				.WithSemicolonToken();
		}

		private static AccessorDeclarationSyntax GenerateSetter(TypeSyntax bindablePropertyField)
		{
			var expression = InvocationExpression(nameSetValue)
				.AddArgumentListArguments(
					Argument(bindablePropertyField),
					Argument(nameValue));

			return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
				.WithExpressionBody(ArrowExpressionClause(expression))
				.WithSemicolonToken();
		}

		private static PropertyDeclarationSyntax GenerateBindablePropertyAccessors(BindablePropertyEntry entry, TypeSyntax propertyType, TypeSyntax bindablePropertyField)
		{
			return PropertyDeclaration(propertyType, entry.PropertyName)
				.AddModifier(SyntaxKind.PublicKeyword)
				.AddAccessorListAccessors(
					GenerateGetter(propertyType, bindablePropertyField),
					GenerateSetter(bindablePropertyField));
		}

		private static void RegisterEntry(SourceProductionContext context, ClassEntry item)
		{
			var identifier = Identifier(item.TypeName);
			var fullIdentifier = IdentifierName(item.FullName);
			var type = TypeDeclaration(SyntaxKind.ClassDeclaration, identifier)
				.AddModifier(SyntaxKind.PartialKeyword);

			foreach (var entry in item.Properties)
			{
				type = type.AddMembers(
					GenerateBindablePropertyDeclaration(entry, fullIdentifier, out var propertyType, out var bindablePropertyField),
					GenerateBindablePropertyAccessors(entry, propertyType, bindablePropertyField));
			}

			var ns = NamespaceDeclaration(IdentifierName(item.Namespace)).AddMembers(type);
			var unit = CompilationUnit().AddMembers(ns).NormalizeWhitespace();

			context.AddSource(item.FileName, unit);
		}

		private static ClassEntry MetadataTransform(GeneratorAttributeSyntaxContext context, CancellationToken token)
		{
			var type = (ITypeSymbol)context.TargetSymbol;
			var ns = type.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
			var typeReference = type.GetFullTypeName();
			var attributes = context.Attributes;
			var entries = new BindablePropertyEntry[attributes.Length];

			for (var i = 0; i < entries.Length; i++)
			{
				var attribute = attributes[i];
				var arguments = attribute.ConstructorArguments;
				var name = (string?)arguments[0].Value ?? "";
				var propType = (ITypeSymbol)arguments[1].Value!;
				entries[i] = new(name, propType.GetFullTypeName());
			}

			return new(ns, type.Name, typeReference, $"{ns}.{type.MetadataName}.g.cs", ImmutableArray.Create(entries));

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
