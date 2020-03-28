using SudokuSpice.Data;
using SudokuSpice.Rules;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
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
            var heuristic = new UniqueInBoxHeuristic(puzzle, possibleValues, ruleKeeper);

            Assert.Equal(new BitVector(0b0111), possibleValues[new Coordinate(1, 1)]); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(0, 0)]);
            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(0, 1)]);
            Assert.Equal(new BitVector(0b0011), possibleValues[new Coordinate(1, 0)]);
            Assert.Equal(new BitVector(0b0100), possibleValues[new Coordinate(1, 1)]); // Modified

            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(2, 1)]);
            Assert.Equal(new BitVector(0b0010), possibleValues[new Coordinate(3, 1)]);

            Assert.Equal(new BitVector(0b0001), possibleValues[new Coordinate(1, 2)]);
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
            var heuristic = new UniqueInBoxHeuristic(puzzle, possibleValues, ruleKeeper);

            var puzzleCopy = new Puzzle(puzzle);
            var possibleValuesCopy = new PossibleValues(possibleValues);
            var ruleKeeperCopy = (StandardRuleKeeper) ruleKeeper.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy);
            var heuristicCopy = heuristic.CopyWithNewReferences(
                puzzleCopy, possibleValuesCopy, new List<ISudokuRestrict> { ruleKeeperCopy });

            var coord = new Coordinate(1, 1);
            var originalPossibleValues = possibleValues[coord];
            Assert.Equal(originalPossibleValues, possibleValuesCopy[coord]);
            heuristicCopy.UpdateAll();
            Assert.Equal(originalPossibleValues, possibleValues[coord]);
            Assert.NotEqual(originalPossibleValues, possibleValuesCopy[coord]);
        }
    }
}
