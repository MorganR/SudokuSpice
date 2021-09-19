using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class MaxCountPerColumnRuleTest
    {
        [Fact]
        public void TryInit_FiltersCorrectly()
        {
            var p = new Puzzle(new int?[][] {
                new int?[] {           1, null /* 2 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, null /* 1 */, 2},
                new int?[] {null /* 2 */,            1, null /* 2 */, 3},
                new int?[] {           3, null /* 2 */, null /* 2 */, 1},
            }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerColumnRule();

            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b1100), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(0, 3)));
        }

        [Fact]
        public void TryInit_WithTooManyValuesInColumn_Fails()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1,      null /* 2 */, null /* 3 */, 2},
                        new int?[] {null /* 2 */,      null /* 3 */, null /* 1 */, 2},
                        new int?[] {null /* 2 */,                 1, null /* 2 */, 3},
                        new int?[] {           3, 1 /* INCORRECT */, null /* 2 */, 1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerColumnRule();

            Assert.False(rule.TryInit(puzzle, puzzle.AllPossibleValues));
        }

        [Fact]
        public void CopyWithNewReference_CreatesDeepCopy()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */, 2},
                        new int?[] {null /* 2 */, null /* 3 */, null /* 1 */, 2},
                        new int?[] {null /* 2 */,            1, null /* 2 */, 3},
                        new int?[] {           3, null /* 2 */, null /* 2 */, 1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 3;
            var coord = new Coordinate(1, 1);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(0, 1);
            int secondVal = 2;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(3, 1) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 1), new Coordinate(3, 1) },
                new HashSet<Coordinate>(originalCoordTracker.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedColumn()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */, 2},
                        new int?[] {null /* 2 */, null /* 3 */, null /* 1 */, 2},
                        new int?[] {null /* 2 */,            1, null /* 2 */, 3},
                        new int?[] {           3, null /* 2 */, null /* 2 */, 1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(0, 1), new Coordinate(3, 1) },
                new HashSet<Coordinate>(coordTracker.TrackedCoords.ToArray()));
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b1110), rule.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(0, 3)));
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
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(new Coordinate(1, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            rule.Update(new Coordinate(2, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b0000), rule.GetPossibleValues(new Coordinate(0, 0)));

            rule.Revert(new Coordinate(2, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
            rule.Revert(new Coordinate(1, 0), 2, coordTracker);
            Assert.Equal(new BitVector(0b0100), rule.GetPossibleValues(new Coordinate(0, 0)));
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
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, rule);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, new CoordinateTracker(puzzle.Size));

            rule.Revert(coord, val);

            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column], rule.GetMissingValuesForColumn(column));
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
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> initialPossibleValuesByColumn = _GetPossibleValuesByColumn(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, updatedCoordTracker);

            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.TrackedCoords.ToArray(),
                revertedCoordTracker.TrackedCoords.ToArray());
            for (int column = 0; column < initialPossibleValuesByColumn.Count; column++)
            {
                Assert.Equal(
                    initialPossibleValuesByColumn[column],
                    rule.GetMissingValuesForColumn(column));
            }
        }

        [Fact]
        public void GetPossibleValues_MatchesGetMissingValuesForColumn()
        {
            var p = new Puzzle(
                    new int?[][] {
                        new int?[] {           1, null /* 2 */, null /* 3 */,            2},
                        new int?[] {null /* 2 */, null /* 3 */,            1, null /* 2 */},
                        new int?[] {null /* 2 */, null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,            2,            2,            1},
                    }, new int[] { 1, 2, 2, 3 });
            var puzzle = new PuzzleWithPossibleValues(p);
            var rule = new MaxCountPerColumnRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        rule.GetMissingValuesForColumn(col),
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByColumn(int numColumns, MaxCountPerColumnRule rule)
        {
            var possibleColumnValues = new List<BitVector>();
            for (int column = 0; column < numColumns; column++)
            {
                possibleColumnValues.Add(rule.GetMissingValuesForColumn(column));
            }
            return possibleColumnValues;
        }
    }
}