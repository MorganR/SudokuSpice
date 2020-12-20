using System;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides utilities for working with "boxes", i.e. square regions within standard Sudokus.
    /// </summary>
    public static class Boxes
    {
        /// <summary>
        /// Calculates the size of a box within a puzzle (this should be the square root).
        /// </summary>
        public static int CalculateBoxSize(int puzzleSize)
        {
            switch (puzzleSize)
            {
                case 1:
                    return 1;
                case 4:
                    return 2;
                case 9:
                    return 3;
                case 16:
                    return 4;
                case 25:
                    return 5;
                default:
                    int root = (int)Math.Sqrt(puzzleSize);
                    if (root * root != puzzleSize)
                    {
                        throw new ArgumentException($"{nameof(puzzleSize)} must be square.");
                    }
                    return root;
            }
        }

        /// <summary>
        /// Calculate the zero-based box-index of the given coordinate, starting at the top-left.
        /// </summary>
        public static int CalculateBoxIndex(in Coordinate c, int boxSize)
        {
            return (c.Row / boxSize) * boxSize + c.Column / boxSize;
        }

        /// <summary>Returns the top-left coordinate for the given box.</summary>
        public static Coordinate GetStartingBoxCoordinate(int box, int boxSize)
        {
            return new Coordinate((box / boxSize) * boxSize, (box % boxSize) * boxSize);
        }

        /// <summary>
        /// Yields an enumerable of coordinates for all the unset squares in the given box.
        /// </summary>
        public static IEnumerable<Coordinate> YieldUnsetCoordsForBox(int box, int boxSize, IReadOnlyPuzzle puzzle)
        {
            (int startRow, int startCol) = GetStartingBoxCoordinate(box, boxSize);
            int endRow = startRow + boxSize;
            int endCol = startCol + boxSize;
            for (int row = startRow; row < endRow; row++)
            {
                for (int col = startCol; col < endCol; col++)
                {
                    if (puzzle[row, col].HasValue)
                    {
                        continue;
                    }
                    yield return new Coordinate(row, col);
                }
            }
        }
    }
}