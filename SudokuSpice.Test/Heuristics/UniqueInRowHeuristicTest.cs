using SudokuSpice.Rules;
using Xunit;

namespace SudokuSpice
{
    public class UniqueInRowHeuristicTest
    {
        [Fact]
        public void UpdateAll_ModifiesRelevantPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new UniqueInRowHeuristic(puzzle, possibleValues, ruleKeeper);

            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(0, 1)]); // Pre-modified
            Assert.Equal(new BitVector(0b1010), possibleValues[new Coordinate(1, 0)]); // Pre-modified
            Assert.Equal(new BitVector(0b1001), possibleValues[new Coordinate(2, 1)]); // Pre-modified
            Assert.Equal(new BitVector(0b0110), possibleValues[new Coordinate(2, 2)]); // Pre-modified
            
            heuristic.UpdateAll();
            
            Assert.Equal(new BitVector(0b1000), possibleValues[new Coordinate(0, 1)]); // Modified
            Assert.Equal(new BitVector(0b0100), possibleValues[new Coordinate(0, 2)]);
            Assert.Equal(new BitVector(0b0010), possibleValues[new Coordinate(1, 0)]); // Modified
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(1, 1)]);
            Assert.Equal(new BitVector(0b1100), possibleValues[new Coordinate(1, 3)]);
            Assert.Equal(new BitVector(0b1000), possibleValues[new Coordinate(2, 0)]);
            Assert.Equal(new BitVector(0b0001), possibleValues[new Coordinate(2, 1)]); // Modified
            Assert.Equal(new BitVector(0b0010), possibleValues[new Coordinate(2, 2)]); // Modified
            Assert.Equal(new BitVector(0b0100), possibleValues[new Coordinate(2, 3)]);
        }
    }
}
