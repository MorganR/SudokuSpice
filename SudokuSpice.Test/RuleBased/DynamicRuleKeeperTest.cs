using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class DynamicRuleKeeperTest
    {
        [Fact]
        public void TryInitFor_UpdatesPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var rules = new List<IRule>
            {
                new RowUniquenessRule(),
                new ColumnUniquenessRule(),
                new BoxUniquenessRule()
            };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));

            Assert.NotNull(ruleKeeper);
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b10100), puzzle.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new Coordinate(2, 1)));
            Assert.Equal(new BitVector(0b01100), puzzle.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(2, 3)));
        }

        [Fact]
        public void TryInitFor_WhenSquareHasNoPossibleValues_Fails()
        {
            var puzzle = new Puzzle(
                new int?[,] {
                    {           1,    3 /* 4 */, null /* 3 */,            2},
                    {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                    {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                    {           3,            2,            4,            1}
                });
            var rules = new List<IRule>
                {
                    new RowUniquenessRule(),
                    new ColumnUniquenessRule(),
                    new BoxUniquenessRule()
                };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.False(ruleKeeper.TryInit(puzzle));
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
            var rules = new List<IRule>
            {
                new RowUniquenessRule(),
                new ColumnUniquenessRule(),
                new BoxUniquenessRule()
            };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));

            var puzzleCopy = new Puzzle(puzzle);
            IRuleKeeper ruleKeeperCopy = ruleKeeper.CopyWithNewReferences(puzzleCopy);

            IReadOnlyList<IRule> rulesCopy = ruleKeeperCopy.GetRules();
            Assert.Equal(rules.Count, rulesCopy.Count);
            for (int i = 0; i < rules.Count; i++)
            {
                Assert.NotSame(rules[i], rulesCopy[i]);
                Type originalType = rules[i].GetType();
                Type copiedType = rulesCopy[i].GetType();
                Assert.Equal(originalType, copiedType);
            }
            var coord = new Coordinate(0, 1);
            int val = 4;
            Assert.True(ruleKeeperCopy.TrySet(coord, val));
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b01000), puzzleCopy.GetPossibleValues(new Coordinate(1, 1)));
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
            var rules = new List<IRule>
                {
                    new RowUniquenessRule(),
                    new ColumnUniquenessRule(),
                    new BoxUniquenessRule()
                };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));
            var coord = new Coordinate(1, 1);
            int val = 3;

            Assert.True(ruleKeeper.TrySet(coord, val));

            puzzle[coord] = val;
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new BitVector(0b10100), puzzle.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new Coordinate(2, 1)));
        }

        [Fact]
        public void TrySet_WithValueThatCausesNoPossiblesForOtherSquare_FailsAndLeavesUnchanged()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var rules = new List<IRule>
                {
                    new RowUniquenessRule(),
                    new ColumnUniquenessRule(),
                    new BoxUniquenessRule()
                };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));
            IDictionary<Coordinate, BitVector> initialPossibleValues = _RetrieveAllUnsetPossibleValues(puzzle);
            var coord = new Coordinate(1, 0);
            int val = 4;

            Assert.False(ruleKeeper.TrySet(coord, val));

            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                Assert.Equal(initialPossibleValues[c], puzzle.GetPossibleValues(in c));
            }
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
            var rules = new List<IRule>
                {
                    new RowUniquenessRule(),
                    new ColumnUniquenessRule(),
                    new BoxUniquenessRule()
                };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));
            IDictionary<Coordinate, BitVector> initialPossibleValues = _RetrieveAllUnsetPossibleValues(puzzle);
            var coord = new Coordinate(1, 1);
            int val = 2;

            Assert.False(ruleKeeper.TrySet(coord, val));

            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                Assert.Equal(initialPossibleValues[c], puzzle.GetPossibleValues(in c));
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
            var rules = new List<IRule>
                {
                    new RowUniquenessRule(),
                    new ColumnUniquenessRule(),
                    new BoxUniquenessRule()
                };
            var ruleKeeper = new DynamicRuleKeeper(rules);
            Assert.True(ruleKeeper.TryInit(puzzle));
            IDictionary<Coordinate, BitVector> initialPossibleValues = _RetrieveAllUnsetPossibleValues(puzzle);
            var coord = new Coordinate(1, 1);
            int val = 3;
            Assert.True(ruleKeeper.TrySet(in coord, val));
            puzzle[in coord] = val;

            puzzle[in coord] = null;
            ruleKeeper.Unset(in coord, val);

            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                Assert.Equal(initialPossibleValues[c], puzzle.GetPossibleValues(in c));
            }
        }

        private static IDictionary<Coordinate, BitVector> _RetrieveAllUnsetPossibleValues(
            Puzzle puzzle)
        {
            var unsetPossibleValues = new Dictionary<Coordinate, BitVector>();
            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                unsetPossibleValues[c] = puzzle.GetPossibleValues(in c);
            }
            return unsetPossibleValues;
        }
    }
}