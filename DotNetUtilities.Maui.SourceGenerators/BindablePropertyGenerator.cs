using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using DotNetUtilities.Maui.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetUtilities.Maui.SourceGenerators;

[Generator]
public class BindablePropertyGenerator : IIncrementalGenerator
{
	private static readonly Type attributeType = typeof(BindablePropertyAttribute);
	private static readonly string attributeName = attributeType.FullName;

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
			BindablePropertySyntaxFactory.GenerateBindablePropertyMembers(ref type, fullIdentifier, entry);

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

			var getterAccessors = tokensPublic;
			var setterAccessors = default(SyntaxTokenList);
			var attachedType = default(string);

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
					case "AttachedType":
						attachedType = ((ITypeSymbol?)value.Value)?.GetFullTypeName();
						break;
				}
			}

			entries[i] = new(name, propType.GetFullTypeName(), getterAccessors, setterAccessors, attachedType);
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