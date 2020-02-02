using BenchmarkDotNet.Running;

namespace SudokuSpice.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SudokuSpiceBenchmarker>();
        }
    }
}
