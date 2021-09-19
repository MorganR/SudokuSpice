using SudokuSpice.Test;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class MagicSquaresRuleTest
    {
        [Fact]
        public void TryInit_FiltersCorrectly()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var rule = new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false);

            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            Assert.Equal(new BitVector(0b0000110110), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0000110000), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1111111110), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b0110110000), rule.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b0000000010), rule.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b0010000000), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b1111111110), rule.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void TryInit_WithDiagonals_FiltersCorrectly()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null,    6, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null,    3,    8, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var rule = new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: true);

            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            Assert.Equal(new BitVector(0b0000110110), rule.GetPossibleValues(new Coordinate(3, 3)));
            Assert.Equal(new BitVector(0b0110110000), rule.GetPossibleValues(new Coordinate(3, 4)));
            Assert.Equal(new BitVector(0b1111111110), rule.GetPossibleValues(new Coordinate(4, 3)));
            Assert.Equal(new BitVector(0b0000110000), rule.GetPossibleValues(new Coordinate(4, 4)));
            Assert.Equal(new BitVector(0b0000000010), rule.GetPossibleValues(new Coordinate(4, 5)));
            Assert.Equal(new BitVector(0b0000010000), rule.GetPossibleValues(new Coordinate(5, 3)));
            Assert.Equal(new BitVector(0b1111111110), rule.GetPossibleValues(new Coordinate(6, 3)));
        }

        [Fact]
        public void TryInit_NotPossible_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    6, null, null, null, null, null, null },
                new int?[] {    1, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    8, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var rule = new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false);

            Assert.False(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var rule = new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false);
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);

            int val = 4;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(1, 2);
            int secondVal = 1;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 0), },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 0), new Coordinate(1, 1) },
                new HashSet<Coordinate>(originalCoordTracker.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedRow()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var rule = new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false);
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            int val = 4;
            var coord = new Coordinate(0, 1);
            var affectedCoordinates = new CoordinateTracker(puzzle.Size);
            rule.Update(coord, val, affectedCoordinates);
            Assert.Equal(
                new HashSet<Coordinate>() { new Coordinate(0, 0), new Coordinate(1, 1) },
                new HashSet<Coordinate>(affectedCoordinates.TrackedCoords.ToArray()));
            Assert.Equal(new BitVector(0b0000000100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0100000000), rule.GetPossibleValues(new Coordinate(1, 1)));
        }

        [Fact]
        public void Revert_WithoutAffectedList_RevertsSpecifiedRow()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            BitVector[] initialPossibleValues = new BitVector[] {
                rule.GetPossibleValues(new Coordinate(0, 0)),
                rule.GetPossibleValues(new Coordinate(1, 1)),
            };
            int val = 4;
            var coord = new Coordinate(0, 1);
            rule.Update(coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val);

            Assert.Equal(
                initialPossibleValues[0],
                rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(
                initialPossibleValues[1],
                rule.GetPossibleValues(new Coordinate(1, 1)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedRow()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            BitVector[] initialPossibleValues = new BitVector[] {
                rule.GetPossibleValues(new Coordinate(0, 0)),
                rule.GetPossibleValues(new Coordinate(1, 1)),
            };
            int val = 4;
            var coord = new Coordinate(0, 1);
            var updatedCoordinates = new CoordinateTracker(puzzle.Size);
            rule.Update(coord, val, updatedCoordinates);

            var revertedCoordinates = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordinates);

            Assert.Equal(
                revertedCoordinates.TrackedCoords.ToArray(),
                updatedCoordinates.TrackedCoords.ToArray());
            Assert.Equal(
                initialPossibleValues[0],
                rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(
                initialPossibleValues[1],
                rule.GetPossibleValues(new Coordinate(1, 1)));
        }
    }

    public class MagicSquaresRuleSolverTests
    {
        [Fact]
        public void Solve_WithManySolutions_Works()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false),
                    }));

            var solution = solver.Solve(puzzle);

            Assert.Equal(0, solution.NumEmptySquares);
            MagicSquareTests.AssertMagicSquaresSatisfied(
                solution, magicSquares, expectedSum: 15, verifyDiagonals: false);
        }

        [Fact]
        public void Solve_WithOneSolution_Works()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    1, null },
                new int?[] { null, null, null, null, null,    5, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null,    8 },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] {    7, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null,    3, null, null, null, null, null },
                new int?[] { null,    4, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null,    6, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(0, 3), 3),
                new Box(new Coordinate(0, 6), 3),
                new Box(new Coordinate(3, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(3, 6), 3),
                new Box(new Coordinate(6, 0), 3),
                new Box(new Coordinate(6, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new ColumnUniquenessRule(),
                        new BoxUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false),
                    }));

            var solution = solver.Solve(puzzle);

            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            MagicSquareTests.AssertMagicSquaresSatisfied(
                solution, magicSquares, expectedSum: 15, verifyDiagonals: false);
        }

        [Fact]
        public void Solve_WithOneSolutionUsingDiagonals_Works()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null, null, null, null, null, null,    1,    2 },
                new int?[] { null,    5, null, null,    9, null, null, null, null },
                new int?[] { null, null, null, null, null,    8, null, null, null },
                new int?[] { null,    3, null, null, null, null,    5, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    9, null },
                new int?[] { null, null, null, null,    4,    3,    9, null, null },
                new int?[] {    6, null,    1, null, null, null,    7, null, null },
                new int?[] {    2, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new ColumnUniquenessRule(),
                        new BoxUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: true),
                    }));

            var solution = solver.Solve(puzzle);

            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            MagicSquareTests.AssertMagicSquaresSatisfied(
                solution, magicSquares, expectedSum: 15, verifyDiagonals: false);
        }
    }

    public class MagicSquaresRuleUniqueSolutionTests
    {
        [Fact]
        public void HasUniqueSolution_WithManySolutions_IsFalse()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false),
                    }));

            Assert.False(solver.HasUniqueSolution(puzzle));
        }

        [Fact]
        public void HasUniqueSolution_WithUniqueSolution_IsTrue()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    1, null },
                new int?[] { null, null, null, null, null,    5, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null,    8 },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] {    7, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null,    3, null, null, null, null, null },
                new int?[] { null,    4, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null,    6, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(0, 3), 3),
                new Box(new Coordinate(0, 6), 3),
                new Box(new Coordinate(3, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(3, 6), 3),
                new Box(new Coordinate(6, 0), 3),
                new Box(new Coordinate(6, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new ColumnUniquenessRule(),
                        new BoxUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: false),
                    }));

            Assert.True(solver.HasUniqueSolution(puzzle));
        }

        [Fact]
        public void HasUniqueSolution_WithOneSolutionUsingDiagonals_IsTrue()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] { null, null, null, null, null, null, null,    1,    2 },
                new int?[] { null,    5, null, null,    9, null, null, null, null },
                new int?[] { null, null, null, null, null,    8, null, null, null },
                new int?[] { null,    3, null, null, null, null,    5, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    9, null },
                new int?[] { null, null, null, null,    4,    3,    9, null, null },
                new int?[] {    6, null,    1, null, null, null,    7, null, null },
                new int?[] {    2, null, null, null, null, null, null, null, null },
            });
            var magicSquares = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var solver = new PuzzleSolver<PuzzleWithPossibleValues>(
                new DynamicRuleKeeper(
                    new IRule[] {
                        new RowUniquenessRule(),
                        new ColumnUniquenessRule(),
                        new BoxUniquenessRule(),
                        new MagicSquaresRule(puzzle.Size, magicSquares, includeDiagonals: true),
                    }));

            Assert.True(solver.HasUniqueSolution(puzzle));
        }
    }
}