namespace Pick.Net.Utilities.Benchmarks;

using BenchmarkDotNet.Attributes;

using Pick.Net.Utilities.Collections;

using Kvp = KeyValuePair<string, string>;

[MemoryDiagnoser]
public class StringDictionaryBenchmarks
{
	private const int Count = 100;
	public static readonly Kvp[] Pairs = Enumerable.Range(1, Count).Select(v => new Kvp("key" + v, "value" + v)).ToArray();
	public static readonly string JoinedKeys = string.Join(';', Pairs.Select(v => v.Key));

	[Benchmark(Baseline = true)]
	public void NormalDictionary()
	{
		var dict = new Dictionary<string, string>(Pairs);

		for (var i = 0; i < Count;)
		{
			var key = "key" + (++i);
			var value = dict.ContainsKey(key);
		}
	}

	[Benchmark]
	public void StringDictionary()
	{
		var dict = new StringDictionary<string>(Pairs);
		var startIndex = 0;
		var charCount = 4;

		for (var i = 0; i < Count;)
		{
			if (++i == 10 || i == 100)
				charCount++;

			var key = JoinedKeys.AsSpan(startIndex, charCount);
			var value = dict.ContainsKey(key);

			startIndex += charCount + 1;
		}
	}
}
