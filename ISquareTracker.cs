using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
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
        /// Gets the internal index for the next square to fill in. Precisely what this internal index
        /// means is implementation dependent.
        /// </summary>
        /// <returns>An internal index for the unset square with the least possible values.</returns>
        int GetBestIndexToGuess();
        /// <summary>
        /// Gets the possible values at the given internal index.
        /// </summary>
        /// <param name="idx">The internal index of the square to retrieve possible values for.</param>
        /// <returns>An enumerable of those possible values.</returns>
        IEnumerable<int> GetPossibleValues(int idx);
        /// <summary>
        /// Tries to set the square identified by the given index to the given possible value. This also 
        /// modifies its internal data as needed to maintain track of the square's values.
        /// If the value can't be set, this undoes any changes made to all squares and their possible values.
        /// </summary>
        /// <param name="idx">The index of the square to set.</param>
        /// <param name="possibleValue">The value to set the square to.</param>
        /// <returns>True if the set succeeded.</returns>
        bool TrySet(int idx, int possibleValue);
        /// <summary>
        /// Unsets the most-recently-set square.
        /// </summary>
        /// <param name="idx">The index that was used to initially set the square.</param>
        void Unset(int idx);
    }
}
