using SudokuSpice.RuleBased.Rules;
using Xunit;

namespace SudokuSpice.RuleBased.Heuristics.Test
{
    public class UniqueInRowHeuristicTest
    {
        [Fact]
        public void UpdateAll_ModifiesRelevantPossibles()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInRowHeuristic(
                (IMissingRowValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(0, 1))); // Pre-modified
            Assert.Equal(new BitVector(0b10100), puzzle.GetPossibleValues(new Coordinate(1, 0))); // Pre-modified
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new Coordinate(2, 1))); // Pre-modified
            Assert.Equal(new BitVector(0b01100), puzzle.GetPossibleValues(new Coordinate(2, 2))); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(0, 1))); // Modified
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b00100), puzzle.GetPossibleValues(new Coordinate(1, 0))); // Modified
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b00010), puzzle.GetPossibleValues(new Coordinate(2, 1))); // Modified
            Assert.Equal(new BitVector(0b00100), puzzle.GetPossibleValues(new Coordinate(2, 2))); // Modified
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(2, 3)));
        }

        [Fact]
        public void CopyWithNewReferences_CreatesDeepCopy()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInRowHeuristic(
                (IMissingRowValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            var puzzleCopy = new PuzzleWithPossibleValues(puzzle);
            var ruleKeeperCopy = (StandardRuleKeeper)ruleKeeper.CopyWithNewReferences(
                puzzleCopy);
            IHeuristic heuristicCopy = heuristic.CopyWithNewReferences(
                puzzleCopy, ruleKeeperCopy.GetRules());

            var coord = new Coordinate(0, 1);
            BitVector originalPossibleValues = puzzle.GetPossibleValues(coord);
            Assert.Equal(originalPossibleValues, puzzleCopy.GetPossibleValues(coord));
            heuristicCopy.UpdateAll();
            Assert.Equal(originalPossibleValues, puzzle.GetPossibleValues(coord));
            Assert.NotEqual(originalPossibleValues, puzzleCopy.GetPossibleValues(coord));
        }
    }
}