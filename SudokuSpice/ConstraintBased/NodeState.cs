namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Indicates if an <see cref="IPossibility"/> or <see cref="IObjective"/> is still possible,
    /// selected, or dropped.
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// This node is still possible.
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// This node has been selected/satisfied.
        /// </summary>
        SELECTED,
        /// <summary>
        /// This node has been dropped, i.e. it is no longer possible with the currently selected
        /// values.
        /// </summary>
        DROPPED
    }
}