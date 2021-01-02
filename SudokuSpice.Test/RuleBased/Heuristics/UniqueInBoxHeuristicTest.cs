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
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInBoxHeuristic((IMissingBoxValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            Assert.Equal(new BitVector(0b01110), puzzle.GetPossibleValues(new(1, 1))); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new(0, 0)));
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new(0, 1)));
            Assert.Equal(new BitVector(0b00110), puzzle.GetPossibleValues(new(1, 0)));
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new(1, 1))); // Modified

            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new(2, 0)));
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new(2, 1)));
            Assert.Equal(new BitVector(0b00100), puzzle.GetPossibleValues(new(3, 1)));

            Assert.Equal(new BitVector(0b00010), puzzle.GetPossibleValues(new(1, 2)));
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
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInBoxHeuristic(
                (IMissingBoxValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            var puzzleCopy = new Puzzle(puzzle);
            var ruleKeeperCopy = (StandardRuleKeeper)ruleKeeper.CopyWithNewReferences(
                puzzleCopy);
            IHeuristic heuristicCopy = heuristic.CopyWithNewReferences(
                puzzleCopy, ruleKeeperCopy.GetRules());

            var coord = new Coordinate(1, 1);
            BitVector originalPossibleValues = puzzle.GetPossibleValues(coord);
            Assert.Equal(originalPossibleValues, puzzleCopy.GetPossibleValues(coord));
            heuristicCopy.UpdateAll();
            Assert.Equal(originalPossibleValues, puzzle.GetPossibleValues(coord));
            Assert.NotEqual(originalPossibleValues, puzzleCopy.GetPossibleValues(coord));
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
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInBoxHeuristic(
                (IMissingBoxValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            var puzzleCopy = new Puzzle(puzzle);
            var ruleKeeperWithoutBoxTracker = new DynamicRuleKeeper(new List<IRule> {
                    new ColumnUniquenessRule(),
                });
            Assert.Throws<ArgumentException>(() => heuristic.CopyWithNewReferences(
                puzzleCopy, ruleKeeperWithoutBoxTracker.GetRules()));
        }
    }
}