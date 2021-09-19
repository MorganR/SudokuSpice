using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleWithPossibleValuesTest
    {
        [Fact]
        public void Constructor_WithPuzzle_SetsUpCorrectly()
        {
            var allPossibleValues = new int[] { 0, 0, 1, 2 };
            var puzzle = new Puzzle(4, allPossibleValues);
            var puzzleWithPossibles = new PuzzleWithPossibleValues(puzzle);

            Assert.Equal(4, puzzleWithPossibles.Size);
            Assert.Equal(16, puzzleWithPossibles.NumSquares);
            Assert.Equal(0, puzzleWithPossibles.NumSetSquares);
            Assert.Equal(16, puzzleWithPossibles.NumEmptySquares);
            Assert.Equal(allPossibleValues, puzzleWithPossibles.AllPossibleValuesSpan.ToArray());
            Assert.Equal(new BitVector(0b111), puzzleWithPossibles.UniquePossibleValues);
            Assert.Equal(new BitVector(0b111), puzzleWithPossibles.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(
                new Dictionary<int, int> { {0, 2}, {1, 1}, {2, 1}, },
                puzzleWithPossibles.CountPerUniqueValue);
        }
    }
}
