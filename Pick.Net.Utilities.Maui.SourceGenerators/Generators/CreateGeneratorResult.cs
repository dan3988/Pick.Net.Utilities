using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly record struct CreateGeneratorResult
{
	public BindablePropertySyntaxGenerator? Result { get; }

	public Diagnostic? Error { get; }

	public CreateGeneratorResult(BindablePropertySyntaxGenerator result)
	{
		Result = result ?? throw new ArgumentNullException(nameof(result));
	}

	public CreateGeneratorResult(Diagnostic error)
	{
		Error = error ?? throw new ArgumentNullException(nameof(error));
	}
	
	public CreateGeneratorResult(DiagnosticDescriptor descriptor, ISymbol owner, params object?[] messageArgs)
	{
		Error = descriptor.CreateDiagnostic(owner, messageArgs);
	}

	public bool IsSuccessful([MaybeNullWhen(false)] out BindablePropertySyntaxGenerator result, [MaybeNullWhen(true)] out Diagnostic error)
	{
		(result, error) = (Result, Error);
		return result != null;
	}
}