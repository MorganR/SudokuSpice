using System;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    public static class Matrices
    {
        [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Provided to ease migration.")]
        public static int?[][] CopyToJagged2D(this int?[,] matrix)
        {
            var numRows = matrix.GetLength(0);
            var numCols = matrix.GetLength(1);
            var copy = new int?[numRows][];
            for (int row = 0; row < copy.Length; ++row) {
                var rowCopy = new int?[numCols];
                for (int col = 0; col < rowCopy.Length; ++col)
                {
                    rowCopy[col] = matrix[row, col];
                }
                copy[row] = rowCopy;
            }
            return copy;
        }

        public static int?[][] Copy2D(this int?[][] matrix)
        {
             var copy = new int?[matrix.GetLength(0)][];
            for (int row = 0; row < copy.Length; ++row) {
                copy[row] = matrix[row].AsSpan().ToArray();
            }
            return copy;
        }
    }
}
