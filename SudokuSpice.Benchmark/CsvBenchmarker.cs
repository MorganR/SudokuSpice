using BenchmarkDotNet.Attributes;
using SudokuSpice.RuleBased;
using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
    [MemoryDiagnoser]
    public class CsvBenchmarker
    {
        private static IReadOnlyList<SudokuSample> _samples => SudokuCsvParser.ParseCsv();

        public IEnumerable<object> SampleCollections => new List<PuzzleSampleCollection>
        {
            new PuzzleSampleCollection(_samples, 0, 1),
            new PuzzleSampleCollection(_samples, 1, 2),
            new PuzzleSampleCollection(_samples, 2, 4),
            new PuzzleSampleCollection(_samples, 4, 8),
            new PuzzleSampleCollection(_samples, 8, int.MaxValue),
        };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(SampleCollections))]
        public bool SudokuSpice(PuzzleSampleCollection sampleCollection)
        {
            var puzzle = PuzzleWithPossibleValues.CopyFrom(sampleCollection.Random().NullableJaggedMatrix);
            var solver = RuleBased.StandardPuzzles.CreateSolver();
            return solver.TrySolve(puzzle);
        }

        [Benchmark]
        [ArgumentsSource(nameof(SampleCollections))]
        public bool SudokuSpiceConstraints(PuzzleSampleCollection sampleCollection)
        {
            var solver = ConstraintBased.StandardPuzzles.CreateSolver();
            var puzzle = Puzzle.CopyFrom(sampleCollection.Random().NullableJaggedMatrix);
            return solver.TrySolve(puzzle);
        }

        [Benchmark]
        [ArgumentsSource(nameof(SampleCollections))]
        public bool SudokuSharp(PuzzleSampleCollection sampleCollection)
        {
            PuzzleSample sample = sampleCollection.Random();
            var board = new SudokuSharp.Board();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sample.NullableMatrix[row, col].HasValue)
                    {
                        board.PutCell(new SudokuSharp.Location(col, row), sample.NullableMatrix[row, col].Value);
                    }
                }
            }

            board = board.Fill.Sequential();

            return board.IsSolved;
        }

        [Benchmark]
        [ArgumentsSource(nameof(SampleCollections))]
        public bool SudokuSolverLite(PuzzleSampleCollection sampleCollection) =>
            // Must clone the input, since this method solves the puzzle in-place.
            SudokuSolver.SudokuSolver.Solve((int[,])sampleCollection.Random().Matrix.Clone());
    }
}