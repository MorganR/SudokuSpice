using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        private readonly Stack<Link> _previousFirstObjectiveLinks = new();
        private int _objectiveCount;
        private Link? _toObjective;
        private Operation _currentOperation = Operation.NONE;

        public Coordinate Coordinate { get; }
        public int Index { get; }
        public PossibilityState State { get; private set; }

        internal Possibility(Coordinate location, int valueIndex)
        {
            _objectiveCount = 0;
            Coordinate = location;
            Index = valueIndex;
            State = PossibilityState.UNKNOWN;
        }

        void IPossibility.AppendObjective(Link toNewObjective)
        {
            if (State != PossibilityState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {State}.");
            }
            ++_objectiveCount;
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
            if (State == PossibilityState.DROPPED)
            {
                return true;
            }
            if (State != PossibilityState.UNKNOWN)
            {
                return false;
            }
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
            State = PossibilityState.DROPPED;
            return true;
        }

        internal bool TrySelect()
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(State == PossibilityState.UNKNOWN,
                $"Cannot select a possibility in state {State}.");
            _currentOperation = Operation.SELECT;
            if (!Links.TryUpdateOnPossibility(
                _toObjective,
                toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
            {
                _currentOperation = Operation.NONE;
                return false;
            }
            State = PossibilityState.SELECTED;
            _currentOperation = Operation.NONE;
            return true;
        }

        internal void Deselect()
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(State == PossibilityState.SELECTED,
                $"Cannot deselect a possibility in state {State}.");
            _currentOperation = Operation.DESELECT;
            State = PossibilityState.UNKNOWN;
            Links.RevertOnPossibility(
                _toObjective,
                toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
            _currentOperation = Operation.NONE;
        }

        bool IPossibility.TryDetachObjective(Link toDetach)
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(_objectiveCount > 0,
                $"Cannot drop an objective from a possibility with {nameof(_objectiveCount)} already equal to 0.");
            Debug.Assert(State == PossibilityState.UNKNOWN,
                $"Cannot drop an objective from a possibility in state {State}.");
            if (_currentOperation != Operation.NONE)
            {
                return _currentOperation == Operation.DROP || !_IsObjectiveImportant(toDetach);
            }
            if (_IsObjectiveImportant(toDetach))
            {
                _currentOperation = Operation.DROP;
                if (!Links.TryUpdateOthersOnPossibility(
                    toDetach,
                    toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                {
                    _currentOperation = Operation.NONE;
                    return false;
                }
                State = PossibilityState.DROPPED;
                _currentOperation = Operation.NONE;
            }
            // Just detach the objective.
            _PopObjective(toDetach);
            --_objectiveCount;
            return true;
        }

        void IPossibility.ReattachObjective(Link toReattach)
        {
            Debug.Assert(State != PossibilityState.SELECTED,
                $"Cannot reattach an objective to a possibility in state {State}.");
            if (_currentOperation != Operation.NONE)
            {
                return;
            }
            Debug.Assert(!_toObjective?.GetLinksOnPossibility().Contains(toReattach) ?? true,
                "Can't reattach already attached objective to possibility.");
            ++_objectiveCount;
            _ReinsertObjective(toReattach);
            if (State == PossibilityState.DROPPED && _currentOperation == Operation.NONE)
            {
                _currentOperation = Operation.RETURN;
                Links.RevertOthersOnPossibility(
                    toReattach,
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                State = PossibilityState.UNKNOWN;
                _currentOperation = Operation.NONE;
            }
        }

        private void _PopObjective(Link toPop)
        {
            toPop.PopFromPossibility();
            _previousFirstObjectiveLinks.Push(_toObjective!);
            if (_toObjective == toPop)
            {
                _toObjective = toPop.NextOnPossibility;
                if (_toObjective == toPop)
                {
                    _toObjective = null;
                }
            }
        }

        private void _ReinsertObjective(Link toReinsert)
        {
            _toObjective = _previousFirstObjectiveLinks.Pop();
            toReinsert.ReinsertToPossibility();
        }

        private bool _IsObjectiveImportant(Link toObjective)
        {
            return toObjective.Objective.IsRequired ||
                _objectiveCount == 1 ||
                Objectives.LinksToUniqueRequiredObjective(toCheck: toObjective, toIterate: _toObjective!);
        }
    }
}
