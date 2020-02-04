using BenchmarkDotNet.Running;

namespace SudokuSpice.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzleSummary = BenchmarkRunner.Run<PuzzleBenchmarker>();
            var csvSummary = BenchmarkRunner.Run<CsvBenchmarker>();
        }
    }
}
