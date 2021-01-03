using BenchmarkDotNet.Attributes;
using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.RuleBased;
using SudokuSpice.RuleBased.Heuristics;
using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
    [MemoryDiagnoser]
    public class PuzzleBenchmarker
    {
        public IEnumerable<object> NineByNinePuzzles => Puzzles.NineByNinePuzzles();

        public IEnumerable<object> MegaPuzzles => Puzzles.MegaPuzzles();

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpice(PuzzleSample puzzle)
        {
            var p = new PuzzleWithPossibleValues(puzzle.NullableMatrix);
            var solver = StandardPuzzles.CreateSolver();
            var result = solver.Solve(p, randomizeGuesses: true);
            return result.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicSingle(PuzzleSample puzzle)
        {
            var p = new PuzzleWithPossibleValues(puzzle.NullableMatrix);
            var standardRules = new StandardRules();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<IRule> { standardRules });
            var heuristic = new StandardHeuristic(
                standardRules, standardRules, standardRules);
            var solver = new RuleBased.PuzzleSolver<PuzzleWithPossibleValues>(ruleKeeper, heuristic);
            var result = solver.Solve(p, randomizeGuesses: true);
            return result.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicMultiple(PuzzleSample puzzle)
        {
            var p = new PuzzleWithPossibleValues(puzzle.NullableMatrix);
            var rowRule = new RowUniquenessRule();
            var columnRule = new ColumnUniquenessRule();
            var boxRule = new BoxUniquenessRule();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<IRule> { rowRule, columnRule, boxRule });
            var heuristic = new StandardHeuristic(
                rowRule, columnRule, boxRule);
            var solver = new RuleBased.PuzzleSolver<PuzzleWithPossibleValues>(ruleKeeper, heuristic);
            var result = solver.Solve(p, randomizeGuesses: true);
            return result.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceConstraints(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var solver = new ConstraintBased.PuzzleSolver<Puzzle>(
                new IConstraint[] {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint()
                });
            var result = solver.Solve(p);
            return result.NumEmptySquares == 0;
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

            board = board.Fill.Randomized();

            return board.IsSolved;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSolverLite(PuzzleSample puzzle) =>
            // Must clone the input, since this method solves the puzzle in-place.
            SudokuSolver.SudokuSolver.Solve((int[,])puzzle.Matrix.Clone());

    }
}