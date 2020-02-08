namespace SudokuSpice
{
    /// <summary>
    /// Performs some logical trickery to reduce the number of possible values for a square. Unlike
    /// an <c>ISudokuRestrict</c>, this depends on the current possible values in the puzzle, not just
    /// the currently set values.
    /// </summary>
    public interface ISudokuHeuristic
    {
        /// <summary>
        /// Updates all the current possible values.
        /// </summary>
        void UpdateAll();
    }
}
