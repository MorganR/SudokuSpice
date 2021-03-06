using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Rules.Test
{
    public class RowUniquenessRuleTest
    {
        [Fact]
        public void TryInitFor_FiltersCorrectly()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var rule = new RowUniquenessRule();

            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b11100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void TryInitFor_WithDuplicateValueInRow_Fails()
        {
            var puzzle = new PuzzleWithPossibleValues(
                    new int?[][] {
                        new int?[] {           1, /* INCORRECT */ 1, null /* 3 */,            2},
                        new int?[] {null /* 2 */,      null /* 3 */,            1, null /* 4 */},
                        new int?[] {null /* 4 */,      null /* 1 */, null /* 2 */, null /* 3 */},
                        new int?[] {           3,                 2,            4,            1}
                    });
            var rule = new RowUniquenessRule();

            Assert.False(rule.TryInit(puzzle, puzzle.AllPossibleValues));
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
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            IRule ruleCopy = rule.CopyWithNewReference(puzzleCopy);
            int val = 2;
            var coord = new Coordinate(2, 2);
            ruleCopy.Update(coord, val, new CoordinateTracker(puzzle.Size));
            Assert.NotEqual(rule.GetPossibleValues(coord), ruleCopy.GetPossibleValues(coord));

            puzzleCopy[coord] = val;
            var secondCoord = new Coordinate(2, 3);
            int secondVal = 3;
            var coordTracker = new CoordinateTracker(puzzle.Size);
            ruleCopy.Update(secondCoord, secondVal, coordTracker);
            var originalCoordTracker = new CoordinateTracker(puzzle.Size);
            rule.Update(secondCoord, secondVal, originalCoordTracker);
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(2, 0), new Coordinate(2, 1), new Coordinate(2, 2) },
                new HashSet<Coordinate>(originalCoordTracker.GetTrackedCoords().ToArray()));
        }

        [Fact]
        public void Update_UpdatesSpecifiedRow()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            var coordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;

            rule.Update(coord, val, coordTracker);

            Assert.Equal(
                new HashSet<Coordinate> { new Coordinate(1, 0), new Coordinate(1, 3) },
                new HashSet<Coordinate>(coordTracker.GetTrackedCoords().ToArray()));
            Assert.Equal(new BitVector(0b11000), rule.GetPossibleValues(new Coordinate(0, 0)));
            Assert.Equal(new BitVector(0b10100), rule.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b11110), rule.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00000), rule.GetPossibleValues(new Coordinate(3, 0)));
        }

        [Fact]
        public void Revert_WithoutAffectedList_RevertsSpecifiedRow()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
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
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> initialPossibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, rule);
            var updatedCoordTracker = new CoordinateTracker(puzzle.Size);
            var revertedCoordTracker = new CoordinateTracker(puzzle.Size);
            var coord = new Coordinate(1, 1);
            int val = 3;
            rule.Update(coord, val, updatedCoordTracker);

            rule.Revert(coord, val, revertedCoordTracker);

            Assert.Equal(
                updatedCoordTracker.GetTrackedCoords().ToArray(),
                revertedCoordTracker.GetTrackedCoords().ToArray());
            for (int row = 0; row < initialPossibleValuesByRow.Count; row++)
            {
                Assert.Equal(
                    initialPossibleValuesByRow[row],
                    rule.GetMissingValuesForRow(row));
            }
        }

        [Fact]
        public void GetPossibleRowValues_IsCorrect()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));

            Assert.Equal(new BitVector(0b11000), rule.GetMissingValuesForRow(0));
            Assert.Equal(new BitVector(0b11100), rule.GetMissingValuesForRow(1));
            Assert.Equal(new BitVector(0b11110), rule.GetMissingValuesForRow(2));
            Assert.Equal(new BitVector(0b00000), rule.GetMissingValuesForRow(3));
        }

        [Fact]
        public void GetPossibleValues_MatchesGetPossibleRowValues()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {1, null /* 4 */, null /* 3 */, 2},
                new int?[] {null /* 2 */, null /* 3 */, 1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {3, 2, 4, 1}
            });
            var rule = new RowUniquenessRule();
            Assert.True(rule.TryInit(puzzle, puzzle.AllPossibleValues));
            IList<BitVector> possibleValuesByRow = _GetPossibleValuesByRow(puzzle.Size, rule);

            for (int row = 0; row < possibleValuesByRow.Count; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.Equal(
                        possibleValuesByRow[row],
                        rule.GetPossibleValues(new Coordinate(row, col)));
                }
            }
        }

        private IList<BitVector> _GetPossibleValuesByRow(int numRows, RowUniquenessRule rule)
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