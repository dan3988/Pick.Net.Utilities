using System.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class HandlerAddPartialPlatformClassFixer() : BaseCodeFixProvider(DiagnosticDescriptors.HandlerAddPartialPlatformClass)
{
	private static bool PlatformFileExists(INamedTypeSymbol symbol, ReadOnlySpan<char> platform)
	{
		foreach (var location in symbol.Locations)
		{
			var path = location.SourceTree?.FilePath;
			if (path != null && path.Length > 3 + platform.Length)
			{
				var span = path.AsSpan(path.Length - platform.Length - 3, platform.Length);
				if (span.Equals(platform, StringComparison.OrdinalIgnoreCase))
					return true;
			}
		}

		return false;
	}

	private static SyntaxNode? GetBaseType(SyntaxGenerator generator, TypeDeclarationSyntax node)
	{
		var list = node.BaseList?.Types ?? default;
		if (list.Count >= 0 && list[0].IsKind(SyntaxKind.SimpleBaseType))
			return null;

		return generator.GenericName("Microsoft.Maui.Handlers.ViewHandler", [node, generator.IdentifierName("Android.Views.View")]);
	}

	protected override async Task RegisterActionsAsync(CodeFixContext context, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic)
	{
		if (!node.IsKind(SyntaxKind.ClassDeclaration) || !context.TextDocument.Name.EndsWith(".cs"))
			return;

		var semanticModel = await context.Document.GetSemanticModelAsync();
		if (semanticModel == null)
			return;

		var type = (TypeDeclarationSyntax)node;
		var symbol = semanticModel.GetDeclaredSymbol(type);
		if (symbol == null)
			return;

		var document = context.Document;
		var path = document.FilePath;
		if (path == null || !path.EndsWith(".cs"))
			return;

		if (!TryGetProperty("Platform", out var platform) || PlatformFileExists(symbol, platform.AsSpan())
			|| !TryGetProperty("Properties", out var properties)
			|| !TryGetProperty("ViewType", out var viewType)
			|| !TryGetProperty("HandlerType", out var handlerType))
			return;

		var propList = properties?.Split([';'], StringSplitOptions.RemoveEmptyEntries) ?? [];

		var generator = new FileGenerator(document, type, platform, propList, handlerType, viewType);
		var fix = CodeAction.Create("Add " + platform + " file", generator.GenerateAsync, "HandlerAddPartialPlatformClassFixer." + platform);
		context.RegisterCodeFix(fix, diagnostic);
		return;

		bool TryGetProperty(string key, [MaybeNullWhen(false)] out string value)
			=> diagnostic.Properties.TryGetValue(key, out value) && value != null;
	}

	private static SyntaxNode[] GenerateMembers(SyntaxGenerator generator, string[] props, string handlerTypeName, string viewTypeName)
	{
		if (props.Length == 0)
			return [];

		var handlerType = generator.GlobalType(handlerTypeName);
		var viewType = generator.GlobalType(viewTypeName);
		var parameters = new[]
		{
			generator.ParameterDeclaration("handler", handlerType),
			generator.ParameterDeclaration("view", viewType)
		};

		var members = new SyntaxNode[props.Length];

		for (var i = 0; i < props.Length; i++)
		{
			var name = "Map" + props[i];
			members[i] = generator.MethodDeclaration(name, parameters, null, SyntaxHelper.TypeVoid, Accessibility.Private, DeclarationModifiers.Static.WithPartial(true), []);
		}

		return members;
	}

	private sealed class FileGenerator(Document document, TypeDeclarationSyntax node, string identifier, string[] props, string handlerTypeName, string viewTypeName)
	{
		public Task<Solution> GenerateAsync(CancellationToken cancellationToken)
		{
			var generator = SyntaxGenerator.GetGenerator(document);
			var baseType = GetBaseType(generator, node);
			var members = GenerateMembers(generator, props, handlerTypeName, viewTypeName);
			var root = generator.ClassDeclaration(node.Identifier.Text, null, Accessibility.NotApplicable, DeclarationModifiers.Partial, baseType, null, members);
			var current = node.Parent;

			while (current != null && current.IsKind(SyntaxKind.ClassDeclaration))
				root = generator.ClassDeclaration(((TypeDeclarationSyntax)current).Identifier.Text, null, Accessibility.NotApplicable, DeclarationModifiers.Partial);

			if (current is BaseNamespaceDeclarationSyntax n)
			{
				root = n.IsKind(SyntaxKind.FileScopedNamespaceDeclaration)
					? SyntaxFactory.FileScopedNamespaceDeclaration(n.Name).AddMembers((MemberDeclarationSyntax)root)
					: generator.NamespaceDeclaration(n.Name, root);
			}

			root = ((CSharpSyntaxNode)root).AddLineBreaks();

			var project = document.Project;
			var id = DocumentId.CreateNewId(project.Id);
			var solution = project.Solution.AddDocument(id, $"{document.Name[..^2]}{identifier}.cs", generator.CompilationUnit(root));

			return Task.FromResult(solution);
		}
	}
}
