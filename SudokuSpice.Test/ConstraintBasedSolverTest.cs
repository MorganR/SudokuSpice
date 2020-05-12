﻿using SudokuSpice.Constraints;
using SudokuSpice.Data;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    public class ConstraintBasedSolverTest
    {
        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void Solve_ValidPuzzle_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new ConstraintBasedSolver(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            solver.Solve(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void SolveRandomly_ValidPuzzle_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new ConstraintBasedSolver(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            solver.SolveRandomly(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(PuzzlesWithStats))]
        public void GetStatsForAllSolutions_ReturnsExpectedNumSolutions(Puzzle puzzle, SolveStats expectedStats)
        {
            var solver = new ConstraintBasedSolver(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Equal(expectedStats.NumSolutionsFound, solver.GetStatsForAllSolutions(puzzle).NumSolutionsFound);
        }

        public static IEnumerable<object[]> ValidPuzzleGenerator()
        {
            yield return new object[]
            {
                new Puzzle(new int?[,]
                {
                    {1, null, null, 2},
                    {null, null, 1, null},
                    {null, 1, null, null},
                    {3, null, 4, null}
                })
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                })
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                })
            };
            yield return new object[] {
                new Puzzle(new int?[,]
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
                })
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                })
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                })
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                })
            };
        }

        public static IEnumerable<object[]> PuzzlesWithStats()
        {
            yield return new object[]
            {
                new Puzzle(new int?[,]
                {
                    {1, null, null, 2},
                    {null, null, 1, null},
                    {null, 1, null, null},
                    {3, null, 4, null}
                }),
                new SolveStats() { NumSolutionsFound = 1 },
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                }),
                new SolveStats() { NumSolutionsFound = 1 },
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
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
                }),
                new SolveStats() { NumSolutionsFound = 1, NumSquaresGuessed = 10, NumTotalGuesses = 22 },
            };
            yield return new object[]
            {
                new Puzzle(new int?[,]
                {
                    {   1, null, null, null},
                    {null, null,    1, null},
                    {null,    1, null, null},
                    {   3, null,    4, null}
                }),
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

        private void _AssertPuzzleSolved(Puzzle puzzle)
        {
            Assert.Equal(0, puzzle.NumEmptySquares);
            var alreadyFound = new HashSet<int>(puzzle.Size);
            for (var row = 0; row < puzzle.Size; row++)
            {
                alreadyFound.Clear();
                for (var col = 0; col < puzzle.Size; col++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that row!");
                }
            }
            for (var col = 0; col < puzzle.Size; col++)
            {
                alreadyFound.Clear();
                for (var row = 0; row < puzzle.Size; row++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that col!");
                }
            }
            for (var box = 0; box < puzzle.Size; box++)
            {
                alreadyFound.Clear();
                (var startRow, var startCol) = puzzle.GetStartingBoxCoordinate(box);
                for (var row = startRow; row < startRow + puzzle.BoxSize; row++)
                {
                    for (var col = startCol; col < startCol + puzzle.BoxSize; col++)
                    {
                        Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that box!");
                    }
                }
            }
        }
    }
}
