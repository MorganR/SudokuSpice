using System.Collections.Generic;

namespace MorganRoff.Sudoku
{
    /// <summary>
    /// Performs some logical trickery to reduce the number of possible values for a square. Unlike
    /// an <c>IRestrict</c>, this depends on the current possible values in the puzzle, not just
    /// the currently set values.
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// Updates all the current possible values.
        /// </summary>
        void UpdateAll();
    }
}
