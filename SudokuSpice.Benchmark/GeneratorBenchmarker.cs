using BenchmarkDotNet.Attributes;
using SudokuSharp;
using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.RuleBased;
using System;
using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
    public class GeneratorBenchmarker
    {
        [Benchmark(Baseline = true)]
        public int SudokuSpice()
        {
            var generator = new StandardPuzzleGenerator();
            RuleBased.Puzzle puzzle = generator.Generate(9, 30, TimeSpan.FromSeconds(10));
            return puzzle.NumEmptySquares;
        }

        [Benchmark]
        public int SudokuSpiceConstraints()
        {
            var generator = new ConstraintBased.PuzzleGenerator<Puzzle>(
                () => new Puzzle(9),
                new List<IConstraint>
                {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                });
            Puzzle puzzle = generator.Generate(30, TimeSpan.FromSeconds(10));
            return puzzle.NumEmptySquares;
        }

        [Benchmark]
        public Board SudokuSharpSingles()
        {
            var rand = new Random();
            Board board = Factory.Solution(rand);
            Board puzzle = Factory.Puzzle(board, rand, QuadsToCut: 0, PairsToCut: 0, SinglesToCut: 51);
            return puzzle;
        }

        [Benchmark]
        public Board SudokuSharpMixed()
        {
            var rand = new Random();
            Board board = Factory.Solution(rand);
            Board puzzle = Factory.Puzzle(board, rand, QuadsToCut: 6, PairsToCut: 8, SinglesToCut: 11);
            return puzzle;
        }
    }
}