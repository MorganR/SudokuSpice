using BenchmarkDotNet.Running;

namespace SudokuSpice.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[] {
                typeof(PuzzleBenchmarker),
                typeof(CsvBenchmarker),
                typeof(GeneratorBenchmarker)
            }).Run(args);
        }
    }
}