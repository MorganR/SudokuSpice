using SudokuSpice.RuleBased.Rules;
using System;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Performs some logical trickery to reduce the number of possible values for a square. Unlike
    /// an <see cref="IRule"/>, this depends on the current possible values in the puzzle, not
    /// just the currently set values.
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// Tries to initialize this heuristic for solving the given puzzle.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the heuristic's state is unchanged
        /// on failure.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <returns>
        /// False if this heuristic cannot be initialized for the given puzzle, else true.
        /// </returns>
        bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle);

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
        IHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            ReadOnlySpan<IRule> rules);
    }
}