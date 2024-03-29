﻿using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class StandardRulesTest
    {
        [Fact]
        public void TryInitFor_FiltersCorrectly()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1},
            });
            var rule = new StandardRules();

            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b01000), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b10000), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b10010), rule.GetPossibleValues(new Coordinate(2, 1)));
            Assert.Equal(new BitVector(0b01100), rule.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(new BitVector(0b01000), rule.GetPossibleValues(new Coordinate(2, 3)));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInRow_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                    new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                    new int?[] {null /* 2 */,            3,            3, null /* 4 */},
                    new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                    new int?[] {           3,            2,            4,            1}
                });
            var rule = new StandardRules();

            Assert.False(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInColumn_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(
                    new int?[][] {
                        new int?[] {           1, null /* 4 */, null /* 3 */, 2},
                        new int?[] {           1, null /* 3 */, null /* 1 */, 4},
                        new int?[] {null /* 4 */,            1, null /* 2 */, 3},
                        new int?[] {           3, null /* 2 */, null /* 4 */, 1}
                    });
            var rule = new StandardRules();

            Assert.False(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInBox_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(
                new int?[][] {
                    new int?[] {           1,      null /* 4 */, null /* 3 */, 2},
                    new int?[] {null /* 2 */, 1 /* INCORRECT */, null /* 1 */, null /* 4 */},
                    new int?[] {null /* 4 */,      null /* 1 */,            2, 3},
                    new int?[] {null /* 3 */,      null /* 2 */,            4, 1}
                });
            var rule = new StandardRules();

            Assert.False(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var rule = new StandardRules();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(0, 1);
            int secondVal = 4;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(2, 1), new Coordinate(1, 0) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 1), new Coordinate(2, 1), new Coordinate(1, 0) },
                new HashSet<Coordinate>(originalCoordTracker.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedCoordinate()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var rule = new StandardRules();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> {
                    new Coordinate(1, 0), new Coordinate(1, 3), new Coordinate(0, 1), new Coordinate(2, 1)
                },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(new BitVector(0b10000), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b10000), rule.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b10010), rule.GetPossibleValues(new Coordinate(2, 1)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedCoordinate()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var rule = new StandardRules();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            BitVector[,] initialPossibleValues = _GetPossibleValues(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, updatedCoordTracker);

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.TrackedCoords.ToArray(),
                revertedCoordTracker.TrackedCoords.ToArray());
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    Assert.Equal(
                        initialPossibleValues[row, column],
                        rule.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private BitVector[,] _GetPossibleValues(int puzzleSize, StandardRules rule)
        {
            var possibleValues = new BitVector[puzzleSize, puzzleSize];
            for (int row = 0; row < puzzleSize; row++)
            {
                for (int column = 0; column < puzzleSize; column++)
                {
                    possibleValues[row, column] = rule.GetPossibleValues(new Coordinate(row, column));
                }
            }
            return possibleValues;
        }
    }
}