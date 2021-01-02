﻿using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class BoxUniquenessRuleTest
    {
        [Fact]
        public void TryInitFor_ValidPuzzle_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2,            3},
                {null /* 3 */, null /* 2 */,            4,            1}
            });
            var rule = new BoxUniquenessRule();

            Assert.True(rule.TryInit(puzzle));

            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b11010), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInBox_Fails()
        {
            var puzzle = new Puzzle(
                    new int?[,] {
                            {           1,      null /* 4 */, null /* 3 */,            2},
                            {null /* 2 */, /* INCORRECT */ 1, null /* 1 */, null /* 4 */},
                            {null /* 4 */,      null /* 1 */,            2,            3},
                            {null /* 3 */,      null /* 2 */,            4,            1}
                    });
            var rule = new BoxUniquenessRule();

            Assert.False(rule.TryInit(puzzle));
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
            var rule = new BoxUniquenessRule();
            Assert.True(rule.TryInit(puzzle));

            var puzzleCopy = new Puzzle(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(3, 0);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(3, 1);
            int secondVal = 2;
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
            var rule = new BoxUniquenessRule();
            Assert.True(rule.TryInit(puzzle));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            int val = 1;

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
        public void Revert_RevertsSpecifiedBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */,            3, null /* 1 */, null /* 4 */},
                {null /* 4 */, null /* 1 */,            2, 3},
                {null /* 3 */, null /* 2 */,            4, 1}
            });
            var rule = new BoxUniquenessRule();
            Assert.True(rule.TryInit(puzzle));
            IList<BitVector> initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            int val = 1;
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
        public void GetPossibleValues_MatchesGetPossibleBoxValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */, 2},
                {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                {null /* 4 */,            1, null /* 2 */, 3},
                {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new BoxUniquenessRule();
            Assert.True(rule.TryInit(puzzle));
            IList<BitVector> possibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    int box = Boxes.CalculateBoxIndex(new(row, column), Boxes.CalculateBoxSize(puzzle.Size));
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
    }
}