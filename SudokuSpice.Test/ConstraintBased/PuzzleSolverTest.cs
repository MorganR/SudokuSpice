﻿using SudokuSpice.ConstraintBased.Constraints;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class PuzzleSolverTest
    {
        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void Solve_ValidPuzzle_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            solver.Solve(puzzle);
            _AssertPuzzleSolved(puzzle);
        }

        [Fact]
        public void Solve_WithMegaPuzzle_Solves()
        {
            var puzzle = new Puzzle(new int?[,] {
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
            });
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint(), new DiagonalUniquenessConstraint() });

            solver.Solve(puzzle);

            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void Solve_ValidPuzzleWithRandomGuesses_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            solver.Solve(puzzle, randomizeGuesses: true);
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void Solve_InvalidPuzzle_Throws(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle));
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void Solve_InvalidPuzzleWithRandomGuesses_Throws(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle, randomizeGuesses: true));
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void TrySolve_ValidPuzzle_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.True(solver.TrySolve(puzzle));
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [MemberData(nameof(ValidPuzzleGenerator))]
        public void TrySolve_ValidPuzzleWithRandomGuesses_SolvesPuzzle(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.True(solver.TrySolve(puzzle, randomizeGuesses: true));
            _AssertPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolve_InvalidPuzzle_ReturnsFalse(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.False(solver.TrySolve(puzzle));
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolve_InvalidPuzzleWithRandomGuesses_ReturnsFalse(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.False(solver.TrySolve(puzzle, randomizeGuesses: true));
        }

        [Theory]
        [MemberData(nameof(PuzzlesWithStats))]
        public void GetStatsForAllSolutions_ReturnsExpectedNumSolutions(Puzzle puzzle, SolveStats expectedStats)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Equal(expectedStats.NumSolutionsFound, solver.GetStatsForAllSolutions(puzzle).NumSolutionsFound);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void GetStatsForAllSolutions_WithInvalidPuzzles_ReturnsNoSolutions(Puzzle puzzle)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Equal(0, solver.GetStatsForAllSolutions(puzzle).NumSolutionsFound);
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
            for (int row = 0; row < puzzle.Size; row++)
            {
                alreadyFound.Clear();
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that row!");
                }
            }
            for (int col = 0; col < puzzle.Size; col++)
            {
                alreadyFound.Clear();
                for (int row = 0; row < puzzle.Size; row++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that col!");
                }
            }
            for (int box = 0; box < puzzle.Size; box++)
            {
                alreadyFound.Clear();
                (int startRow, int startCol) = puzzle.GetStartingBoxCoordinate(box);
                for (int row = startRow; row < startRow + puzzle.BoxSize; row++)
                {
                    for (int col = startCol; col < startCol + puzzle.BoxSize; col++)
                    {
                        Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that box!");
                    }
                }
            }
        }
    }
}