using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class BoxRestrictTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2, 3},
                {null /* 3 */, null /* 2 */,            4, 1}
            });
            var restrict = new BoxRestrict(puzzle, false);
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1101), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var restrict = new BoxRestrict(puzzle, false);
            Assert.NotNull(restrict);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInBox_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new BoxRestrict(
                    new Puzzle(
                        new int?[,] {
                            {           1,      null /* 4 */, null /* 3 */, 2},
                            {null /* 2 */, 1 /* INCORRECT */, null /* 1 */, null /* 4 */},
                            {null /* 4 */,      null /* 1 */,            2, 3},
                            {null /* 3 */,      null /* 2 */,            4, 1}
                        }), false);
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
        }

        [Fact]
        public void Update_UpdatesSpecifiedBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2, 3},
                {null /* 3 */, null /* 2 */,            4, 1}
            });
            var restrict = new BoxRestrict(puzzle, false);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 2);
            var val = 1;
            puzzle[coord] = val;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> {new Coordinate(0, 2), new Coordinate(1, 3)}, list);
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void Update_SkippingRowsAndCols_AppendsOnlyOthersInBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2,            3},
                {null /* 3 */, null /* 2 */,            4,            1}
            });
            var restrict = new BoxRestrict(puzzle, true);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 3);
            var val = 4;
            puzzle[coord] = val;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 2) }, list);
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0101), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b0101), restrict.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2, 3},
                {null /* 3 */, null /* 2 */,            4, 1}
            });
            var restrict = new BoxRestrict(puzzle, false);
            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 0);
            var val = 1;
            restrict.Revert(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 1), new Coordinate(1, 0) }, list);
            Assert.Equal(new BitVector(0b1011), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1101), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void Revert_SkippingRowsAndCols_AppendsOnlyOthersInBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1,            4, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2, 3},
                {null /* 3 */, null /* 2 */,            4, 1}
            });
            var restrict = new BoxRestrict(puzzle, true);
            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 0);
            var val = 1;
            restrict.Revert(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(1, 1) }, list);
            Assert.Equal(new BitVector(0b0111), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0111), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b0111), restrict.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b1101), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(2, 2)));
        }
    }
}
