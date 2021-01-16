namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Indicates if a <see cref="Possibility"/> is still possible, selected, or dropped.
    /// </summary>
    public enum PossibilityState
    {
        /// <summary>
        /// This <see cref="Possibility"/> is still possible.
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// This <see cref="Possibility"/> has been selected.
        /// </summary>
        SELECTED,
        /// <summary>
        /// This <see cref="Possibility"/> has been dropped, i.e. it is no longer possible
        /// with the currently selected values.
        /// </summary>
        DROPPED
    }
}