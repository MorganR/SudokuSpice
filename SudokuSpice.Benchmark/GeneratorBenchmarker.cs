using BenchmarkDotNet.Attributes;
using SudokuSharp;
using System;

namespace SudokuSpice.Benchmark
{
    [SimpleJob]
    public class GeneratorBenchmarker
    {
        [Benchmark(Baseline = true)]
        public int SudokuSpice()
        {
            var generator = new Generator(9);
            var puzzle = generator.Generate(30, TimeSpan.FromSeconds(10));
            return puzzle.NumEmptySquares;
        }

        [Benchmark]
        public Board SudokuSharpSingles()
        {
            var rand = new Random();
            var board = Factory.Solution(rand);
            var puzzle = Factory.Puzzle(board, rand, QuadsToCut: 0, PairsToCut: 0, SinglesToCut: 51);
            return puzzle;
        }

        [Benchmark]
        public Board SudokuSharpMixed()
        {
            var rand = new Random();
            var board = Factory.Solution(rand);
            var puzzle = Factory.Puzzle(board, rand, QuadsToCut: 6, PairsToCut: 8, SinglesToCut: 11);
            return puzzle;
        }
    }
}
