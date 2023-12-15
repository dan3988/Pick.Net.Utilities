// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

using Pick.Net.Utilities.Benchmarks;

//BenchmarkRunner.Run<EnumsBenchmarks>();
//BenchmarkRunner.Run<StringDictionaryBenchmarks>();
BenchmarkRunner.Run<MapBenchmarks>();
