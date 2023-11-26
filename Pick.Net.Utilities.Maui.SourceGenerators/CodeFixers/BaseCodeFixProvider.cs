using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

public abstract class BaseCodeFixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

	protected BaseCodeFixProvider(DiagnosticDescriptor dignostic)
	{
		FixableDiagnosticIds = ImmutableArray.Create(dignostic.Id);
	}

	protected BaseCodeFixProvider(params DiagnosticDescriptor[] dignostics)
	{
		FixableDiagnosticIds = dignostics.Select(v => v.Id).ToImmutableArray();
	}

	public override FixAllProvider GetFixAllProvider()
		=> WellKnownFixAllProviders.BatchFixer;

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var root = await document.GetSyntaxRootAsync(context.CancellationToken);
		if (root == null)
			return;

		foreach (var diagnostic in context.Diagnostics)
		{
			var span = diagnostic.Location.SourceSpan;
			var node = root.FindNode(span);
			var action = CreateAction(document, root, node, diagnostic);
			if (action != null)
			{
				context.RegisterCodeFix(action, diagnostic);
			}
		}
	}

	protected abstract CodeAction? CreateAction(Document document, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic);
}

public abstract class BaseCodeFixProvider<T> : BaseCodeFixProvider where T : SyntaxNode
{
	protected BaseCodeFixProvider(DiagnosticDescriptor dignostic) : base(dignostic)
	{
	}

	protected BaseCodeFixProvider(params DiagnosticDescriptor[] dignostics) : base(dignostics)
	{
	}

	protected sealed override CodeAction? CreateAction(Document document, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic)
		=> node is T t ? CreateAction(document, root, t, diagnostic) : null;

	protected abstract CodeAction? CreateAction(Document document, SyntaxNode root, T node, Diagnostic diagnostic);
}
