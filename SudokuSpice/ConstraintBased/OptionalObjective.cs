using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    public class OptionalObjective : IOptionalObjective
    {
        private enum Operation
        {
            NONE,
            SELECT,
            DESELECT,
            DROP,
            RETURN,
        }

        private enum DropCause
        {
            UNKNOWN,
            DROPPED_POSSIBILITY,
            DETACHED_OBJECTIVE,
        }

        private readonly int _totalCountToSatisfy;
        private readonly HashSet<IObjective> _requiredObjectives = new();
        private readonly Stack<Link> _previousFirstPossibilityLinks = new();
        private readonly Stack<Link> _previousFirstObjectiveLinks = new();
        private int _objectiveCount;
        private int _possibilityCount;
        private int _selectedCount;
        private Operation _currentOperation = Operation.NONE;
        private DropCause _dropCause = DropCause.UNKNOWN;
        private PossibilityState _state;
        private Link? _toPossibility;
        private Link? _toObjective;

        private bool _AllPossibilitiesAreRequired => _possibilityCount == _totalCountToSatisfy;
        private bool _IsSatisfied => _selectedCount == _totalCountToSatisfy;
        private int _CountUnknown => _possibilityCount - _selectedCount;
        internal int TotalCountToSatisfy => _totalCountToSatisfy;
        // TODO: Make this part of IPossibility
        internal PossibilityState State => _state;

        bool IObjective.IsRequired => false;
        IReadOnlySet<IObjective> IObjective.RequiredObjectives => _requiredObjectives;

        private OptionalObjective(int countToSatisfy)
        {
            _totalCountToSatisfy = countToSatisfy;
            _state = PossibilityState.UNKNOWN;
        }

        public static OptionalObjective CreateWithPossibilities(ReadOnlySpan<IPossibility> possibilities, int countToSatisfy)
        {
            if (countToSatisfy < 1 || countToSatisfy > possibilities.Length)
            {
                throw new ArgumentException($"{nameof(countToSatisfy)} must be in the inclusive range [1, {nameof(possibilities)}.Length].");
            }
            var objective = new OptionalObjective(countToSatisfy);
            foreach (var possibility in possibilities)
            {
                Link.CreateConnectedLink(possibility, objective);
            }
            return objective;
        }

        IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities()
        {
            if (_toPossibility is null)
            {
                return Enumerable.Empty<IPossibility>();
            }
            return _toPossibility.GetLinksOnObjective().Select(link => link.Possibility);
        }

        void IPossibility.AppendObjective(Link toNewObjective)
        {
            if (_state != PossibilityState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {_state}.");
            }
            ++_objectiveCount;
            if (toNewObjective.Objective.IsRequired)
            {
                ((IOptionalObjective)this).RecordRequiredObjective(toNewObjective.Objective);
            }
            if (_toObjective is null)
            {
                _toObjective = toNewObjective;
                return;
            }
            _toObjective.PrependToPossibility(toNewObjective);
        }

        bool IPossibility.TryDetachObjective(Link toDetach)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(_objectiveCount > 0,
                $"Cannot detach an objective from an optional objective with {nameof(_objectiveCount)} already equal to 0.");
            if (_currentOperation != Operation.NONE)
            {
                Debug.Assert(_currentOperation == Operation.SELECT
                    || _currentOperation == Operation.DROP);
                return _currentOperation == Operation.DROP || !_IsObjectiveImportant(toDetach);
            }
            Debug.Assert(_state == PossibilityState.UNKNOWN,
                $"Cannot detach an objective from an optional objective in state {_state}.");
            if (_IsObjectiveImportant(toDetach))
            {
                _currentOperation = Operation.DROP;
                // Completely drop this as a possibility and detach the objective.
                if (!Links.TryUpdateOthersOnPossibility(
                    toDetach,
                    toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                {
                    _currentOperation = Operation.NONE;
                    return false;
                }
                if (!Links.TryUpdateOnObjective(
                    _toPossibility,
                    toDetach => toDetach.Possibility.TryDetachObjective(toDetach),
                    toReattach => toReattach.Possibility.ReattachObjective(toReattach)))
                {
                    Links.RevertOthersOnPossibility(
                        toDetach,
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                    _currentOperation = Operation.NONE;
                    return false;
                }
                _state = PossibilityState.DROPPED;
                _dropCause = DropCause.DETACHED_OBJECTIVE;
                _currentOperation = Operation.NONE;
            }
            _PopObjective(toDetach);
            --_objectiveCount;
            return true;
        }

        void IPossibility.ReattachObjective(Link toReattach)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_state != PossibilityState.SELECTED,
                "Shouldn't be reattaching an objective while selected.");
            if (_currentOperation != Operation.NONE)
            {
                return;
            }
            ++_objectiveCount;
            Debug.Assert(!_toObjective?.GetLinksOnPossibility().Contains(toReattach) ?? true,
                "Can't reattach already attached objective to this optional objective.");
            _ReinsertObjective(toReattach);
            if (_state != PossibilityState.DROPPED
                || _dropCause != DropCause.DETACHED_OBJECTIVE)
            {
                return;
            }
            _currentOperation = Operation.RETURN;
            Links.RevertOnObjective(
                _toPossibility,
                toReattach => toReattach.Possibility.ReattachObjective(toReattach));
            Links.RevertOthersOnPossibility(
                toReattach,
                toReturn => toReturn.Objective.ReturnPossibility(toReturn));
            _state = PossibilityState.UNKNOWN;
                _dropCause = DropCause.UNKNOWN;
            _currentOperation = Operation.NONE;
        }

        void IObjective.AppendPossibility(Link toNewPossibility)
        {
            if (_state != PossibilityState.UNKNOWN)
            {
                throw new InvalidOperationException(
                    $"Can't append a new possibility to an optional objective in state {_state}.");
            }
            ++_possibilityCount;
            if (_toPossibility is null)
            {
                _toPossibility = toNewPossibility;
                return;
            }
            _toPossibility.PrependToObjective(toNewPossibility);
        }

        bool IObjective.TrySelectPossibility(Link possibilityToSelect)
        {
            if (_state != PossibilityState.UNKNOWN)
            {
                // Fail if already selected or if dropped and selecting this possibility would cause
                // this objective to be satisfied.
                return _state == PossibilityState.DROPPED &&
                    (!_AllPossibilitiesAreRequired || _CountUnknown > 1);
            }
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            // Debug.Assert(!_IsSatisfied,
            //     "Cannot select a possibility on an already satisfied objective.");
            Debug.Assert(_currentOperation == Operation.NONE,
                $"Tried to select a possibility while performing operation {_currentOperation}.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(possibilityToSelect),
                "Tried to select a link that's not connected to this objective.");
            if (_state == PossibilityState.SELECTED ||
                // Or if selecting one more would satisfy this objective
                (_state == PossibilityState.DROPPED && _totalCountToSatisfy - _selectedCount == 1))
            {
                return false;
            }
            ++_selectedCount;
            if (_IsSatisfied && _state == PossibilityState.UNKNOWN)
            {
                _currentOperation = Operation.SELECT;
                if (!Links.TryUpdateOthersOnObjective(
                    possibilityToSelect,
                    toDetach => toDetach.Possibility.TryDetachObjective(toDetach),
                    toReattach => toReattach.Possibility.ReattachObjective(toReattach)))
                {
                    --_selectedCount;
                    _currentOperation = Operation.NONE;
                    return false;
                }
                if (!Links.TryUpdateOnPossibility(
                    _toObjective,
                    toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
                {
                    Links.RevertOthersOnObjective(
                        possibilityToSelect,
                        toReattach => toReattach.Possibility.ReattachObjective(toReattach));
                    --_selectedCount;
                    _currentOperation = Operation.NONE;
                    return false;
                }
                _state = PossibilityState.SELECTED;
                _currentOperation = Operation.NONE;
            }
            _PopPossibility(possibilityToSelect);
            return true;
        }

        void IObjective.DeselectPossibility(Link possibilityToDeselect)
        {
            if (_state == PossibilityState.DROPPED)
            {
                // TODO: Assert this should have returned "true" in TrySelect.
                return;
            }
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(_currentOperation == Operation.NONE,
                $"Cannot deselect a possibility while performing operation {_currentOperation}.");
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(possibilityToDeselect) ?? true,
                "Tried to unselect and reinsert a possibility that was already present.");
            _ReinsertPossibility(possibilityToDeselect);
            if (_state == PossibilityState.SELECTED && _IsSatisfied)
            {
                _currentOperation = Operation.DESELECT;
                _state = PossibilityState.UNKNOWN;
                Links.RevertOnPossibility(
                    _toObjective,
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
                Links.RevertOthersOnObjective(
                    possibilityToDeselect,
                    toReattach => toReattach.Possibility.ReattachObjective(toReattach));
                _currentOperation = Operation.NONE;
            }
            --_selectedCount;
        }

        bool IObjective.TryDropPossibility(Link toDrop)
        {
            if (_currentOperation == Operation.DROP)
            {
                return true;
            }
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_state == PossibilityState.DROPPED || _toObjective is not null,
                $"At least one objective must be attached if state is not DROPPED (found {_state}).");
            Debug.Assert(!_IsSatisfied,
                "Cannot drop a possibility from an already satisfied objective.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(toDrop),
                "Tried to drop a possibility that's not connected to this objective.");
            Debug.Assert(_currentOperation == Operation.NONE,
                $"Shouldn't be possible to call {nameof(IObjective.TryDropPossibility)} on an optional objective already performing {_currentOperation}");
            if (_state != PossibilityState.DROPPED && _AllPossibilitiesAreRequired)
            {
                _currentOperation = Operation.DROP;
                if (!Links.TryUpdateOthersOnObjective(
                    toDrop,
                    toDetach => toDetach.Possibility.TryDetachObjective(toDetach),
                    toReattach => toReattach.Possibility.ReattachObjective(toReattach)))
                {
                    _currentOperation = Operation.NONE;
                    return false;
                }
                if (!Links.TryUpdateOnPossibility(
                    _toObjective,
                    toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                {
                    Links.RevertOthersOnObjective(
                        toDrop,
                        toReattach => toReattach.Possibility.ReattachObjective(toReattach));
                    _currentOperation = Operation.NONE;
                    return false;
                }
                _state = PossibilityState.DROPPED;
                _dropCause = DropCause.DROPPED_POSSIBILITY;
                _currentOperation = Operation.NONE;
            }
            _PopPossibility(toDrop);
            --_possibilityCount;
            return true;
        }

        void IObjective.ReturnPossibility(Link toReturn)
        {
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toReturn) ?? true,
                "Tried to return a possibility that's already connected to this objective.");
            if (_currentOperation == Operation.RETURN)
            {
                return;
            }
            Debug.Assert(_currentOperation == Operation.NONE);
            ++_possibilityCount;
            _ReinsertPossibility(toReturn);
            if (_state == PossibilityState.DROPPED && _dropCause == DropCause.DROPPED_POSSIBILITY && _AllPossibilitiesAreRequired)
            {
                Debug.Assert(_toObjective is not null,
                    "At least one objective must be attached.");
                _currentOperation = Operation.RETURN;
                _state = PossibilityState.UNKNOWN;
                Links.RevertOnPossibility(
                    _toObjective,
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                Links.RevertOthersOnObjective(
                    toReturn,
                    toReattach => toReattach.Possibility.ReattachObjective(toReattach));
                _currentOperation = Operation.NONE;
                _dropCause = DropCause.UNKNOWN;
            }
        }

        void IOptionalObjective.RecordRequiredObjective(IObjective objective)
        {
            Debug.Assert(_toPossibility is not null,
                "Cannot add objectives before possibilities.");
            _requiredObjectives.Add(objective);
            // Forward this information to chidren, since objective tables are built from
            // possibility -> objective.
            foreach (var link in _toPossibility.GetLinksOnObjective())
            {
                // Skip self-link.
                if (link.Possibility is IOptionalObjective optionalObjective)
                {
                    optionalObjective.RecordRequiredObjective(objective);
                }
            }
        }

        private void _PopPossibility(Link toPop)
        {
            toPop.PopFromObjective();
            _previousFirstPossibilityLinks.Push(_toPossibility!);
            if (_toPossibility == toPop)
            {
                _toPossibility = toPop.NextOnObjective;
                if (_toPossibility == toPop)
                {
                    _toPossibility = null;
                }
            }
        }

        private void _ReinsertPossibility(Link toReinsert)
        {
            _toPossibility = _previousFirstPossibilityLinks.Pop();
            toReinsert.ReinsertToObjective();
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
