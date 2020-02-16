using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class DiagonalRestrictTest
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
            var restrict = new DiagonalRestrict(puzzle);
            var expectedBackwardPossibles = new BitVector(0b1110);
            var expectedForwardPossibles = new BitVector(0b0011);
            var expectedOtherPossibles = new BitVector(0b1111);
            Assert.Equal(expectedBackwardPossibles,
                restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(expectedBackwardPossibles,
                restrict.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(expectedBackwardPossibles,
                restrict.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(expectedBackwardPossibles,
                restrict.GetPossibleValues(new Coordinate(3, 3)));
            Assert.Equal(expectedForwardPossibles,
                restrict.GetPossibleValues(new Coordinate(0, 3)));
            Assert.Equal(expectedForwardPossibles,
                restrict.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(expectedForwardPossibles,
                restrict.GetPossibleValues(new Coordinate(2, 1)));
            Assert.Equal(expectedForwardPossibles,
                restrict.GetPossibleValues(new Coordinate(3, 0)));
            Assert.Equal(expectedOtherPossibles,
                restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(expectedOtherPossibles,
                restrict.GetPossibleValues(new Coordinate(1, 3)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var restrict = new DiagonalRestrict(puzzle);
            Assert.NotNull(restrict);
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
                var restrict = new DiagonalRestrict(puzzle);
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
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
            var restrict = new DiagonalRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(2, 2);
            var val = 4;
            restrict.Update(coord, val, list);
            Assert.Equal(
                new List<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 3) }, list);
            Assert.Equal(new BitVector(0b0110), restrict.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b0011), restrict.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(0, 2)));
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
            var restrict = new DiagonalRestrict(puzzle);
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, restrict);

            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 1);
            var val = 2;
            restrict.Update(coord, val, list);

            Assert.Empty(list);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        previousPossibles[row, col],
                        restrict.GetPossibleValues(new Coordinate(row, col)));
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
            var restrict = new DiagonalRestrict(puzzle);
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, restrict);
            var coord = new Coordinate(2, 2);
            var val = 4;
            restrict.Update(in coord, val, new List<Coordinate>());

            restrict.Revert(coord, val);

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        initialPossibles[row, col],
                        restrict.GetPossibleValues(new Coordinate(row, col)));
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
            var restrict = new DiagonalRestrict(puzzle);
            BitVector[,] initialPossibles = _GetPossibleValues(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(2, 2);
            var val = 4;
            restrict.Update(in coord, val, updatedList);

            var revertedList = new List<Coordinate>();
            restrict.Revert(coord, val, revertedList);

            Assert.Equal(revertedList, updatedList);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        initialPossibles[row, col],
                        restrict.GetPossibleValues(new Coordinate(row, col)));
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
            var restrict = new DiagonalRestrict(puzzle);
            BitVector[,] previousPossibles = _GetPossibleValues(puzzle.Size, restrict);

            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 1);
            var val = 2;
            restrict.Update(in coord, val, new List<Coordinate>());

            restrict.Revert(coord, val, list);

            Assert.Empty(list);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        previousPossibles[row, col],
                        restrict.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private BitVector[,] _GetPossibleValues(int puzzleSize, DiagonalRestrict restrict)
        {
            BitVector[,] possibleValues = new BitVector[puzzleSize, puzzleSize];
            for (int row = 0; row < puzzleSize; row++)
            {
                for (int col = 0; col < puzzleSize; col++)
                {
                    possibleValues[row, col] =
                        restrict.GetPossibleValues(new Coordinate(row, col));
                }
            }
            return possibleValues;
        }
    }
}
