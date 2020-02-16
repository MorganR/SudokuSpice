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
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1110), restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(0, 3)));
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
                            {                1, null /* 4 */, null /* 3 */, 2},
                            {/* INCORRECT */ 1, null /* 3 */, null /* 1 */, 4},
                            {     null /* 4 */,            1, null /* 2 */, 3},
                            {                3, null /* 2 */, null /* 4 */, 1}
                        }));
            });
            Assert.Contains("Puzzle has duplicate value in column", ex.Message);
        }

        [Fact]
        public void Update_UpdatesSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 1), new Coordinate(3, 1) }, list);
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Fact]
        public void Revert_WithoutAffectedCoordsList_RevertsSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, restrict);
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, new List<Coordinate>());

            restrict.Revert(coord, val);

            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column],
                    restrict.GetPossibleColumnValues(column));
            }
        }

        [Fact]
        public void Revert_RevertsSpecifiedColumn()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, updatedList);

            var revertedList = new List<Coordinate>();
            restrict.Revert(coord, val, revertedList);

            Assert.Equal(updatedList, revertedList);
            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column],
                    restrict.GetPossibleColumnValues(column));
            }
        }

        [Fact]
        public void GetPossibleValues_MatchesGetPossibleColumnValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new ColumnRestrict(puzzle);
            var possibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, restrict);

            for (int column = 0; column < possibleValuesByColumn.Count; column++)
            {
                for (int row = 0; row < puzzle.Size; row++)
                {
                    Assert.Equal(
                        possibleValuesByColumn[column],
                        restrict.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByColumn(int numColumns, ColumnRestrict restrict)
        {
            var possibleColumnValues = new List<BitVector>();
            for (int column = 0; column < numColumns; column++)
            {
                possibleColumnValues.Add(restrict.GetPossibleColumnValues(column));
            }
            return possibleColumnValues;
        }
    }
}
