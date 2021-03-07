using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// A concrete possibility in the <see cref="ExactCoverGraph"/>. This represents a single
    /// possible value for a single square in the puzzle.
    /// </summary>
    public sealed class Possibility : IPossibility
    {
        private enum Operation
        {
            NONE,
            SELECT,
            DESELECT,
            DROP,
            RETURN,
        }

        private Link? _toObjective;
        private Operation _currentOperation = Operation.NONE;
        private Link? _objectiveThatCausedDrop;

        /// <summary>
        /// The square this possibility is for.
        /// </summary>
        public Coordinate Coordinate { get; }
        /// <summary>
        /// The value index this possibility is for (corresponds to the values in
        /// <see cref="ExactCoverGraph.AllPossibleValues"/>).
        /// </summary>
        public int Index { get; }
        /// <inheritdoc />
        public bool IsConcrete => true;
        /// <inheritdoc />
        public NodeState State { get; private set; }

        internal Possibility(Coordinate location, int valueIndex)
        {
            Coordinate = location;
            Index = valueIndex;
            State = NodeState.UNKNOWN;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IPossibility.AppendObjective(Link toNewObjective)
        {
            if (State != NodeState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {State}.");
            }
            if (_toObjective is null)
            {
                _toObjective = toNewObjective;
                return;
            }
            _toObjective.PrependToPossibility(toNewObjective);
        }

        /// <summary>
        /// Tries to drop this possibility, if that is compatible with existing objectives.
        ///
        /// This is meant to be checked during matrix construction so that constraints can drop
        /// possibilities that violate the puzzle's preset values.
        ///
        /// If this method fails, it suggests that the current puzzle cannot satisfy all the current
        /// constraints.
        /// </summary>
        /// <returns>
        /// True if the possibility is already dropped or was successfully dropped from all
        /// existing objectives, else false.
        /// </returns>
        public bool TryDrop()
        {
            switch (State)
            {
                case NodeState.DROPPED:
                    return true;
                case NodeState.SELECTED:
                    return false;
                default:
                    if (_toObjective is not null)
                    {
                        _currentOperation = Operation.DROP;
                        if (!Links.TryUpdateOnPossibility(
                            _toObjective,
                            toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                            toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                        {
                            _currentOperation = Operation.NONE;
                            return false;
                        }
                        _currentOperation = Operation.NONE;
                    }
                    State = NodeState.DROPPED;
                    return true;
            }
        }

        /// <summary>
        /// Tries to select this possibility and update the graph accordingly.
        /// </summary>
        /// <remarks>
        /// This is a safe operation. If it returns false, then no lasting changes have been made to
        /// the graph.
        /// </remarks>
        /// <returns>
        /// True if this succeeds and all objectives are/can still be satisfied, else false.
        /// </returns>
        internal bool TrySelect()
        {
            switch (State)
            {
                case NodeState.SELECTED:
                    return true;
                case NodeState.DROPPED:
                    return false;
                default:
                    Debug.Assert(_toObjective is not null,
                        "At least one objective must be attached.");
                    _currentOperation = Operation.SELECT;
                    if (!Links.TryUpdateOnPossibility(
                        _toObjective,
                        toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                        toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
                    {
                        _currentOperation = Operation.NONE;
                        return false;
                    }
                    State = NodeState.SELECTED;
                    _currentOperation = Operation.NONE;
                    return true;
            }
        }

        /// <summary>
        /// Deselects this possibility and updates the graph accordingly.
        ///
        /// This possibility must be selected prior to calling this method.
        /// </summary>
        internal void Deselect()
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(State == NodeState.SELECTED,
                $"Cannot deselect a possibility in state {State}.");
            _currentOperation = Operation.DESELECT;
            State = NodeState.UNKNOWN;
            Links.RevertOnPossibility(
                _toObjective,
                toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
            _currentOperation = Operation.NONE;
        }

        /// <inheritdoc />
        bool IPossibility.TryDropFromObjective(Link dropSource)
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            switch (State)
            {
                case NodeState.DROPPED:
                    return true;
                case NodeState.SELECTED:
                    return false;
                default:
                    if (_currentOperation == Operation.DROP)
                    {
                        return true;
                    } else if (_currentOperation == Operation.SELECT)
                    {
                        return false;
                    }
                    Debug.Assert(_currentOperation == Operation.NONE);
                    _currentOperation = Operation.DROP;
                    if (!Links.TryUpdateOthersOnPossibility(
                        dropSource,
                        toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                    {
                        _currentOperation = Operation.NONE;
                        return false;
                    }
                    State = NodeState.DROPPED;
                    _objectiveThatCausedDrop = dropSource;
                    _currentOperation = Operation.NONE;
                    return true;
            }
        }

        /// <inheritdoc />
        void IPossibility.ReturnFromObjective(Link returnSource)
        {
            switch (State)
            {
                case NodeState.DROPPED:
                    if (_objectiveThatCausedDrop != returnSource)
                    {
                        return;
                    }
                    _objectiveThatCausedDrop = null;
                    _currentOperation = Operation.RETURN;
                    Links.RevertOthersOnPossibility(
                        returnSource,
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                    State = NodeState.UNKNOWN;
                    _currentOperation = Operation.NONE;
                    return;
                default:
                    Debug.Assert(State == NodeState.UNKNOWN);
                    Debug.Assert(_currentOperation == Operation.NONE
                        || _currentOperation == Operation.RETURN);
                    return;
            }
        }
    }
}