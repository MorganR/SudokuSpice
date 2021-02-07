using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.ConstraintBased
{
    public class Possibility : IPossibility
    {
        private enum Operation
        {
            NONE,
            SELECT,
            DESELECT,
            DROP,
            RETURN,
        }

        private int _possibleObjectiveCount;
        private Link? _toObjective;
        private Operation _currentOperation = Operation.NONE;
        private Link? _objectiveThatCausedDrop;

        public Coordinate Coordinate { get; }
        public int Index { get; }
        public NodeState State { get; private set; }

        internal Possibility(Coordinate location, int valueIndex)
        {
            _possibleObjectiveCount = 0;
            Coordinate = location;
            Index = valueIndex;
            State = NodeState.UNKNOWN;
        }

        void IPossibility.AppendObjective(Link toNewObjective)
        {
            if (State != NodeState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {State}.");
            }
            ++_possibleObjectiveCount;
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

        bool IPossibility.TryDropFromObjective(Link dropSource)
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(_possibleObjectiveCount > 0,
                $"Cannot be dropped from an objective with {nameof(_possibleObjectiveCount)} already equal to 0.");
            switch (State)
            {
                case NodeState.DROPPED:
                    --_possibleObjectiveCount;
                    return true;
                case NodeState.SELECTED:
                    return false;
                default:
                    if (_currentOperation == Operation.DROP)
                    {
                        --_possibleObjectiveCount;
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
                    --_possibleObjectiveCount;
                    return true;
            }
        }

        void IPossibility.ReturnFromObjective(Link toReattach)
        {
            ++_possibleObjectiveCount;
            switch (State)
            {
                case NodeState.DROPPED:
                    if (_objectiveThatCausedDrop != toReattach)
                    {
                        return;
                    }
                    _objectiveThatCausedDrop = null;
                    _currentOperation = Operation.RETURN;
                    Links.RevertOthersOnPossibility(
                        toReattach,
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
