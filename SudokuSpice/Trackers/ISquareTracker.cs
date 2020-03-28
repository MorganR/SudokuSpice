using SudokuSpice.Data;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Tracks and sets Sudoku squares and their possible values.
    /// </summary>
    public interface ISquareTracker
    {
        /// <summary>
        /// Gets the number of empty squares remaining.
        /// </summary>
        int GetNumEmptySquares();
        /// <summary>
        /// Gets the coordinate for the next square to fill in.
        /// </summary>
        /// <returns>The coordinate for the unset square with the least possible values.</returns>
        Coordinate GetBestCoordinateToGuess();
        /// <summary>
        /// Gets the possible values at the given internal index.
        /// </summary>
        /// <param name="c">The coordinate of the square to retrieve possible values for.</param>
        /// <returns>A list of those possible values.</returns>
        List<int> GetPossibleValues(in Coordinate c);
        /// <summary>
        /// Tries to set the square at the given coordinate to the given possible value. This also 
        /// modifies its internal data as needed to maintain track of the square's values. If the
        /// value can't be set, this undoes any changes made to all squares and their possible
        /// values.
        /// </summary>
        /// <param name="c">The coordinate of the square to set.</param>
        /// <param name="value">The value to set the square to.</param>
        /// <returns>True if the set succeeded.</returns>
        bool TrySet(in Coordinate c, int value);
        /// <summary>
        /// Unsets the most recently set square.
        /// </summary>
        void UnsetLast();
        /// <summary>
        /// Creates a deep copy of this ISquareTracker in its current state.
        /// </summary>
        ISquareTracker DeepCopy();
    }
}
