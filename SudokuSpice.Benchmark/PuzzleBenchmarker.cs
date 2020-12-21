using BenchmarkDotNet.Attributes;
using SudokuSpice.ConstraintBased;
using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.RuleBased;
using SudokuSpice.RuleBased.Heuristics;
using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;

namespace SudokuSpice.Benchmark
{
    public class PuzzleBenchmarker
    {
        public IEnumerable<object> NineByNinePuzzles => Puzzles.NineByNinePuzzles();

        public IEnumerable<object> MegaPuzzles => Puzzles.MegaPuzzles();

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpice(PuzzleSample puzzle)
        {
            var p = new RuleBased.Puzzle(puzzle.NullableMatrix);
            PuzzleSolver solver = StandardPuzzles.CreateSolver();
            solver.SolveRandomly(p);
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicSingle(PuzzleSample puzzle)
        {
            var p = new RuleBased.Puzzle(puzzle.NullableMatrix);
            var standardRules = new StandardRules();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<ISudokuRule> { standardRules });
            var heuristic = new StandardHeuristic(
                standardRules, standardRules, standardRules);
            var solver = new PuzzleSolver(ruleKeeper, heuristic);
            solver.SolveRandomly(p);
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicMultiple(PuzzleSample puzzle)
        {
            var p = new RuleBased.Puzzle(puzzle.NullableMatrix);
            var rowRule = new RowUniquenessRule();
            var columnRule = new ColumnUniquenessRule();
            var boxRule = new BoxUniquenessRule();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<ISudokuRule> { rowRule, columnRule, boxRule });
            var heuristic = new StandardHeuristic(
                rowRule, columnRule, boxRule);
            var solver = new PuzzleSolver(ruleKeeper, heuristic);
            solver.SolveRandomly(p);
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceConstraints(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var solver = new PuzzleSolver<Puzzle>(
                new IConstraint[] {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint()
                });
            solver.Solve(p);
            return p.NumEmptySquares == 0;
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