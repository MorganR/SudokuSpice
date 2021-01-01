namespace SudokuSpice.ConstraintBased.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Adds necessary <see cref="ConstraintHeader"/>s and links to the given
        /// <paramref name="matrix"/>.
        /// </summary>
        /// <remarks>
        /// This should skip adding <c>ConstraintHeaders</c> that are already satisfied by the given
        /// <paramref name="puzzle"/>. Instead, it should drop the corresponding
        /// <see cref="PossibleSquareValue"/>s that would have been included in these headers.
        /// 
        /// Note: See <see cref="ExactCoverMatrix"/> to understand how the matrix works.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="matrix">The exact-cover matrix to constrain.</param>
        void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix);
    }
}