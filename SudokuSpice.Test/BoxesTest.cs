using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuSpice.Test
{
    public class BoxesTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(9, 3)]
        [InlineData(16, 4)]
        [InlineData(25, 5)]
        [InlineData(100, 10)]
        public void CalculateBoxSize_WithValidSizes_IsCorrect(int puzzleSize, int expectedBoxSize)
        {
            Assert.Equal(expectedBoxSize, Boxes.CalculateBoxSize(puzzleSize));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(8)]
        public void CalculateBoxSize_WithInvalidSize_Throws(int puzzleSize)
        {
            Assert.Throws<ArgumentException>(() => Boxes.CalculateBoxSize(puzzleSize));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(9, 3)]
        [InlineData(16, 4)]
        [InlineData(25, 5)]
        [InlineData(100, 10)]
        public void TryCalculateBoxSize_WithValidSizes_IsCorrect(int puzzleSize, int expectedBoxSize)
        {
            Assert.True(Boxes.TryCalculateBoxSize(puzzleSize, out int boxSize));
            Assert.Equal(expectedBoxSize, boxSize);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(8)]
        public void TryCalculateBoxSize_WithInvalidSize_ReturnsFalse(int puzzleSize)
        {
            Assert.False(Boxes.TryCalculateBoxSize(puzzleSize, out _));
        }

        [Theory]
        [InlineData(0, 0, 1, 0)]
        [InlineData(0, 0, 2, 0)]
        [InlineData(1, 1, 2, 0)]
        [InlineData(0, 2, 2, 1)]
        [InlineData(1, 3, 2, 1)]
        [InlineData(2, 1, 2, 2)]
        [InlineData(3, 0, 2, 2)]
        [InlineData(2, 2, 2, 3)]
        [InlineData(3, 3, 2, 3)]
        public void GetBoxIndex_SucceedsForValidValues(int row, int col, int boxSize, int expectedIndex)
        {
            Assert.Equal(expectedIndex, Boxes.CalculateBoxIndex(new(row, col), boxSize));
        }

        [Theory]
        [InlineData(1, 0, 0, 0)]
        [InlineData(2, 0, 0, 0)]
        [InlineData(2, 1, 0, 2)]
        [InlineData(2, 2, 2, 0)]
        [InlineData(2, 3, 2, 2)]
        public void GetStartingBoxCoordinate_SucceedsForValidValues(int boxSize, int box, int row, int col)
        {
            Assert.Equal(new Coordinate(row, col), Boxes.GetStartingBoxCoordinate(box, boxSize));
        }

        [Fact]
        public void YieldUnsetForBox_ReturnsAllUnsetCoordsInBox()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, null, null, 2},
                {null, null, 1, null},
                {null, 1, null, null},
                {3, null, 4, null}
            });
            int box = 1;
            var allUnset = new List<Coordinate>(Boxes.YieldUnsetCoordsForBox(box, boxSize: 2, puzzle: puzzle));
            Assert.Equal(2, allUnset.Count);
            Assert.Equal(new HashSet<Coordinate> { new(0, 2), new(1, 3) }, new HashSet<Coordinate>(allUnset));
        }
    }
}