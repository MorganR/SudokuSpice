using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class ColumnRestrictTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1,            null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */, 1,            null /* 2 */, 3},
                {3,            null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            Assert.Equal(0b1010, restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(0b1110, restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(0b1111, restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(0b0000, restrict.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var restrict = new ColumnRestrict(puzzle);
            Assert.NotNull(restrict);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInColumn_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new ColumnRestrict(
                    new Puzzle(
                        new int?[,] {
                            {1,            null /* 4 */, null /* 3 */, 2},
                            {1, null /* 3 */, null /* 1 */, 4},
                            {null /* 4 */, 1,            null /* 2 */, 3},
                            {3,            null /* 2 */, null /* 4 */, 1}
                        }));
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
        }

        [Fact]
        public void Update_UpdatesSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1,            null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */, 1,            null /* 2 */, 3},
                {3,            null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            puzzle[coord] = val;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> {new Coordinate(0, 1), new Coordinate(3, 1)}, list);
            Assert.Equal(0b1010, restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(0b1010, restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(0b1111, restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(0b0000, restrict.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Fact]
        public void Update_OnUnsetCoord_Throws()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1,            null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */, 1,            null /* 2 */, 3},
                {3,            null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                restrict.Update(new Coordinate(1, 1), 3, new List<Coordinate>());
            });
            Assert.Contains("Cannot update a restrict for an unset puzzle coordinate", ex.Message);
        }

        [Fact]
        public void Revert_OnUnsetCoord_Throws()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1,            null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */, 1,            null /* 2 */, 3},
                {3,            null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                restrict.Revert(new Coordinate(1, 1), 3, new List<Coordinate>());
            });
            Assert.Contains("Cannot revert a restrict for an unset puzzle coordinate", ex.Message);
        }

        [Fact]
        public void Revert_RevertsSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1,            null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */, 1,            null /* 2 */, 3},
                {3,            null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 0);
            var val = 1;
            restrict.Revert(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(1, 0), new Coordinate(2, 0) }, list);
            Assert.Equal(0b1011, restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(0b1110, restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(0b1111, restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(0b0000, restrict.GetPossibleValues(new Coordinate(0, 3)));
        }
    }
}
