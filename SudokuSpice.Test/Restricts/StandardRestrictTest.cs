using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class StandardRestrictTest
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
            var restrict = new StandardRestrict(puzzle);
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b0100), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b1000), restrict.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b1001), restrict.GetPossibleValues(new Coordinate(2, 1)));
            Assert.Equal(new BitVector(0b0110), restrict.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(new BitVector(0b0100), restrict.GetPossibleValues(new Coordinate(2, 3)));
        }

        [Fact]
        public void Constructor_WithDuplicateValueInRow_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new StandardRestrict(
                    new Puzzle(new int?[,] {
                        {           1, null /* 4 */, null /* 3 */,            2},
                        {null /* 2 */,            3,            3, null /* 4 */},
                        {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        {           3,            2,            4,            1}
                    }));
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInColumn_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new StandardRestrict(
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
        public void Constructor_WithDuplicateValueInBox_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var restrict = new StandardRestrict(
                    new Puzzle(
                        new int?[,] {
                            {           1,      null /* 4 */, null /* 3 */, 2},
                            {null /* 2 */, 1 /* INCORRECT */, null /* 1 */, null /* 4 */},
                            {null /* 4 */,      null /* 1 */,            2, 3},
                            {null /* 3 */,      null /* 2 */,            4, 1}
                        }));
            });
            Assert.Contains("Puzzle does not satisfy restrict", ex.Message);
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
            var restrict = new StandardRestrict(puzzle);

            var puzzleCopy = new Puzzle(puzzle);
            var restrictCopy = restrict.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(1, 1);
            restrictCopy.Update(coord, val, new List<Coordinate>());
            Assert.NotEqual(restrict.GetPossibleValues(coord), restrictCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(0, 1);
            var secondVal = 4;
            var list = new List<Coordinate>();
            restrictCopy.Update(secondCoord, secondVal, list);
            var originalList = new List<Coordinate>();
            restrict.Update(secondCoord, secondVal, originalList);
            Assert.Equal(new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(2, 1), new Coordinate(1, 0) }, new HashSet<Coordinate>(list));
            Assert.Equal(new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 1), new Coordinate(2, 1), new Coordinate(1, 0) }, new HashSet<Coordinate>(originalList));
        }

        [Fact]
        public void Update_UpdatesSpecifiedCoordinate()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var restrict = new StandardRestrict(puzzle);
            var list = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, list);
            Assert.Equal(
                new List<Coordinate> {
                    new Coordinate(1, 0), new Coordinate(1, 3), new Coordinate(0, 1), new Coordinate(2, 1)
                },
                list);
            Assert.Equal(new BitVector(0b1000), restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1000), restrict.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b1001), restrict.GetPossibleValues(new Coordinate(2, 1)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedCoordinate()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var restrict = new StandardRestrict(puzzle);
            var initialPossibleValues = _GetPossibleValues(puzzle.Size, restrict);
            var updatedList = new List<Coordinate>();
            var coord = new Coordinate(1, 1);
            var val = 3;
            restrict.Update(coord, val, updatedList);

            var restrictedList = new List<Coordinate>();
            restrict.Revert(coord, val, restrictedList);

            Assert.Equal(updatedList, restrictedList);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    Assert.Equal(
                        initialPossibleValues[row, column],
                        restrict.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private BitVector[,] _GetPossibleValues(int puzzleSize, StandardRestrict restrict)
        {
            var possibleValues = new BitVector[puzzleSize, puzzleSize];
            for (int row = 0; row < puzzleSize; row++)
            {
                for (int column = 0; column < puzzleSize; column++)
                {
                    possibleValues[row, column] = restrict.GetPossibleValues(new Coordinate(row, column));
                }
            }
            return possibleValues;
        }
    }
}
