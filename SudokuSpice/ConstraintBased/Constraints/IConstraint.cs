namespace SudokuSpice.ConstraintBased.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Adds necessary <see cref="IObjective"/>s and links to the given
        /// <paramref name="graph"/> in order to solve the given <paramref name="puzzle"/>
        /// according to this constraint. The details here are implementation-specific.
        /// </summary>
        /// <remarks>
        /// This should skip adding objectives that are already satisfied by the given
        /// <paramref name="puzzle"/>. Instead, it should drop the relevant
        /// <see cref="Possibility"/>s that are no longer possible.
        /// 
        /// Note: See <see cref="ExactCoverGraph"/> to understand how the graph works.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="graph">The exact-cover graph to constrain.</param>
        /// <returns>
        /// False if the constraint could not be satisfied by the given puzzle, else true.
        /// </returns>
        bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph graph);
    }
}