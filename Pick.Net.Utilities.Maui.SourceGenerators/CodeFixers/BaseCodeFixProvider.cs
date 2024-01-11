﻿using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Pick.Net.Utilities.Maui.SourceGenerators.CodeFixers;

public abstract class BaseCodeFixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }

	protected BaseCodeFixProvider(DiagnosticDescriptor dignostic)
	{
		FixableDiagnosticIds = ImmutableArray.Create(dignostic.Id);
	}

	protected BaseCodeFixProvider(DiagnosticDescriptor first, params DiagnosticDescriptor[] rest)
	{
		FixableDiagnosticIds = rest.Length == 0 ? ImmutableArray.Create(first.Id) : Enumerable.Repeat(first, 1).Concat(rest).Select(v => v.Id).ToImmutableArray();
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
			await RegisterActionsAsync(context, root, node, diagnostic);
		}
	}

	protected abstract Task RegisterActionsAsync(CodeFixContext context, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic);
}

public abstract class BaseCodeFixProvider<T> : BaseCodeFixProvider where T : SyntaxNode
{
	public string Title { get; }

	public virtual string EquivelanceKey => GetType().FullName;

	public virtual CodeActionPriority Priority => CodeActionPriority.Default;

	protected BaseCodeFixProvider(string title, DiagnosticDescriptor dignostic) : base(dignostic)
	{
		Title = title;
	}

	protected BaseCodeFixProvider(string title, DiagnosticDescriptor first, params DiagnosticDescriptor[] rest) : base(first, rest)
	{
		Title = title;
	}

	protected sealed override Task RegisterActionsAsync(CodeFixContext context, SyntaxNode root, SyntaxNode node, Diagnostic diagnostic)
	{
		if (node is T t && CanFix(t))
		{
			var document = context.Document;
			var action = CodeAction.Create(Title, EditDocument, EquivelanceKey, Priority);
			context.RegisterCodeFix(action, diagnostic);

			async Task<Document> EditDocument(CancellationToken token)
			{
				var editor = await DocumentEditor.CreateAsync(document, token);
				return Fix(editor, t, diagnostic, token) ? editor.GetChangedDocument() : document;
			}
		}

		return Task.CompletedTask;
	}

	protected virtual bool CanFix(T node) => true;

	protected abstract bool Fix(DocumentEditor editor, T node, Diagnostic diagnostic, CancellationToken token);
}
