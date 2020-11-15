using BenchmarkDotNet.Attributes;
using SudokuSpice.Constraints;
using SudokuSpice.Data;
using SudokuSpice.Heuristics;
using SudokuSpice.Rules;
using System.Collections.Generic;

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
            var solver = new Solver(p);
            solver.SolveRandomly();
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicSingle(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var possibleValues = new PossibleValues(p);
            var standardRules = new StandardRules(p, possibleValues.AllPossible);
            var ruleKeeper = new DynamicRuleKeeper(
                p, possibleValues,
                new List<ISudokuRule> { standardRules });
            var heuristic = new StandardHeuristic(
                p, possibleValues, standardRules, standardRules, standardRules);
            var solver = new Solver(p, possibleValues, ruleKeeper, heuristic);
            solver.SolveRandomly();
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceDynamicMultiple(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var possibleValues = new PossibleValues(p);
            var rowRule = new RowUniquenessRule(p, possibleValues.AllPossible);
            var columnRule = new ColumnUniquenessRule(p, possibleValues.AllPossible);
            var boxRule = new BoxUniquenessRule(p, possibleValues.AllPossible, true);
            var ruleKeeper = new DynamicRuleKeeper(
                p, possibleValues,
                new List<ISudokuRule> { rowRule, columnRule, boxRule });
            var heuristic = new StandardHeuristic(
                p, possibleValues, rowRule, columnRule, boxRule);
            var solver = new Solver(p, possibleValues, ruleKeeper, heuristic);
            solver.SolveRandomly();
            return p.NumEmptySquares == 0;
        }

        [Benchmark]
        [ArgumentsSource(nameof(NineByNinePuzzles))]
        public bool SudokuSpiceConstraints(PuzzleSample puzzle)
        {
            var p = new Puzzle(puzzle.NullableMatrix);
            var solver = new ConstraintBasedSolver<Puzzle>(
                new IConstraint<Puzzle>[] {
                    new RowUniquenessConstraint<Puzzle>(),
                    new ColumnUniquenessConstraint<Puzzle>(),
                    new BoxUniquenessConstraint<Puzzle>()
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
        public bool SudokuSolverLite(PuzzleSample puzzle)
        {
            // Must clone the input, since this method solves the puzzle in-place.
            return SudokuSolver.SudokuSolver.Solve((int[,])puzzle.Matrix.Clone());
        }

    }
}