using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Defines an objective in the <see cref="ExactCoverGraph"/> graph.
    /// </summary>
    public interface IObjective
    {
        /// <summary>
        /// The current state of this node in the <see cref="ExactCoverGraph"/>.
        /// </summary>
        public NodeState State { get; }

        /// <summary>
        /// Retrieves any possibilities that are direct descendents of this objective and whose
        /// states are unknown.
        /// </summary>
        public IEnumerable<IPossibility> GetUnknownDirectPossibilities();

        /// <summary>
        /// Whether or not this objective is required.
        /// </summary>
        internal bool IsRequired { get; }

        /// <summary>
        /// Appends the given link to this objective, including updating the next and previous
        /// links as necessary to maintain a valid doubly linked list.
        /// </summary>
        /// <param name="toNewPossibility">
        /// The link to the new possibility to connect to this objective.
        /// </param>
        internal void AppendPossibility(Link toNewPossibility);

        /// <summary>
        /// Tries to select the linked possibility on this objective. This should be called by the
        /// linked possibility.
        /// </summary>
        internal bool TrySelectPossibility(Link toSelect);

        /// <summary>
        /// Tries to deselect the linked possibility on this objective. This should be called by the
        /// linked possibility.
        ///
        /// The possibility must have already been selected.
        /// </summary>
        internal void DeselectPossibility(Link toDeselect);

        /// <summary>
        /// Tries to drop the linked possibility from this objective. This should be called by the
        /// linked possiblity.
        /// </summary>
        internal bool TryDropPossibility(Link toDrop);

        /// <summary>
        /// Returns the linked possibility to this objective. This should be called by the linked
        /// possibility.
        ///
        /// The possibility must have already been dropped from this objective.
        /// </summary>
        internal void ReturnPossibility(Link toReturn);
    }
}