using System.Reflection;

using BenchmarkDotNet.Attributes;

namespace Pick.Net.Utilities.Benchmarks;

[MemoryDiagnoser]
public class EnumsBenchmarks
{
	public const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

	[Benchmark(Baseline = true)]
	public void HasFlags()
	{
		foreach (var value in Enums.GetValues<BindingFlags>())
			Flags.HasFlag(value);
	}

	[Benchmark]
	public void HasFlagsFast()
	{
		foreach (var value in Enums.GetValues<BindingFlags>())
			Flags.HasFlagsFast(value);
	}
}
