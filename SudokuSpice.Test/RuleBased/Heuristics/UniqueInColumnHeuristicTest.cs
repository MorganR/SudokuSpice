using SudokuSpice.RuleBased.Rules;
using Xunit;

namespace SudokuSpice.RuleBased.Heuristics.Test
{
    public class UniqueInColumnHeuristicTest
    {
        [Fact]
        public void UpdateAll_ModifiesRelevantPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */,            3},
                {           3,            2, null /* 4 */,            1}
            });
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInColumnHeuristic(
                (IMissingColumnValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));

            Assert.Equal(new BitVector(0b11000), puzzle.GetPossibleValues(new Coordinate(0, 1))); // Pre-modified
            Assert.Equal(new BitVector(0b10010), puzzle.GetPossibleValues(new Coordinate(2, 1))); // Pre-modified
            Assert.Equal(new BitVector(0b01010), puzzle.GetPossibleValues(new Coordinate(1, 2))); // Pre-modified
            Assert.Equal(new BitVector(0b10100), puzzle.GetPossibleValues(new Coordinate(2, 2))); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b00100), puzzle.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(0, 1))); // Modified
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new BitVector(0b00010), puzzle.GetPossibleValues(new Coordinate(2, 1))); // Modified
            Assert.Equal(new BitVector(0b01000), puzzle.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new BitVector(0b00010), puzzle.GetPossibleValues(new Coordinate(1, 2))); // Modified
            Assert.Equal(new BitVector(0b00100), puzzle.GetPossibleValues(new Coordinate(2, 2))); // Modified
            Assert.Equal(new BitVector(0b10000), puzzle.GetPossibleValues(new Coordinate(3, 2)));
        }

        [Fact]
        public void CopyWithNewReferences_CreatesDeepCopy()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */,            3},
                {           3,            2, null /* 4 */,            1}
            });
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(ruleKeeper.TryInit(puzzle));
            var heuristic = new UniqueInColumnHeuristic(
                (IMissingColumnValuesTracker)ruleKeeper.GetRules()[0]);
            Assert.True(heuristic.TryInitFor(puzzle));
            var puzzleCopy = new Puzzle(puzzle);

            var ruleKeeperCopy = (StandardRuleKeeper)ruleKeeper.CopyWithNewReferences(
                puzzleCopy);
            ISudokuHeuristic heuristicCopy = heuristic.CopyWithNewReferences(
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