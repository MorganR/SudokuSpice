using BenchmarkDotNet.Attributes;
using SudokuSharp;
using SudokuSpice.ConstraintBased;
using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.RuleBased;
using System;
using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
    [SimpleJob]
    public class GeneratorBenchmarker
    {
        [Benchmark(Baseline = true)]
        public int SudokuSpice()
        {
            var generator = new StandardPuzzleGenerator(9);
            var puzzle = generator.Generate(30, TimeSpan.FromSeconds(10));
            return puzzle.NumEmptySquares;
        }

        [Benchmark]
        public int SudokuSpiceConstraints()
        {
            var generator = new ConstraintBasedGenerator<Puzzle>(
                () => new Puzzle(9),
                new List<IConstraint<Puzzle>>
                {
                    new RowUniquenessConstraint<Puzzle>(),
                    new ColumnUniquenessConstraint<Puzzle>(),
                    new BoxUniquenessConstraint<Puzzle>(),
                });
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
