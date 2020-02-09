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
            puzzle[coord] = val;
            restrict.Update(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(1, 0), new Coordinate(1, 3), new Coordinate(0, 1), new Coordinate(2, 1) }, list);
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
            var list = new List<Coordinate>();
            var coord = new Coordinate(0, 0);
            var val = 1;
            restrict.Revert(coord, val, list);
            Assert.Equal(new List<Coordinate> { new Coordinate(0, 1), new Coordinate(0, 2), new Coordinate(1, 0), new Coordinate(2, 0), new Coordinate(1, 1) }, list);
            Assert.Equal(new BitVector(0b1001), restrict.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1101), restrict.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b0100), restrict.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1010), restrict.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1100), restrict.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b1001), restrict.GetPossibleValues(new Coordinate(2, 0)));
        }
    }
}
