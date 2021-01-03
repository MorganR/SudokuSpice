namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Indicates if a <see cref="PossibleSquareValue"/> is still possible, selected, or dropped.
    /// </summary>
    public enum PossibleValueState
    {
        /// <summary>
        /// This <see cref="PossibleSquareValue"/> is still possible.
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// This <see cref="PossibleSquareValue"/> has been selected.
        /// </summary>
        SELECTED,
        /// <summary>
        /// This <see cref="PossibleSquareValue"/> has been dropped, i.e. it is no longer possible
        /// with the currently selected values.
        /// </summary>
        DROPPED
    }
}