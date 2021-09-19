using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class MaxCountPerBoxRuleTest
    {
        [Fact]
        public void TryInit_ValidPuzzle_FiltersCorrectly()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */,            3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();

            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void TryInit_WithDuplicateValueInBox_Fails()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, 1 /* INCORRECT */, null /* 3 */,            2},
                new int?[] {null /* 2 */,                 3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null      /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null      /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();

            Assert.False(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */,            3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
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
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1), new Coordinate(3, 0) },
                new HashSet<Coordinate>(originalCoordTracker.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedBox()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */,            3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            int val = 1;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 2), new Coordinate(1, 3) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(2, 2)));
        }

        [Fact]
        public void UpdateAndRevert_WithDuplicateValues()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */,            3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);

            rule.Update(new Coordinate(2, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            rule.Update(new Coordinate(3, 1), 2, coordTracker);
            Assert.Equal(new BitVector(0b1010), rule.GetPossibleValues(new Coordinate(2, 0)));

            rule.Revert(new Coordinate(3, 1), 2, coordTracker);
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            rule.Revert(new Coordinate(2, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
        }

        [Fact]
        public void Revert_RevertsSpecifiedBox()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */,            3, null /* 1 */, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */,            2,            3},
                new int?[] {null /* 3 */, null /* 2 */,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerBoxRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));
            IList<BitVector> initialPossibleValuesByBox = _GetPossibleValuesByBox(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 2);
            int val = 1;
            rule.Update(in coord, val, updatedCoordTracker);

            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.TrackedCoords.ToArray(),
                revertedCoordTracker.TrackedCoords.ToArray());
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
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, null /* 1 */, 4},
                new int?[] {null /* 4 */,            1, null /* 2 */, 3},
                new int?[] {           3, null /* 2 */, null /* 4 */, 1}
            });
            var rule = new MaxCountPerBoxRule();
            Assert.True(rule.TryInit(puzzle, puzzle.UniquePossibleValues));

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int column = 0; column < puzzle.Size; column++)
                {
                    int box = Boxes.CalculateBoxIndex(new(row, column), Boxes.IntSquareRoot(puzzle.Size));
                    Assert.Equal(
                        rule.GetMissingValuesForBox(box),
                        rule.GetPossibleValues(new Coordinate(row, column)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByBox(int numBoxes, MaxCountPerBoxRule rule)
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