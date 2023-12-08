// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using Pick.Net.Utilities.Benchmarks;

var config = DefaultConfig.Instance
	.AddDiagnoser(MemoryDiagnoser.Default);
//	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core70));

BenchmarkRunner.Run<EnumsBenchmarks>(config);
