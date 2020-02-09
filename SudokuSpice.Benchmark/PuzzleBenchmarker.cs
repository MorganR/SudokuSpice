using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace SudokuSpice.Benchmark
{
    [SimpleJob]
    public class PuzzleBenchmarker
    {
        public IEnumerable<object> NineByNinePuzzles => Puzzles.NineByNinePuzzles();

        public IEnumerable<object> MegaPuzzles => Puzzles.MegaPuzzles();

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpice(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var restrict = new StandardRestrict(p);
            var sudoku = new Solver(
                new FlexibleSquareTracker(
                    p,
                    new List<ISudokuRestrict> { restrict },
                    new List<ISudokuHeuristic>
                    {
                    new UniqueInRowHeuristic(p, restrict),
                    new UniqueInColumnHeuristic(p, restrict),
                    new UniqueInBoxHeuristic(p, restrict)
                    }));
            sudoku.Solve();
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSolverLite(PuzzleSample puzzle)
        {
            // Must clone the input, since this method solves the puzzle in-place.
            return SudokuSolver.SudokuSolver.Solve((int[,])puzzle.Matrix.Clone());
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSharp(PuzzleSample puzzle)
        {
            var board = new SudokuSharp.Board();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (puzzle.NullableMatrix[row, col].HasValue)
                    {
                        board.PutCell(new SudokuSharp.Location(col, row), puzzle.NullableMatrix[row, col].Value);
                    }
                }
            }

            board = board.Fill.Sequential();

            return board.IsSolved;
        }
    }
}