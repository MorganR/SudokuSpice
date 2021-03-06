using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class DiagonalUniquenessRuleTest
    {
        [Fact]
        public void TryInitFor_ValidPuzzle_FiltersCorrectly()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();

            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            var expectedBackwardPossibles = new BitVector(0b11100);
            var expectedForwardPossibles = new BitVector(0b00110);
            var expectedOtherPossibles = new BitVector(0b11110);
            Assert.Equal(expectedBackwardPossibles,
                rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(expectedBackwardPossibles,
                rule.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(expectedBackwardPossibles,
                rule.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(expectedBackwardPossibles,
                rule.GetPossibleValues(new Coordinate(3, 3)));
            Assert.Equal(expectedForwardPossibles,
                rule.GetPossibleValues(new Coordinate(0, 3)));
            Assert.Equal(expectedForwardPossibles,
                rule.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(expectedForwardPossibles,
                rule.GetPossibleValues(new Coordinate(2, 1)));
            Assert.Equal(expectedForwardPossibles,
                rule.GetPossibleValues(new Coordinate(3, 0)));
            Assert.Equal(expectedOtherPossibles,
                rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(expectedOtherPossibles,
                rule.GetPossibleValues(new Coordinate(1, 3)));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInDiagonal_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                   new int?[] {   1, null, null,   4},
                   new int?[] {null,    1,    3,   2},
                   new int?[] {   2, null, null, null},
                   new int?[] {null, null, null, null}
               });
            var rule = new DiagonalUniquenessRule();

            Assert.False(rule.TryInit(puzzle, puzzle.AllPossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,    4},
                new int?[] {null, null,    3,    2},
                new int?[] {   4, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 4;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(2, 2);
            int secondVal = 2;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(3, 3) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 3) },
                new HashSet<Coordinate>(originalCoordTracker.GetTrackedCoords().ToArray()));
        }

        [Fact]
        public void Update_OnDiagonal_UpdatesSpecifiedDiagonal()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(2, 2);
            int val = 4;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 3) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(new BitVector(0b01100), rule.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b00110), rule.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(0, 2)));
        }

        [Fact]
        public void Update_OnNonDiagonal_DoesNothing()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, rule);
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(0, 1);
            int val = 2;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(0, coordTracker.NumTracked);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        previousPossibles[row, col],
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        [Fact]
        public void Revert_WithoutAffectedCoordsList_RevertsSpecifiedDiagonal()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, rule);
            var coord = new Coordinate(2, 1);
            int val = 1;
            rule.Update(in coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val);

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        initialPossibles[row, col],
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        [Fact]
        public void Revert_RevertsSpecifiedDiagonal()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(2, 2);
            int val = 4;
            rule.Update(in coord, val, updatedCoordTracker);

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                revertedCoordTracker.GetTrackedCoords().ToArray(),
                updatedCoordTracker.GetTrackedCoords().ToArray());
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        initialPossibles[row, col],
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        [Fact]
        public void Revert_OnNonDiagonal_DoesNothing()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {   1, null, null,   4},
                new int?[] {null, null,    3,   2},
                new int?[] {   2, null, null, null},
                new int?[] {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, rule);
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(0, 1);
            int val = 2;
            rule.Update(in coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val, coordTracker);

            Assert.Equal(0, coordTracker.NumTracked);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        previousPossibles[row, col],
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private BitVector[,] _GetPossibleValues(int puzzleSize, DiagonalUniquenessRule rule)
        {
            var possibleValues = new BitVector[puzzleSize, puzzleSize];
            for (int row = 0; row < puzzleSize; row++)
            {
                for (int col = 0; col < puzzleSize; col++)
                {
                    possibleValues[row, col] =
                        rule.GetPossibleValues(new Coordinate(row, col));
                }
            }
            return possibleValues;
        }
    }
}