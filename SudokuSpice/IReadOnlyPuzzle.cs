using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Provides read-only access to a puzzle's data.
    /// </summary>
    public interface IReadOnlyPuzzle
    {
        public int Size { get; }
        /// <summary>The length of one side of a mini box within the puzzle.</summary>
        /// <para>A mini box is a square region that must contain each possible value exactly once.</para>
        public int BoxSize { get; }
        /// <summary>The total number of squares in the puzzle.</summary>
        public int NumSquares { get; }
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        public int NumEmptySquares { get; }
        /// <summary>The number of set/known squares in the puzzle.</summary>
        public int NumSetSquares { get; }

        /// <summary>Gets the current value of a given square. A square can be 'unset' by 
        /// setting its value to <c>null</c></summary>
        public int? this[int row, int col] { get; }

        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public int? this[in Coordinate c] { get; }

        /// <summary>Returns the index of the box that the given coordinates are in.</summary>
        public int GetBoxIndex(int row, int col);

        /// <summary>Returns the top-left coordinate for the given box.</summary>
        public Coordinate GetStartingBoxCoordinate(int box);

        /// <summary>Gets a span of <c>Coordinate</c>s for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords();

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given row.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForRow(int row);

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given column.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForColumn(int col);

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given box.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForBox(int box);
    }
}
