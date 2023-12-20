using System.Diagnostics.CodeAnalysis;

namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

internal readonly struct Result<T> where T : class
{
	public T? Value { get; }

	public Diagnostic? Error { get; }

	public Result(T value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public Result(Diagnostic error)
	{
		Error = error ?? throw new ArgumentNullException(nameof(error));
	}

	public Result(DiagnosticDescriptor descriptor, ISymbol owner, params object?[] messageArgs)
	{
		Error = descriptor.CreateDiagnostic(owner, messageArgs);
	}

	public bool IsSuccessful([MaybeNullWhen(false)] out T value, [MaybeNullWhen(true)] out Diagnostic error)
	{
		(value, error) = (Value, Error);
		return value != null;
	}
}
