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
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var heuristic = new UniqueInBoxHeuristic(puzzle, (BoxRestrict)restricts[2]);
            RestrictUtils.RestrictAllUnsetPossibleValues(puzzle, restricts);

            Assert.Equal(new BitVector(0b0111), puzzle.GetPossibleValues(1, 1)); // Pre-modified

            heuristic.UpdateAll();

            Assert.Equal(new BitVector(0b1001), puzzle.GetPossibleValues(0, 0));
            Assert.Equal(new BitVector(0b1001), puzzle.GetPossibleValues(0, 1));
            Assert.Equal(new BitVector(0b0011), puzzle.GetPossibleValues(1, 0));
            Assert.Equal(new BitVector(0b0100), puzzle.GetPossibleValues(1, 1)); // Modified

            Assert.Equal(new BitVector(0b1001), puzzle.GetPossibleValues(2, 0));
            Assert.Equal(new BitVector(0b1001), puzzle.GetPossibleValues(2, 1));
            Assert.Equal(new BitVector(0b0010), puzzle.GetPossibleValues(3, 1));

            Assert.Equal(new BitVector(0b0001), puzzle.GetPossibleValues(1, 2));
        }
    }
}
