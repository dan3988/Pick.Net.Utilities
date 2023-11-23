using System.Collections;
using System.Collections.Immutable;

namespace Pick.Net.Utilities.Maui.SourceGenerators;

internal static class GeneratorExtensions
{
	private sealed record Grouping<TKey, TValue>(TKey Key, IEnumerable<TValue> Values) : IGrouping<TKey, TValue>
	{
		public IEnumerator<TValue> GetEnumerator()
			=> Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}

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
}
