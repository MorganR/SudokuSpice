namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Defines a possibility in the <see cref="ExactCoverGraph"/>.
    /// </summary>
    public interface IPossibility
    {
        /// <summary>
        /// True if this possibility is a concrete possibility, i.e. a leaf node in the graph, as
        /// opposed to a group of other possibilities.
        /// </summary>
        public bool IsConcrete { get; }

        /// <summary>
        /// The current state of this node in the <see cref="ExactCoverGraph"/>.
        /// </summary>
        public NodeState State { get; }
        /// <summary>
        /// Appends the linked objective to this possibility.
        /// </summary>
        internal void AppendObjective(Link toNewObjective);
        /// <summary>
        /// Undrops this possibility. This must be called from the linked objective.
        ///
        /// This possibility must have been previously dropped from the objective.
        /// </summary>
        internal void ReturnFromObjective(Link returnSource);
        /// <summary>
        /// Tries to drop this possibility. This must be called from the linked objective.
        /// </summary>
        /// <param name="dropSource">
        /// Links to the objective that is causing this possibility to be dropped.
        /// </param>
        internal bool TryDropFromObjective(Link dropSource);
    }
}