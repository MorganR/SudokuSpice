﻿using Xunit;

namespace SudokuSpice
{
    public class PossibleValuesTest
    {
        [Fact]
        public void Constructor_SetsAllUnsetToAllPossible()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);

            var allPossibleValues = new BitVector(0b1111);
            var noPossibleValues = new BitVector(0);

            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(0, 0)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(0, 1)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(0, 2)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(0, 3)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(1, 1)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(1, 2)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(1, 3)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(2, 1)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(2, 2)]);
            Assert.Equal(allPossibleValues, possibleValues[new Coordinate(2, 3)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(3, 0)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(3, 1)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(3, 2)]);
            Assert.Equal(noPossibleValues, possibleValues[new Coordinate(3, 3)]);
        }

        [Fact]
        public void Intersect_ModifiedCurrentValue()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);

            var coord = new Coordinate(1, 1);
            possibleValues.Intersect(coord, new BitVector(0b1010));
            Assert.Equal(new BitVector(0b1010), possibleValues[coord]);

            possibleValues.Intersect(coord, new BitVector(0b1100));
            Assert.Equal(new BitVector(0b1000), possibleValues[coord]);
        }

        [Fact]
        public void SettingByIndex_SetsValue()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);

            var coord = new Coordinate(1, 1);
            possibleValues[coord] = new BitVector(0b1010);

            Assert.Equal(new BitVector(0b1010), possibleValues[coord]);
        }

        [Fact]
        public void Reset_RevertsToAllPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);

            var coord = new Coordinate(1, 1);
            possibleValues[coord] = new BitVector(0b1010);

            possibleValues.Reset(coord);
            Assert.Equal(new BitVector(0b1111), possibleValues[coord]);
        }
    }
}
