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
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new UniqueInColumnHeuristic(
                puzzle, possibleValues, (IMissingColumnValuesTracker)ruleKeeper.GetRules()[0]);

            Assert.Equal(new BitVector(0b11000), possibleValues[new Coordinate(0, 1)]); // Pre-modified
            Assert.Equal(new BitVector(0b10010), possibleValues[new Coordinate(2, 1)]); // Pre-modified
            Assert.Equal(new BitVector(0b01010), possibleValues[new Coordinate(1, 2)]); // Pre-modified
            Assert.Equal(new BitVector(0b10100), possibleValues[new Coordinate(2, 2)]); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b00100), possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(new BitVector(0b10000), possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(new BitVector(0b10000), possibleValues[new Coordinate(0, 1)]); // Modified
            Assert.Equal(new BitVector(0b01000), possibleValues[new Coordinate(1, 1)]);
            Assert.Equal(new BitVector(0b00010), possibleValues[new Coordinate(2, 1)]); // Modified
            Assert.Equal(new BitVector(0b01000), possibleValues[new Coordinate(0, 2)]);
            Assert.Equal(new BitVector(0b00010), possibleValues[new Coordinate(1, 2)]); // Modified
            Assert.Equal(new BitVector(0b00100), possibleValues[new Coordinate(2, 2)]); // Modified
            Assert.Equal(new BitVector(0b10000), possibleValues[new Coordinate(3, 2)]);
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
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new UniqueInColumnHeuristic(
                puzzle, possibleValues, (IMissingColumnValuesTracker)ruleKeeper.GetRules()[0]);
            var puzzleCopy = new Puzzle(puzzle);

            var possibleValuesCopy = new PossibleValues(possibleValues);
            var ruleKeeperCopy = (StandardRuleKeeper)ruleKeeper.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy);
            ISudokuHeuristic heuristicCopy = heuristic.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy, ruleKeeperCopy.GetRules());

            var coord = new Coordinate(0, 1);
            BitVector originalPossibleValues = possibleValues[coord];
            Assert.Equal(originalPossibleValues, possibleValuesCopy[coord]);
            heuristicCopy.UpdateAll();
            Assert.Equal(originalPossibleValues, possibleValues[coord]);
            Assert.NotEqual(originalPossibleValues, possibleValuesCopy[coord]);
        }
    }
}
