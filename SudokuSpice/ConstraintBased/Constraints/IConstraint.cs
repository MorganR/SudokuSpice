namespace SudokuSpice.ConstraintBased.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Adds necessary <see cref="ConstraintHeader"/>s and links to the given
        /// <paramref name="matrix"/>.
        /// </summary>
        /// <remarks>
        /// This skips adding <c>ConstraintHeaders</c> that are already satisfied by the given
        /// <paramref name="puzzle"/>. Instead, it drops the corresponding
        /// <see cref="PossibleValue"/>s that would have been included in these headers.
        /// 
        /// Note: See <see cref="ExactCoverMatrix"/> to understand how the matrix works.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="matrix">The exact-cover matrix to constrain.</param>
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix);
    }
}