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
        /// Updates all the current possible values and returns them in a dictionary.
        /// </summary>
        bool UpdateAll();

        /// <summary>
        /// Undoes the last modifications made by this heuristic.
        /// </summary>
        void UndoLastUpdate();
    }
}
