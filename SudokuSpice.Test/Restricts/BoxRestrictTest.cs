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
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2,            3},
                {null /* 3 */, null /* 2 */,            4,            1}
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
                            {           1,      null /* 4 */, null /* 3 */,            2},
                            {null /* 2 */, /* INCORRECT */ 1, null /* 1 */, null /* 4 */},
                            {null /* 4 */,      null /* 1 */,            2,            3},
                            {null /* 3 */,      null /* 2 */,            4,            1}
                        }), false);
            });
            Assert.Contains("Puzzle has duplicate value in box", ex.Message);
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
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 3) }, list);
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
            var initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(1, 2);
            var val = 1;
            restrict.Update(in coord, val, updatedList);

            var revertedList = new List<Coordinate>();
            restrict.Revert(coord, val, revertedList);

            Assert.Equal(updatedList, revertedList);
            for (int box = 0; box < initialPossibleValuesByBox.Count; box++)
            {
                Assert.Equal(
                    initialPossibleValuesByBox[box],
                    restrict.GetPossibleBoxValues(box));
            }
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
            var initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, restrict);
            var coord = new Coordinate(1, 3);
            var val = 4;
            restrict.Update(in coord, val, new List<Coordinate>());

            var revertedList = new List<Coordinate>();
            restrict.Revert(coord, val, revertedList);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 2) }, revertedList);
            for (int box = 0; box < initialPossibleValuesByBox.Count; box++)
            {
                Assert.Equal(
                    initialPossibleValuesByBox[box],
                    restrict.GetPossibleBoxValues(box));
            }
        }


        [Fact]
        public void GetPossibleValues_MatchesGetPossibleBoxValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var restrict = new BoxRestrict(puzzle, false);
            var possibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, restrict);

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    int box = puzzle.GetBoxIndex(row, column);
                    Assert.Equal(
                        possibleValuesByBox[box],
                        restrict.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByBox(int numBoxes, BoxRestrict restrict)
        {
            var possibleBoxValues = new List<BitVector>();
            for (int box = 0; box < numBoxes; box++)
            {
                possibleBoxValues.Add(restrict.GetPossibleBoxValues(box));
            }
            return possibleBoxValues;
        }
    }
}
