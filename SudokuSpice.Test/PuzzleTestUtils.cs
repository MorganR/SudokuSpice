using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    internal static class PuzzleTestUtils
    {
        internal static void AssertStandardPuzzleSolved(IPuzzle puzzle)
        {
            Assert.Equal(0, puzzle.NumEmptySquares);
            var alreadyFound = new HashSet<int>(puzzle.Size);
            for (int row = 0; row < puzzle.Size; row++)
            {
                alreadyFound.Clear();
                for (int col = 0; col < puzzle.Size; col++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that row!");
                }
            }
            for (int col = 0; col < puzzle.Size; col++)
            {
                alreadyFound.Clear();
                for (int row = 0; row < puzzle.Size; row++)
                {
                    Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that col!");
                }
            }
            int boxSize = Boxes.CalculateBoxSize(puzzle.Size);
            for (int box = 0; box < puzzle.Size; box++)
            {
                alreadyFound.Clear();
                (int startRow, int startCol) = Boxes.GetStartingBoxCoordinate(box, boxSize);
                for (int row = startRow; row < startRow + boxSize; row++)
                {
                    for (int col = startCol; col < startCol + boxSize; col++)
                    {
                        Assert.True(alreadyFound.Add(puzzle[row, col].Value), $"Value at ({row}, {col}) clashed with another value in that box!");
                    }
                }
            }

        }
    }
}