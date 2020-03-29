using SudokuSpice.Data;
using SudokuSpice.Rules;
using System.Collections.Generic;

namespace SudokuSpice.Heuristics
{
    /// <summary>
    /// Performs some logical trickery to reduce the number of possible values for a square. Unlike
    /// an <c>ISudokuRule</c>, this depends on the current possible values in the puzzle, not just
    /// the currently set values.
    /// </summary>
    public interface ISudokuHeuristic
    {
        /// <summary>
        /// Updates all the current possible values.
        /// </summary>
        /// <returns>Returns true if any modifications were made.</returns>
        bool UpdateAll();

        /// <summary>
        /// Undoes the last modifications made by this heuristic.
        /// </summary>
        void UndoLastUpdate();

        /// <summary>
        /// Creates a deep copy of this heuristic, replacing internal references with the ones
        /// provided. These references may or may not be used during the copy, depending on the
        /// implementation of the heuristic.
        /// </summary>
        ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules);
    }
}
