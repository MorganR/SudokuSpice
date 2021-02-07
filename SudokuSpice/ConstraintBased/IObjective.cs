using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased
{
    public interface IObjective
    {
        public NodeState State { get; }

        internal bool IsRequired { get; }

        /// <summary>
        /// Appends the given link to this objective, including updating the next and previous
        /// links as necessary to maintain a valid doubly linked list.
        /// </summary>
        /// <param name="toNewPossibility">
        /// The link to the new possibility to connect to this objective.
        /// </param>
        internal void AppendPossibility(Link toNewPossibility);

        internal IEnumerable<IPossibility> GetUnknownDirectPossibilities();

        internal bool TrySelectPossibility(Link toSelect);

        internal void DeselectPossibility(Link toDeselect);

        internal bool TryDropPossibility(Link toDrop);

        internal void ReturnPossibility(Link toReturn);
    }
}
