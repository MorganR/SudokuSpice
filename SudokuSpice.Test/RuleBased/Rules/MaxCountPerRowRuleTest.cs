using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class MaxCountPerRowRuleTest
    {
        [Fact]
        public void TryInit_FiltersCorrectly()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            2,            1}
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();

            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void TryInit_WithTooManyValuesInRow_Fails()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, /* INCORRECT */ 1, null /* 3 */,            2},
                        new int?[] {null /* 2 */,      null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */,      null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,                 2,            2,            1}
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();

            Assert.False(rule.TryInit(puzzle, puzzle.AllPossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(1, 0);
            int secondVal = 2;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 3) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 1), new Coordinate(1, 3) },
                new HashSet<Coordinate>(originalCoordTracker.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedRow()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 0), new Coordinate(1, 3) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void UpdateAndRevert_WithDuplicatePossibleValue()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(new Coordinate(1, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(1, 0)));
            rule.Update(new Coordinate(1, 3), 2, coordTracker);
            Assert.Equal(new BitVector(0b1000), rule.GetPossibleValues(new Coordinate(1, 0)));

            rule.Revert(new Coordinate(1, 3), 2, coordTracker);
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(1, 0)));
            rule.Revert(new Coordinate(1, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(1, 0)));
        }

        [Fact]
        public void Revert_WithoutAffectedList_RevertsSpecifiedRow()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> initialPossibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, rule);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val);

            for (int row = 0; row < initialPossibleValuesByRow.Count; row++)
            {
                Assert.Equal(
                    initialPossibleValuesByRow[row], rule.GetMissingValuesForRow(row));
            }
        }

        [Fact]
        public void Revert_RevertsSpecifiedRow()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> initialPossibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, updatedCoordTracker);

            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.TrackedCoords.ToArray(),
                revertedCoordTracker.TrackedCoords.ToArray());
            for (int row = 0; row < initialPossibleValuesByRow.Count; row++)
            {
                Assert.Equal(
                    initialPossibleValuesByRow[row],
                    rule.GetMissingValuesForRow(row));
            }
        }

        [Fact]
        public void GetPossibleValues_MatchesGetMissingValuesForRow()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerRowRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        rule.GetMissingValuesForRow(row),
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByRow(int numRows, MaxCountPerRowRule rule)
        {
            var possibleRowValues = new List<BitVector>();
            for (int row = 0; row < numRows; row++)
            {
                possibleRowValues.Add(rule.GetMissingValuesForRow(row));
            }
            return possibleRowValues;
        }
    }
}