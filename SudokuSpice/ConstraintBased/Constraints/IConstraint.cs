namespace SudokuSpice.ConstraintBased.Constraints
{
    public interface IConstraint<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        /// <summary>
        /// Adds necessary <see cref="ConstraintHeader{TPuzzle}"/>s and links to the given
        /// <paramref name="matrix"/>.
        /// </summary>
        /// <remarks>
        /// This skips adding <c>ConstraintHeader</c>s that are already satisfied by the given
        /// <paramref name="puzzle"/>. Instead, it drops the corresponding
        /// <see cref="PossibleSquareValue{TPuzzle}"/>s that would have been included in these headers.
        /// 
        /// Note: See <see cref="ExactCoverMatrix{TPuzzle}"/> to understand how the matrix works.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="matrix">The exact-cover matrix to constrain.</param>
        public void Constrain(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix);
    }
}
