using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Heuristics.Test
{
    public class UniqueInBoxHeuristicTest
    {
        [Fact]
        public void UpdateAll_ModifiesRelevantPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {null /* 1 */, null /* 4 */,            3,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */,            4},
                {null /* 4 */, null /* 1 */,            2,            3},
                {           3, null /* 2 */,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new UniqueInBoxHeuristic(
                puzzle, possibleValues, (IMissingBoxValuesTracker)ruleKeeper.GetRules()[0]);

            Assert.Equal(new BitVector(0b01110), possibleValues[new Coordinate(1, 1)]); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b10010), possibleValues[new Coordinate(0, 0)]);
            Assert.Equal(new BitVector(0b10010), possibleValues[new Coordinate(0, 1)]);
            Assert.Equal(new BitVector(0b00110), possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(new BitVector(0b01000), possibleValues[new Coordinate(1, 1)]); // Modified

            Assert.Equal(new BitVector(0b10010), possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(new BitVector(0b10010), possibleValues[new Coordinate(2, 1)]);
            Assert.Equal(new BitVector(0b00100), possibleValues[new Coordinate(3, 1)]);

            Assert.Equal(new BitVector(0b00010), possibleValues[new Coordinate(1, 2)]);
        }

        [Fact]
        public void CopyWithNewReferences_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {null /* 1 */, null /* 4 */,            3,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */,            4},
                {null /* 4 */, null /* 1 */,            2,            3},
                {           3, null /* 2 */,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new UniqueInBoxHeuristic(
                puzzle, possibleValues, (IMissingBoxValuesTracker)ruleKeeper.GetRules()[0]);

            var puzzleCopy = new Puzzle(puzzle);
            var possibleValuesCopy = new PossibleValues(possibleValues);
            var ruleKeeperCopy = (StandardRuleKeeper)ruleKeeper.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy);
            ISudokuHeuristic heuristicCopy = heuristic.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy, ruleKeeperCopy.GetRules());

            var coord = new Coordinate(1, 1);
            BitVector originalPossibleValues = possibleValues[coord];
            Assert.Equal(originalPossibleValues, possibleValuesCopy[coord]);
            heuristicCopy.UpdateAll();
            Assert.Equal(originalPossibleValues, possibleValues[coord]);
            Assert.NotEqual(originalPossibleValues, possibleValuesCopy[coord]);
        }

        [Fact]
        public void CopyWithNewReferences_WithoutIMissingBoxValuesTracker_Throws()
        {
            var puzzle = new Puzzle(new int?[,] {
                {null /* 1 */, null /* 4 */,            3,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */,            4},
                {null /* 4 */, null /* 1 */,            2,            3},
                {           3, null /* 2 */,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var boxRule = new BoxUniquenessRule(puzzle, possibleValues.AllPossible, false);
            var ruleKeeper = new DynamicRuleKeeper(puzzle, possibleValues, new List<ISudokuRule> { boxRule });
            var heuristic = new UniqueInBoxHeuristic(
                puzzle, possibleValues, boxRule);

            var puzzleCopy = new Puzzle(puzzle);
            var possibleValuesCopy = new PossibleValues(possibleValues);
            var ruleKeeperWithoutBoxTracker = new DynamicRuleKeeper(puzzleCopy, possibleValuesCopy,
                new List<ISudokuRule> {
                    new ColumnUniquenessRule(puzzleCopy, possibleValues.AllPossible),
                });
            Assert.Throws<ArgumentException>(() => heuristic.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy, ruleKeeperWithoutBoxTracker.GetRules()));
        }
    }
}
