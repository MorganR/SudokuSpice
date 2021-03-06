using SudokuSpice.RuleBased.Heuristics;
using SudokuSpice.RuleBased.Rules;
using SudokuSpice.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleSolverTest
    {
        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void Solve_ValidPuzzle_SolvesPuzzleCopy(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            var solved = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void Solve_WithInvalidPuzzle_Throws(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle));
        }

        [Fact]
        public void Solve_LeavesPuzzleUnchanged()
        {
            var puzzle = new PuzzleWithPossibleValues(9);
            var solver = StandardPuzzles.CreateSolver();
            var solved = solver.Solve(puzzle);
            for (int row = 0; row < puzzle.Size; ++row)
            {
                for (int col = 0; col < puzzle.Size; ++col)
                {
                    Assert.False(puzzle[row, col].HasValue);
                    Assert.True(solved[row, col].HasValue);
                }
            }
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void TrySolve_ValidPuzzle_SolvesPuzzleInPlace(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.True(solver.TrySolve(puzzle));
            PuzzleTestUtils.AssertStandardPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolve_WithInvalidPuzzle_ReturnsFalse(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.False(solver.TrySolve(puzzle));
        }

        [Fact]
        public void TrySolve_MegaPuzzle_Solves()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {null, null, null, null, 10,   1,    null, 8,    null, 15,   3,    11,   null, 2,    16,   null},
                new int?[] {14,   null, 2,    null, null, 4,    3,    null, null, 13,   8,    null, null, 12,   null, null},
                new int?[] {null, null, null, 12,   null, null, null, 15,   null, null, null, 7,    null, null, 9,    10},
                new int?[] {1,    10,   15,   null, 6,    null, null, null, null, 14,   null, null, null, null, null, 11},
                new int?[] {null, 11,   14,   6,    null, null, null, 9,    13,   8,    null, null, null, null, 2,    3},
                new int?[] {12,   null, null, null, 4,    null, 7,    3,    11,   6,    null, null, 16,   null, 5,    null},
                new int?[] {13,   16,   null, 2,    null, null, null, 1,    null, null, 5,    null, 10,   9,    null, null},
                new int?[] {null, 4,    null, null, 13,   null, 2,    null, null, null, 16,   3,    11,   null, null, null},
                new int?[] {null, null, null, 10,   3,    6,    null, null, null, 9,    null, 12,   null, null, 4,    null},
                new int?[] {null, null, 12,   15,   null, 9,    null, null, 7,    null, null, null, 1,    null, 3,    14},
                new int?[] {null, 1,    null, 4,    null, null, 5,    12,   3,    10,   null, 8,    null, null, null, 2},
                new int?[] {3,    6,    null, null, null, null, 15,   10,   4,    null, null, null, 12,   5,    7,    null},
                new int?[] {2,    null, null, null, null, null, 4,    null, null, null, null, 15,   null, 16,   11,   9},
                new int?[] {4,    14,   null, null, 16,   null, null, null, 2,    null, null, null, 6,    null, null, null},
                new int?[] {null, null, 16,   null, null, 7,    8,    null, null, 4,    10,   null, null, 14,   null, 5},
                new int?[] {null, 3,    6,    null, 9,    12,   14,   null, 8,    null, 13,   16,   null, null, null, null}
            });
            var rowRule = new RowUniquenessRule();
            var columnRule = new ColumnUniquenessRule();
            var boxRule = new BoxUniquenessRule();
            var diagonalRule = new DiagonalUniquenessRule();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<IRule> { rowRule, columnRule, boxRule, diagonalRule });
            var heuristic = new StandardHeuristic(
                rowRule, columnRule, boxRule);
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(ruleKeeper, heuristic);

            Assert.True(solver.TrySolve(puzzle));
            _AssertMegaPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void SolveRandomly_ValidPuzzle_SolvesPuzzleInPlace(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            var solved = solver.Solve(puzzle, randomizeGuesses: true);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void SolveRandomly_WithInvalidPuzzle_Throws(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle, randomizeGuesses: true));
        }

        [Fact]
        public void SolveRandomly_LeavesPuzzleUnchanged()
        {
            var puzzle = new PuzzleWithPossibleValues(9);
            var solver = StandardPuzzles.CreateSolver();
            var solved = solver.Solve(puzzle, randomizeGuesses: true);
            for (int row = 0; row < puzzle.Size; ++row)
            {
                for (int col = 0; col < puzzle.Size; ++col)
                {
                    Assert.False(puzzle[row, col].HasValue);
                    Assert.True(solved[row, col].HasValue);
                }
            }
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void TrySolveRandomly_ValidPuzzle_SolvesPuzzleInPlace(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.True(solver.TrySolve(puzzle, randomizeGuesses: true));
            PuzzleTestUtils.AssertStandardPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolveRandomly_WithInvalidPuzzle_ReturnsFalse(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.False(solver.TrySolve(puzzle, randomizeGuesses: true));
        }

        [Theory]
        [ClassData(typeof(PuzzlesWithStats))]
        public void ComputeStatsForAllSolutions_WithoutHeuristics_ReturnsExpectedResults(
            int?[][] matrix, SolveStats expectedStats)
        {
            // Skip heuristics so the stats are easy to fully define.
            var ruleKeeper = new StandardRuleKeeper();
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(ruleKeeper);
            Assert.Equal(expectedStats, solver.ComputeStatsForAllSolutions(new PuzzleWithPossibleValues(matrix)));
        }

        [Theory]
        [ClassData(typeof(PuzzlesWithStats))]
        public void ComputeStatsForAllSolutions_WithHeuristics_ReturnsExpectedNumSolutions(
            int?[][] matrix, SolveStats expectedStats)
        {
            var ruleKeeper = new StandardRuleKeeper();
            IRule rule = ruleKeeper.GetRules()[0];
            var heuristics = new StandardHeuristic(
                (IMissingRowValuesTracker)rule,
                (IMissingColumnValuesTracker)rule, (IMissingBoxValuesTracker)rule);
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(ruleKeeper, heuristics);
            Assert.Equal(
                expectedStats.NumSolutionsFound,
                solver.ComputeStatsForAllSolutions(new PuzzleWithPossibleValues(matrix)).NumSolutionsFound);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void ComputeStatsForAllSolutions_WithInvalidPuzzle_ReturnsNoSolutions(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();

            var stats = solver.ComputeStatsForAllSolutions(puzzle);

            Assert.Equal(0, stats.NumSolutionsFound);
        }

        [Fact]
        public void ComputeStatsForAllSolutions_WithCanceledToken_Throws()
        {
            var solver = StandardPuzzles.CreateSolver();
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => solver.ComputeStatsForAllSolutions(new PuzzleWithPossibleValues(9), cancellationSource.Token));
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void HasUniqueSolution_WithUniqueSolution_ReturnsTrue(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.True(solver.HasUniqueSolution(puzzle));
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void HasUniqueSolution_WithInvalidPuzzle_ReturnsFalse(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var solver = StandardPuzzles.CreateSolver();
            Assert.False(solver.HasUniqueSolution(puzzle));
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void HasUniqueSolution_LeavesPuzzleUnchanged(int?[][] matrix)
        {
            var puzzle = new PuzzleWithPossibleValues(matrix);
            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            var solver = StandardPuzzles.CreateSolver();

            solver.HasUniqueSolution(puzzle);

            for (int row = 0; row < puzzleCopy.Size; ++row)
            {
                for (int col = 0; col < puzzleCopy.Size; ++col)
                {
                    Assert.Equal(puzzleCopy[row, col], puzzle[row, col]);
                }
            }
        }

        [Fact]
        public void HasUniqueSolution_WithCanceledToken_Throws()
        {
            var solver = StandardPuzzles.CreateSolver();
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => solver.HasUniqueSolution(new PuzzleWithPossibleValues(9), cancellationSource.Token));
        }



        private static void _AssertMegaPuzzleSolved(PuzzleWithPossibleValues puzzle)
        {
            PuzzleTestUtils.AssertStandardPuzzleSolved(puzzle);
            var alreadyFound = new HashSet<int>(puzzle.Size);
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in the backward diagonal!");
            }
            alreadyFound.Clear();
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in the forward diagonal!");
            }
        }
    }
}