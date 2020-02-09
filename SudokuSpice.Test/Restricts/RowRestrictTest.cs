using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class RowRestrictTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1110), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var restrict = new RowRestrict(puzzle);
            Assert.NotNull(restrict);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInRow_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new RowRestrict(
                    new Puzzle(
                        new int?[,] {
                            {1, 1, null /* 3 */, 2},
                            {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                            {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                            {3, 2, 4, 1}
                        }));
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
        }

        [Fact]
        public void Update_UpdatesSpecifiedRow()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            puzzle[coord] = val;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> {new Coordinate(1, 0), new Coordinate(1, 3)}, list);
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedRow()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 0);
            var val = 1;
            restrict.Revert(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 1), new Coordinate(0, 2) }, list);
            Assert.Equal(new BitVector(0b1101), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1110), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void GetPossibleRowValues_IsCorrect()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleRowValues(0));
            Assert.Equal(new BitVector(0b1110), restrict.GetPossibleRowValues(1));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleRowValues(2));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleRowValues(3));
        }
    }
}
