using SudokuSpice.RuleBased.Heuristics;
using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleSolverTest
    {
        private readonly ITestOutputHelper _output;

        public PuzzleSolverTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void Solve_WithStandardConstructor_SolvesPuzzle(int?[,] puzzle)
        {
            var solver = StandardPuzzles.CreateSolver();
            solver.Solve(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void Solve_ValidPuzzle_SolvesPuzzle(int?[,] puzzle)
        {
            var rowRule = new RowUniquenessRule();
            var columnRule = new ColumnUniquenessRule();
            var boxRule = new BoxUniquenessRule();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<ISudokuRule> { rowRule, columnRule, boxRule });
            var heuristic = new StandardHeuristic(
                rowRule, columnRule, boxRule);
            var solver = new PuzzleSolver(ruleKeeper, heuristic);
            solver.Solve(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Fact]
        public void Solve_MegaPuzzle_Solves()
        {
            var puzzle = new int?[,] {
                {null, null, null, null, 10,   1,    null, 8,    null, 15,   3,    11,   null, 2,    16,   null},
                {14,   null, 2,    null, null, 4,    3,    null, null, 13,   8,    null, null, 12,   null, null},
                {null, null, null, 12,   null, null, null, 15,   null, null, null, 7,    null, null, 9,    10},
                {1,    10,   15,   null, 6,    null, null, null, null, 14,   null, null, null, null, null, 11},
                {null, 11,   14,   6,    null, null, null, 9,    13,   8,    null, null, null, null, 2,    3},
                {12,   null, null, null, 4,    null, 7,    3,    11,   6,    null, null, 16,   null, 5,    null},
                {13,   16,   null, 2,    null, null, null, 1,    null, null, 5,    null, 10,   9,    null, null},
                {null, 4,    null, null, 13,   null, 2,    null, null, null, 16,   3,    11,   null, null, null},
                {null, null, null, 10,   3,    6,    null, null, null, 9,    null, 12,   null, null, 4,    null},
                {null, null, 12,   15,   null, 9,    null, null, 7,    null, null, null, 1,    null, 3,    14},
                {null, 1,    null, 4,    null, null, 5,    12,   3,    10,   null, 8,    null, null, null, 2},
                {3,    6,    null, null, null, null, 15,   10,   4,    null, null, null, 12,   5,    7,    null},
                {2,    null, null, null, null, null, 4,    null, null, null, null, 15,   null, 16,   11,   9},
                {4,    14,   null, null, 16,   null, null, null, 2,    null, null, null, 6,    null, null, null},
                {null, null, 16,   null, null, 7,    8,    null, null, 4,    10,   null, null, 14,   null, 5},
                {null, 3,    6,    null, 9,    12,   14,   null, 8,    null, 13,   16,   null, null, null, null}
            };
            var rowRule = new RowUniquenessRule();
            var columnRule = new ColumnUniquenessRule();
            var boxRule = new BoxUniquenessRule();
            var diagonalRule = new DiagonalUniquenessRule();
            var ruleKeeper = new DynamicRuleKeeper(
                new List<ISudokuRule> { rowRule, columnRule, boxRule, diagonalRule });
            var heuristic = new StandardHeuristic(
                rowRule, columnRule, boxRule);
            var solver = new PuzzleSolver(ruleKeeper, heuristic);
            solver.Solve(puzzle);
            _AssertMegaPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void SolveRandomly_ValidPuzzle_SolvesPuzzle(int?[,] puzzle)
        {
            var solver = StandardPuzzles.CreateSolver();
            solver.SolveRandomly(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(PuzzlesWithStats))]
        public void GetStatsForAllSolutions_WithoutHeuristics_ReturnsExpectedResults(int?[,] puzzle, SolveStats expectedStats)
        {
            // Skip heuristics so the stats are easy to fully define.
            var ruleKeeper = new StandardRuleKeeper();
            var solver = new PuzzleSolver(ruleKeeper);
            Assert.Equal(expectedStats, solver.GetStatsForAllSolutions(puzzle));
        }

        [Theory]
        [MemberData(nameof(PuzzlesWithStats))]
        public void GetStatsForAllSolutions_WithHeuristics_ReturnsExpectedNumSolutions(int?[,] puzzle, SolveStats expectedStats)
        {
            var ruleKeeper = new StandardRuleKeeper();
            ISudokuRule rule = ruleKeeper.GetRules()[0];
            var heuristics = new StandardHeuristic(
                (IMissingRowValuesTracker)rule,
                (IMissingColumnValuesTracker)rule, (IMissingBoxValuesTracker)rule);
            var solver = new PuzzleSolver(ruleKeeper, heuristics);
            Assert.Equal(expectedStats.NumSolutionsFound, solver.GetStatsForAllSolutions(puzzle).NumSolutionsFound);
        }

        [Fact]
        public void GetStatsForAllSolutions_WithCanceledToken_Throws()
        {
            var solver = StandardPuzzles.CreateSolver();
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => solver.GetStatsForAllSolutions(new int?[9, 9], cancellationSource.Token));
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void HasUniqueSolution_WithUniqueSolution_ReturnsTrue(int?[,] puzzle)
        {
            var solver = StandardPuzzles.CreateSolver();
            Assert.True(solver.HasUniqueSolution(puzzle));
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void HasUniqueSolution_LeavesPuzzleUnchanged(int?[,] puzzle)
        {
            var puzzleCopy = (int?[,])puzzle.Clone();
            var solver = StandardPuzzles.CreateSolver();

            solver.HasUniqueSolution(puzzle);

            for (int row = 0; row < puzzleCopy.GetLength(0); ++row)
            {
                for (int col = 0; col < puzzleCopy.GetLength(1); ++col)
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
            Assert.Throws<OperationCanceledException>(() => solver.HasUniqueSolution(new int?[9, 9], cancellationSource.Token));
        }

        public static IEnumerable<object[]> ValidPuzzleGenerator()
        {
            yield return new object[]
            {
                new int?[,]
                {
                    {1, null, null, 2},
                    {null, null, 1, null},
                    {null, 1, null, null},
                    {3, null, 4, null}
                }
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {4, null, 2, null, null, 1, 8, 7, 6},
                    {3, null, 8, null, null, 5, null, 9, 4},
                    {6, null, 9, 4, null, 8, 3, null, 5},
                    {null, 3, 1, null, 6, null, null, null, null},
                    {2, 4, 5, 9, null, 7, 1, 6, 3},
                    {9, null, 7, 2, null, 3, 5, 4, 8},
                    {null, 9, null, 8, null, 2, null, null, null},
                    {1, 8, 3, null, 4, 9, 6, 5, 2},
                    {5, 2, 4, 1, 3, 6, 9, null, 7}
                }
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {null, 2, null, 6, null, 8, null, null, null},
                    {5, 8, null, null, null, 9, 7, null, null},
                    {null, null, null, null, 4, null, null, null, null},
                    {3, 7, null, null, null, null, 5, null, null},
                    {6, null, null, null, null, null, null, null, 4},
                    {null, null, 8, null, null, null, null, 1, 3},
                    {null, null, null, null, 2, null, null, null, null},
                    {null, null, 9, 8, null, null, null, 3, 6},
                    {null, null, null, 3, null, 6, null, 9, null},
                }
            };
            yield return new object[] {
                new int?[,]
                {
                    {   1, null, null, null,    2,    6, null, null, null},
                    {   7, null,    6, null, null,    5, null, null, null},
                    {null, null,    5,    8,    1, null, null, null, null},
                    {null,    5, null, null,    8, null,    1, null, null},
                    {null,    2, null, null, null, null, null,    8, null},
                    {null, null,    1, null,    6, null, null,    3, null},
                    {null, null, null, null,    5,    8,    4, null, null},
                    {null, null, null,    6, null, null,    3, null,    9},
                    {null, null, null,    2,    4, null, null, null,    5}
                }
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {null, null,    6, null,    1, null,    9, null, null},
                    {   7, null, null,    3, null, null, null,    6,    5},
                    {null, null, null, null,    7, null,    4, null,    8},
                    {   6, null, null, null, null,    1, null, null, null},
                    {null, null,    2, null, null, null,    5, null, null},
                    {null, null, null,    2, null, null, null, null,    9},
                    {   2, null,    8, null,    4, null, null, null, null},
                    {   1,    3, null, null, null,    7, null, null,    6},
                    {null, null,    4, null,    8, null,    1, null, null}
                }
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {null, null, null, 6, null, null, 4, null, null},
                    {7, null, null, null, null, 3, 6, null, null},
                    {null, null, null, null, 9, 1, null, 8, null},
                    {null, null, null, null, null, null, null, null, null},
                    {null, 5, null, 1, 8, null, null, null, 3},
                    {null, null, null, 3, null, 6, null, 4, 5},
                    {null, 4, null, 2, null, null, null, 6, null},
                    {9, null, 3, null, null, null, null, null, null},
                    {null, 2, null, null, null, null, 1, null, null}
                }
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {null, 2, null, null, null, null, null, null, null},
                    {null, null, null, 6, null, null, null, null, 3},
                    {null, 7, 4, null, 8, null, null, null, null},
                    {null, null, null, null, null, 3, null, null, 2},
                    {null, 8, null, null, 4, null, null, 1, null},
                    {6, null, null, 5, null, null, null, null, null},
                    {null, null, null, null, 1, null, 7, 8, null},
                    {5, null, null, null, null, 9, null, null, null},
                    {null, null, null, null, null, null, null, 4, null}
                }
            };
        }

        public static IEnumerable<object[]> PuzzlesWithStats()
        {
            yield return new object[]
            {
                new int?[,]
                {
                    {1, null, null, 2},
                    {null, null, 1, null},
                    {null, 1, null, null},
                    {3, null, 4, null}
                },
                new SolveStats() { NumSolutionsFound = 1 },
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {4, null, 2, null, null, 1, 8, 7, 6},
                    {3, null, 8, null, null, 5, null, 9, 4},
                    {6, null, 9, 4, null, 8, 3, null, 5},
                    {null, 3, 1, null, 6, null, null, null, null},
                    {2, 4, 5, 9, null, 7, 1, 6, 3},
                    {9, null, 7, 2, null, 3, 5, 4, 8},
                    {null, 9, null, 8, null, 2, null, null, null},
                    {1, 8, 3, null, 4, 9, 6, 5, 2},
                    {5, 2, 4, 1, 3, 6, 9, null, 7}
                },
                new SolveStats() { NumSolutionsFound = 1 },
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {null, 2, null, null, null, null, null, null, null},
                    {null, null, null, 6, null, null, null, null, 3},
                    {null, 7, 4, null, 8, null, null, null, null},
                    {null, null, null, null, null, 3, null, null, 2},
                    {null, 8, null, null, 4, null, null, 1, null},
                    {6, null, null, 5, null, null, null, null, null},
                    {null, null, null, null, 1, null, 7, 8, null},
                    {5, null, null, null, null, 9, null, null, null},
                    {null, null, null, null, null, null, null, 4, null}
                },
                new SolveStats() { NumSolutionsFound = 1, NumSquaresGuessed = 10, NumTotalGuesses = 22 },
            };
            yield return new object[]
            {
                new int?[,]
                {
                    {   1, null, null, null},
                    {null, null,    1, null},
                    {null,    1, null, null},
                    {   3, null,    4, null}
                },
                // Solutions:
                // +---+---+    +---+---+ +---+---+ +---+---+
                // |1 x|x x|    |1 4|3 2| |1 3|2 4| |1 4|2 3|
                // |x x|1 x|    |2 3|1 4| |2 4|1 3| |2 3|1 4|
                // +---+---+ => +---+---+ +---+---+ +---+---+
                // |x 1|x x|    |4 1|2 3| |4 1|3 2| |4 1|3 2|
                // |3 x|4 x|    |3 2|4 1| |3 2|4 1| |3 2|4 1|
                // +---+---+    +---+---+ +---+---+ +---+---+
                new SolveStats() { NumSolutionsFound = 3, NumSquaresGuessed = 2, NumTotalGuesses = 4 },
            };

        }

        private static void _AssertPuzzleSolved(int?[,] puzzle)
        {
            int size = puzzle.GetLength(0);
            var alreadyFound = new HashSet<int>(size);
            for (int row = 0; row < size; row++)
            {
                alreadyFound.Clear();
                for (int col = 0; col < size; col++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that row!");
                }
            }
            for (int col = 0; col < size; col++)
            {
                alreadyFound.Clear();
                for (int row = 0; row < size; row++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that col!");
                }
            }
            int boxSize = Boxes.CalculateBoxSize(size);
            for (int box = 0; box < size; box++)
            {
                alreadyFound.Clear();
                (int startRow, int startCol) = Boxes.GetStartingBoxCoordinate(box, boxSize);
                for (int row = startRow; row < startRow + boxSize; row++)
                {
                    for (int col = startCol; col < startCol + boxSize; col++)
                    {
                        Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that box!");
                    }
                }
            }
        }

        private static void _AssertMegaPuzzleSolved(int?[,] puzzle)
        {
            _AssertPuzzleSolved(puzzle);
            int size = puzzle.GetLength(0);
            var alreadyFound = new HashSet<int>(size);
            for (int row = 0, col = 0; row < size; row++, col++)
            {
                Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in the backward diagonal!");
            }
            alreadyFound.Clear();
            for (int row = 0, col = size - 1; row < size; row++, col--)
            {
                Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in the forward diagonal!");
            }
        }
    }
}