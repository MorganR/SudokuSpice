using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Rules.Test
{
    public class DiagonalUniquenessRuleTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            var expectedBackwardPossibles = new BitVector(0b1110);
            var expectedForwardPossibles = new BitVector(0b0011);
            var expectedOtherPossibles = new BitVector(0b1111);
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

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var rule = new DiagonalUniquenessRule(puzzle);
            Assert.NotNull(rule);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInDiagonal_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var puzzle = new Puzzle(new int?[,] {
                    {   1, null, null,   4},
                    {null,    1,    3,   2},
                    {   2, null, null, null},
                    {null, null, null, null}
                });
                var rule = new DiagonalUniquenessRule(puzzle);
            });
            Assert.Contains("Puzzle does not satisfy diagonal uniqueness rule", ex.Message);
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,    4},
                {null, null,    3,    2},
                {   4, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);

            var puzzleCopy = new Puzzle(puzzle);
            var ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 4;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new List<Coordinate>());
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(2, 2);
            var secondVal = 2;
            var list = new List<Coordinate>();
            ruleCopy.Update(secondCoord, secondVal, list);
            var originalList = new List<Coordinate>();
            rule.Update(secondCoord, secondVal, originalList);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(3, 3) },
                new HashSet<Coordinate>(list));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 3) },
                new HashSet<Coordinate>(originalList));
        }

        [Fact]
        public void Update_OnDiagonal_UpdatesSpecifiedDiagonal()
        {
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(2, 2);
            var val = 4;
            rule.Update(coord, val, list);
            Assert.Equal(
                new List<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 3) }, list);
            Assert.Equal(new BitVector(0b0110), rule.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b0011), rule.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b1111), rule.GetPossibleValues(new Coordinate(0, 2)));
        }

        [Fact]
        public void Update_OnNonDiagonal_DoesNothing()
        {
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, rule);

            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 1);
            var val = 2;
            rule.Update(coord, val, list);

            Assert.Empty(list);
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
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, rule);
            var coord = new Coordinate(2, 2);
            var val = 4;
            rule.Update(in coord, val, new List<Coordinate>());

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
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, rule);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(2, 2);
            var val = 4;
            rule.Update(in coord, val, updatedList);

            var revertedList = new List<Coordinate>();
            rule.Revert(coord, val, revertedList);

            Assert.Equal(revertedList, updatedList);
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
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,   4},
                {null, null,    3,   2},
                {   2, null, null, null},
                {null, null, null, null}
            });
            var rule = new DiagonalUniquenessRule(puzzle);
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, rule);

            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 1);
            var val = 2;
            rule.Update(in coord, val, new List<Coordinate>());

            rule.Revert(coord, val, list);

            Assert.Empty(list);
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
            BitVector[,] possibleValues = new BitVector[puzzleSize, puzzleSize];
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
