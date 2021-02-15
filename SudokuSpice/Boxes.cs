using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Provides utilities for working with "boxes", i.e. square regions within standard Sudokus.
    /// </summary>
    public static class Boxes
    {
        /// <summary>
        /// Calculates the square root of a number. Only works for integers that have integer
        /// square roots.
        ///
        /// This is useful for determining the standard box-size for a puzzle (i.e.
        /// <paramref name="toRoot"/> would be the size of the puzzle, and the result would be the
        /// size of each box.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="toRoot"/> is not the square of a whole number.
        /// </exception>
        public static int IntSquareRoot(int toRoot)
        {
            switch (toRoot)
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
                    int root = (int)Math.Sqrt(toRoot);
                    if (root == 0 || root * root != toRoot)
                    {
                        throw new ArgumentException($"{nameof(toRoot)} must be square.");
                    }
                    return root;
            }
        }

        /// <summary>
        /// Calculates the square root of a number. Only works for integers that have integer
        /// square roots.
        /// 
        /// This is useful for determining the standard box-size for a puzzle (i.e.
        /// <paramref name="toRoot"/> would be the size of the puzzle, and
        /// <paramref name="root"/> would be the size of each box).
        /// </summary>
        /// <param name="toRoot">
        /// The number to root. Should be the square of another integer.
        /// </param>
        /// <param name="root">Out parameter: the square root of <paramref name="toRoot"/>.</param>
        /// <returns>
        /// False if <paramref name="toRoot"/> is not the square of a whole number.
        /// </returns>
        public static bool TryIntSquareRoot(int toRoot, out int root)
        {
            switch (toRoot)
            {
                case 1:
                    root = 1;
                    break;
                case 4:
                    root = 2;
                    break;
                case 9:
                    root = 3;
                    break;
                case 16:
                    root = 4;
                    break;
                case 25:
                    root = 5;
                    break;
                default:
                    root = (int)Math.Sqrt(toRoot);
                    if (toRoot == 0 || root * root != toRoot)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Calculate the zero-based box-index of the given coordinate, starting at the top-left.
        /// </summary>
        public static int CalculateBoxIndex(in Coordinate c, int boxSize)
        {
            return c.Row / boxSize * boxSize + c.Column / boxSize;
        }

        /// <summary>Returns the top-left coordinate for the given box.</summary>
        public static Coordinate GetStartingBoxCoordinate(int box, int boxSize)
        {
            return new Coordinate(box / boxSize * boxSize, box % boxSize * boxSize);
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