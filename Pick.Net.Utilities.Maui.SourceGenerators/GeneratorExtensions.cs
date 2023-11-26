using System.Collections;
using System.Reflection;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class GeneratorExtensions
{
	private static readonly Dictionary<SyntaxKind, AttributeTargets> TargetsMap = new()
	{
		[SyntaxKind.ClassDeclaration]		= AttributeTargets.Class,
		[SyntaxKind.ConstructorDeclaration] = AttributeTargets.Constructor,
		[SyntaxKind.DelegateDeclaration]	= AttributeTargets.Delegate,
		[SyntaxKind.EnumDeclaration]		= AttributeTargets.Enum,
		[SyntaxKind.EventDeclaration]		= AttributeTargets.Event,
		[SyntaxKind.FieldDeclaration]		= AttributeTargets.Field,
		[SyntaxKind.GenericName]			= AttributeTargets.GenericParameter,
		[SyntaxKind.InterfaceDeclaration]	= AttributeTargets.Interface,
		[SyntaxKind.MethodDeclaration]		= AttributeTargets.Method | AttributeTargets.ReturnValue,
		[SyntaxKind.Parameter]				= AttributeTargets.Parameter,
		[SyntaxKind.PropertyDeclaration]	= AttributeTargets.Property,
		[SyntaxKind.StructDeclaration]		= AttributeTargets.Struct,
	};

	public static IncrementalValuesProvider<IGrouping<TKey, T>> GroupBy<T, TKey>(this IncrementalValuesProvider<T> source, Func<T, TKey> keySelector)
		=> GroupBy(source, keySelector, v => v);

	public static IncrementalValuesProvider<IGrouping<TKey, TValue>> GroupBy<T, TKey, TValue>(this IncrementalValuesProvider<T> source, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
	{
		return source.Collect().SelectMany((array, token) =>
		{
			var dict = new Dictionary<TKey, List<TValue>>();

			foreach (var value in array)
			{
				var key = keySelector.Invoke(value);
				if (!dict.TryGetValue(key, out var group))
					dict[key] = group = new();

				var val = valueSelector.Invoke(value);
				group.Add(val);
			}

			var builder = ImmutableArray.CreateBuilder<IGrouping<TKey, TValue>>(dict.Count);

			foreach (var (key, values) in dict)
				builder.Add(new Grouping<TKey, TValue>(key, values));

			return builder.ToImmutable();
		});
	}

	public static IncrementalValuesProvider<T> ForAttributeWithMetadataType<T>(this SyntaxValueProvider provider, Type attributeType, Func<GeneratorAttributeSyntaxContext, CancellationToken, T> transform)
	{
		var usage = attributeType.GetCustomAttribute<AttributeUsageAttribute>(true) ?? throw new ArgumentException("AttributeUsageAttribute is not defined on type " + attributeType.FullName);
		return provider.ForAttributeWithMetadataName(attributeType.FullName!, (node, _) => MatchesTarget(node, usage.ValidOn), transform);
	}

	private static bool MatchesTarget(SyntaxNode node, AttributeTargets targets)
	{
		var kind = node.Kind();
		return targets == AttributeTargets.All || (TargetsMap.TryGetValue(kind, out var flag) && (targets & flag) != 0);
	}

	private sealed record Grouping<TKey, TValue>(TKey Key, IEnumerable<TValue> Values) : IGrouping<TKey, TValue>
	{
		public IEnumerator<TValue> GetEnumerator()
			=> Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}
}
