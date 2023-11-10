using System;

using Microsoft.CodeAnalysis;

namespace DotNetUtils.Maui.SourceGenerators;

[Generator]
public class SourceGenerator : ISourceGenerator
{
	public void Execute(GeneratorExecutionContext context)
	{
		Console.WriteLine("SourceGenerator Executed!");
	}

	public void Initialize(GeneratorInitializationContext context)
	{
		Console.WriteLine("SourceGenerator Initialized!");
	}
}
