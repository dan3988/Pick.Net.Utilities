using BenchmarkDotNet.Attributes;

using Pick.Net.Utilities.Collections;

namespace Pick.Net.Utilities.Benchmarks;

using Kvp = KeyValuePair<string, string>;

[MemoryDiagnoser]
public class MapBenchmarks
{
	private const int Count = 100;
	public static readonly Kvp[] Pairs = Enumerable.Range(1, Count).Select(v => new Kvp("key" + v, "value" + v)).ToArray();

	[Benchmark(Baseline = true)]
	public void Dictionary()
	{
		var map = new Dictionary<string, string>(Pairs);

		foreach (var (key, value) in Pairs)
		{
			map.TryGetValue(key, out var value2);
		}
	}

	[Benchmark]
	public void Map()
	{
		var map = new Map<string, string>(Pairs);

		foreach (var (key, value) in Pairs)
		{
			var value2 = map[key];
		}
	}
}
