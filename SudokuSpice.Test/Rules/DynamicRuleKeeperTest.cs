using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Rules.Test
{
    public class DynamicRuleKeeperTest
    {
        [Fact]
        public void Constructor_UpdatesPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
            {
                new RowRestrict(puzzle),
                new ColumnRestrict(puzzle),
                new BoxRestrict(puzzle, true)
            };
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);
            Assert.NotNull(ruleKeeper);
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(0, 1)]);
            Assert.Equal(new BitVector(0b0100), possibleValues[new Coordinate(0, 2)]);
            Assert.Equal(new BitVector(0b1010), possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(1, 1)]);
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(1, 3)]);
            Assert.Equal(new BitVector(0b1000), possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(2, 1)]);
            Assert.Equal(new BitVector(0b0110), possibleValues[new Coordinate(2, 2)]);
            Assert.Equal(new BitVector(0b0100), possibleValues[new Coordinate(2, 3)]);
        }

        [Fact]
        public void Constructor_WhenSquareHasNoPossibleValues_Throws()
        {
            var puzzle = new Puzzle(
                new int?[,] {
                    {           1,    3 /* 4 */, null /* 3 */,            2},
                    {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                    {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                    {           3,            2,            4,            1}
                });
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
                {
                    new RowRestrict(puzzle),
                    new ColumnRestrict(puzzle),
                    new BoxRestrict(puzzle, true)
                };
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);
            });
            Assert.Contains("Puzzle could not be solved with the given values", ex.Message);
        }

        [Fact]
        public void CopyWithNewReferences_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
            {
                new RowRestrict(puzzle),
                new ColumnRestrict(puzzle),
                new BoxRestrict(puzzle, true)
            };
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);

            var puzzleCopy = new Puzzle(puzzle);
            var possibleValuesCopy = new PossibleValues(possibleValues);
            var ruleKeeperCopy = ruleKeeper.CopyWithNewReferences(puzzleCopy, possibleValuesCopy);

            var restrictsCopy = ruleKeeperCopy.GetRestricts();
            Assert.Equal(restricts.Count, restrictsCopy.Count);
            for (int i = 0; i < restricts.Count; i++)
            {
                Assert.NotSame(restricts[i], restrictsCopy[i]);
                var originalType = restricts[i].GetType();
                var copiedType = restrictsCopy[i].GetType();
                Assert.Equal(originalType, copiedType);
            }
            var coord = new Coordinate(0, 1);
            var val = 4;
            Assert.True(ruleKeeperCopy.TrySet(coord, val));
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(1, 1)]);
            Assert.Equal(new BitVector(0b0100), possibleValuesCopy[new Coordinate(1, 1)]);
        }

        [Fact]
        public void TrySet_WithValidValue_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
                {
                    new RowRestrict(puzzle),
                    new ColumnRestrict(puzzle),
                    new BoxRestrict(puzzle, true)
                };
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);
            var coord = new Coordinate(1, 1);
            var val = 3;
            Assert.True(ruleKeeper.TrySet(coord, val));
            puzzle[coord] = val;
            Assert.Equal(new BitVector(0b1000), possibleValues[new Coordinate(0, 1)]);
            Assert.Equal(new BitVector(0b1010), possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(new BitVector(0b1000), possibleValues[new Coordinate(1, 3)]);
            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(2, 1)]);
        }

        [Fact]
        public void TrySet_WithInvalidValue_FailsAndLeavesUnchanged()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
                {
                    new RowRestrict(puzzle),
                    new ColumnRestrict(puzzle),
                    new BoxRestrict(puzzle, true)
                };
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);
            var initialPossibleValues = _RetrieveAllUnsetPossibleValues(puzzle, possibleValues);
            var coord = new Coordinate(1, 0);
            var val = 4;
            Assert.False(ruleKeeper.TrySet(coord, val));

            foreach (var c in puzzle.GetUnsetCoords())
            {
                Assert.Equal(initialPossibleValues[c], possibleValues[in c]);
            }
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
            var possibleValues = new PossibleValues(puzzle);
            var restricts = new List<ISudokuRestrict>
                {
                    new RowRestrict(puzzle),
                    new ColumnRestrict(puzzle),
                    new BoxRestrict(puzzle, true)
                };
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, restricts);
            var initialPossibleValues = _RetrieveAllUnsetPossibleValues(puzzle, possibleValues);
            var coord = new Coordinate(1, 1);
            var val = 3;
            Assert.True(ruleKeeper.TrySet(in coord, val));
            puzzle[in coord] = val;

            puzzle[in coord] = null;
            ruleKeeper.Unset(in coord, val);

            foreach (var c in puzzle.GetUnsetCoords())
            {
                Assert.Equal(initialPossibleValues[c], possibleValues[in c]);
            }
        }

        private IDictionary<Coordinate, BitVector> _RetrieveAllUnsetPossibleValues(
            Puzzle puzzle, PossibleValues possibleValues)
        {
            var unsetPossibleValues = new Dictionary<Coordinate, BitVector>();
            foreach (var c in puzzle.GetUnsetCoords())
            {
                unsetPossibleValues[c] = possibleValues[in c];
            }
            return unsetPossibleValues;
        }
    }
}
