namespace Pick.Net.Utilities.Maui.SourceGenerators.Generators;

public abstract class BaseGroupedCodeGenerator<T> : BaseCodeGenerator<IReadOnlyList<T>>
	where T : class
{
	private readonly record struct ResultAndType(INamedTypeSymbol Owner, Result<T> Result);

	private protected abstract Type AttributeType { get; }

	private ResultAndType TransformMemberInternal(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		var result = TransformMember(context, token);
		return new(context.TargetSymbol.ContainingType, result);
	}

	private protected abstract Result<T> TransformMember(GeneratorAttributeSyntaxContext context, CancellationToken token);

	private GeneratorOutput<IReadOnlyList<T>> GroupGenerators(ImmutableArray<ResultAndType> values, CancellationToken token)
	{
		var builder = GeneratorOutput.CreateBuilder<IReadOnlyList<T>>();
		var map = new Dictionary<INamedTypeSymbol, List<T>>(SymbolEqualityComparer.Default);

		foreach (var (declaringType, result) in values)
		{
			if (!result.IsSuccessful(out var value, out var error))
			{
				builder.AddDiagnostic(error);
				continue;
			}

			if (!map.TryGetValue(declaringType, out var properties))
			{
				map[declaringType] = properties = [];
				builder.AddType(new(declaringType, properties));
			}

			properties.Add(value);
		}

		return builder.Build();
	}

	private protected sealed override IncrementalValueProvider<GeneratorOutput<IReadOnlyList<T>>> Register(SyntaxValueProvider provider)
		=> provider.ForAttributeWithMetadataType(AttributeType, TransformMemberInternal).Collect().Select(GroupGenerators);
}
