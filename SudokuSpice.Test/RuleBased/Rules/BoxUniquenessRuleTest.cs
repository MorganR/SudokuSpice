using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class BoxUniquenessRuleTest
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b11010), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(25)]
        public void Constructor_AcceptsValidPuzzleSizes(int size)
        {
            var matrix = new int?[size, size];
            var puzzle = new Puzzle(matrix);
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);
            Assert.NotNull(rule);
        }

        [Fact]
        public void Constructor_WithDuplicateValueInBox_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var rule = new BoxUniquenessRule(
                    new Puzzle(
                        new int?[,] {
                            {           1,      null /* 4 */, null /* 3 */,            2},
                            {null /* 2 */, /* INCORRECT */ 1, null /* 1 */, null /* 4 */},
                            {null /* 4 */,      null /* 1 */,            2,            3},
                            {null /* 3 */,      null /* 2 */,            4,            1}
                        }),
                    BitVector.CreateWithSize(4),
                    false);
            });
            Assert.Contains("Puzzle has duplicate value in box", ex.Message);
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2,            3},
                {null /* 3 */, null /* 2 */,            4,            1}
            });
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);

            var puzzleCopy = new Puzzle(puzzle);
            var ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(3, 0);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(3, 1);
            var secondVal = 2;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1), new Coordinate(3, 0) },
                new HashSet<Coordinate>(originalCoordTracker.GetTrackedCoords().ToArray()));
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            var val = 1;
            rule.Update(coord, val, coordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 3) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(2, 2)));
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), true);
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 3);
            var val = 4;
            rule.Update(coord, val, coordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b01010), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b01010), rule.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(2, 2)));
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);
            var initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            var val = 1;
            rule.Update(in coord, val, updatedCoordTracker);

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.GetTrackedCoords().ToArray(),
                revertedCoordTracker.GetTrackedCoords().ToArray());
            for (int box = 0; box < initialPossibleValuesByBox.Count; box++)
            {
                Assert.Equal(
                    initialPossibleValuesByBox[box],
                    rule.GetMissingValuesForBox(box));
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), true);
            var initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);
            var coord = new Coordinate(1, 3);
            var val = 4;
            rule.Update(in coord, val, new CoordinateTracker(puzzle.Size));

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2) },
                new HashSet<Coordinate>(revertedCoordTracker.GetTrackedCoords().ToArray()));
            for (int box = 0; box < initialPossibleValuesByBox.Count; box++)
            {
                Assert.Equal(
                    initialPossibleValuesByBox[box],
                    rule.GetMissingValuesForBox(box));
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
            var rule = new BoxUniquenessRule(puzzle, _GetAllPossibleValues(puzzle.Size), false);
            var possibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    int box = puzzle.GetBoxIndex(row, column);
                    Assert.Equal(
                        possibleValuesByBox[box],
                        rule.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByBox(int numBoxes, BoxUniquenessRule rule)
        {
            var possibleBoxValues = new List<BitVector>();
            for (int box = 0; box < numBoxes; box++)
            {
                possibleBoxValues.Add(rule.GetMissingValuesForBox(box));
            }
            return possibleBoxValues;
        }

        private BitVector _GetAllPossibleValues(int size)
        {
            var possibleValues = BitVector.CreateWithSize(size + 1);
            possibleValues.UnsetBit(0);
            return possibleValues;
        }
    }
}
