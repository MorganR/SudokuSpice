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
        /// <summary>The side-length of the puzzle (puzzles must be square).</summary>
        int Size { get; }
        /// <summary>The total number of squares in the puzzle.</summary>
        int NumSquares { get; }
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        int NumEmptySquares { get; }
        /// <summary>The number of set/known squares in the puzzle.</summary>
        int NumSetSquares { get; }
        /// <summary>
        /// Gets all the possible values for this puzzle.
        /// 
        /// If a value can be repeated n times in a region, then there should be n instances of it
        /// in the span.
        /// </summary>
        ReadOnlySpan<int> AllPossibleValuesSpan { get; }
        /// <summary>
        /// The count of times each unique value is expected to be included in a region.
        /// </summary>
        IReadOnlyDictionary<int,int> CountPerUniqueValue { get; }

        /// <summary>
        /// Gets the current value of a given square.
        /// </summary>
        int? this[int row, int col] { get; }

        [SuppressMessage(
            "Design",
            "CA1043:Use Integral Or String Argument For Indexers",
            Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        int? this[in Coordinate c] { get; }

        /// <summary>Gets a span of <c>Coordinate</c>s for all the unset squares.</summary>
        ReadOnlySpan<Coordinate> GetUnsetCoords();
    }
}