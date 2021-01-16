namespace SudokuSpice.ConstraintBased.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Adds necessary <see cref="Requirement"/>s and links to the given
        /// <paramref name="matrix"/> in order to solve the given <paramref name="puzzle"/>
        /// according to this constraint. The details here are implementation-specific.
        /// </summary>
        /// <remarks>
        /// This should skip adding requirements that are already satisfied by the given
        /// <paramref name="puzzle"/>. Instead, it should drop the corresponding
        /// <see cref="Possibility"/>s that would have been included in these requirements.
        /// 
        /// Note: See <see cref="ExactCoverMatrix"/> to understand how the matrix works.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="matrix">The exact-cover matrix to constrain.</param>
        /// <returns>
        /// False if the constraint could not be satisfied by the given puzzle, else true.
        /// </returns>
        bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix);
    }
}