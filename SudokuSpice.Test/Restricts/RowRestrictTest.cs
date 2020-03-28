using SudokuSpice.Data;
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
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
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
                            {           1, /* INCORRECT */ 1, null /* 3 */,            2},
                            {null /* 2 */,      null /* 3 */,            1, null /* 4 */},
                            {null /* 4 */,      null /* 1 */, null /* 2 */, null /* 3 */},
                            {           3,                 2,            4,            1}
                        }));
            });
            Assert.Contains("Puzzle has duplicate value in row", ex.Message);
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var restrict = new RowRestrict(puzzle);

            var puzzleCopy = new Puzzle(puzzle);
            var restrictCopy = restrict.CopyWithNewReference(puzzleCopy);
            int val = 2;
            var coord = new Coordinate(2, 2);
            restrictCopy.Update(coord, val, new List<Coordinate>());
            Assert.NotEqual(restrict.GetPossibleValues(coord), restrictCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(2, 3);
            var secondVal = 3;
            var list = new List<Coordinate>();
            restrictCopy.Update(secondCoord, secondVal, list);
            var originalList = new List<Coordinate>();
            restrict.Update(secondCoord, secondVal, originalList);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1) },
                new HashSet<Coordinate>(list));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1), new Coordinate(2, 2) },
                new HashSet<Coordinate>(originalList));
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
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(1, 0), new Coordinate(1, 3) }, list);
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1111), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), restrict.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void Revert_WithoutAffectedList_RevertsSpecifiedRow()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            var initialPossibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, updatedList);

            restrict.Revert(coord, val);

            for (int row = 0; row < initialPossibleValuesByRow.Count; row++)
            {
                Assert.Equal(
                    initialPossibleValuesByRow[row], restrict.GetPossibleRowValues(row));
            }
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
            var initialPossibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var revertedList = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, updatedList);

            restrict.Revert(coord, val, revertedList);
            
            Assert.Equal(updatedList, revertedList);
            for (int row = 0; row < initialPossibleValuesByRow.Count; row++)
            {
                Assert.Equal(
                    initialPossibleValuesByRow[row],
                    restrict.GetPossibleRowValues(row));
            }
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

        [Fact]
        public void GetPossibleValues_MatchesGetPossibleRowValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {3, 2, 4, 1}
            });
            var restrict = new RowRestrict(puzzle);
            var possibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, restrict);

            for (int row = 0; row < possibleValuesByRow.Count; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        possibleValuesByRow[row],
                        restrict.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByRow(int numRows, RowRestrict restrict)
        {
            var possibleRowValues = new List<BitVector>();
            for (int row = 0; row < numRows; row++)
            {
                possibleRowValues.Add(restrict.GetPossibleRowValues(row));
            }
            return possibleRowValues;
        }
    }
}
