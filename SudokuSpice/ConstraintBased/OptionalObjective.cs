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

        private readonly int _totalCountToSatisfy;
        private readonly HashSet<IObjective> _requiredObjectives = new();
        private readonly Stack<Link> _previousFirstPossibilityLinks = new();
        private int _possibleObjectiveCount;
        private int _possibilityCount;
        private int _selectedCount;
        private Operation _currentOperation = Operation.NONE;
        private NodeState _state;
        private Link? _toPossibility;
        private Link? _toObjective;
        private Link? _linkThatCausedDrop;

        private bool _AllPossibilitiesAreRequired => _possibilityCount == _totalCountToSatisfy;
        private bool _IsSatisfied => _selectedCount == _totalCountToSatisfy;
        private int _CountUnknown => _possibilityCount - _selectedCount;
        internal int TotalCountToSatisfy => _totalCountToSatisfy;

        bool IObjective.IsRequired => false;
        IReadOnlySet<IObjective> IObjective.RequiredObjectives => _requiredObjectives;

        public NodeState State => _state;

        private OptionalObjective(int countToSatisfy)
        {
            _totalCountToSatisfy = countToSatisfy;
            _state = NodeState.UNKNOWN;
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
            if (_state != NodeState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {_state}.");
            }
            ++_possibleObjectiveCount;
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

        bool IPossibility.TryNotifyDroppedFromObjective(Link toDetach)
        {
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            Debug.Assert(_possibleObjectiveCount > 0,
                $"Cannot detach an objective from an optional objective with {nameof(_possibleObjectiveCount)} already equal to 0.");
            switch (State)
            {
                case NodeState.DROPPED:
                    --_possibleObjectiveCount;
                    return true;
                case NodeState.SELECTED:
                    // Can be dropped from any objective as long as it's not an important one.
                    if (!_IsObjectiveImportant(toDetach))
                    {
                        --_possibleObjectiveCount;
                        return true;
                    }
                    return false;
                default:
                    Debug.Assert(_toPossibility is not null,
                        "At least one possibility must be attached.");
                    if (_IsObjectiveImportant(toDetach))
                    {
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
                            toDetach => toDetach.Possibility.TryNotifyDroppedFromObjective(toDetach),
                            toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach)))
                        {
                            Links.RevertOthersOnPossibility(
                                toDetach,
                                toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                            _currentOperation = Operation.NONE;
                            return false;
                        }
                        _state = NodeState.DROPPED;
                        _linkThatCausedDrop = toDetach;
                        _currentOperation = Operation.NONE;
                    }
                    --_possibleObjectiveCount;
                    return true;
            }
        }

        void IPossibility.NotifyReattachedToObjective(Link toReattach)
        {
            ++_possibleObjectiveCount;
            switch (State)
            {
                case NodeState.DROPPED:
                    if (_linkThatCausedDrop != toReattach)
                    {
                        return;
                    }
                    Debug.Assert(_toPossibility is not null,
                        "At least one possibility must be attached.");
                    Debug.Assert(
                        _IsObjectiveImportant(toReattach),
                        $"Objective that caused drop was unimportant when reattaching it.");
                    _linkThatCausedDrop = null;
                    _currentOperation = Operation.RETURN;
                    Links.RevertOnObjective(
                        _toPossibility,
                        toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach));
                    Links.RevertOthersOnPossibility(
                        toReattach,
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                    _state = NodeState.UNKNOWN;
                    _currentOperation = Operation.NONE;
                    return;
                default:
                    if (_currentOperation == Operation.DROP
                        || _currentOperation == Operation.RETURN)
                    {
                        return;
                    }
                    Debug.Assert(
                        !_IsObjectiveImportant(toReattach),
                        $"Reattached important objective while in state {State} and performing operation {_currentOperation}.");
                    return;
            }
        }

        void IObjective.AppendPossibility(Link toNewPossibility)
        {
            if (_state != NodeState.UNKNOWN)
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
            Debug.Assert(_currentOperation == Operation.NONE);
            switch (State)
            {
                case NodeState.SELECTED:
                    return false;
                case NodeState.DROPPED:
                    ++_selectedCount;
                    if (_IsSatisfied)
                    {
                        --_selectedCount;
                        return false;
                    }
                    _PopPossibility(possibilityToSelect);
                    return true;
                default:
                    Debug.Assert(_toPossibility is not null,
                        "At least one possibility must be attached.");
                    Debug.Assert(_toObjective is not null,
                        "At least one objective must be attached.");
                    ++_selectedCount;
                    if (_IsSatisfied)
                    {
                        _currentOperation = Operation.SELECT;
                        if (!Links.TryUpdateOthersOnObjective(
                            possibilityToSelect,
                            toNotify => toNotify.Possibility.TryNotifyDroppedFromObjective(toNotify),
                            toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach)))
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
                                toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach));
                            --_selectedCount;
                            _currentOperation = Operation.NONE;
                            return false;
                        }
                        _state = NodeState.SELECTED;
                        _currentOperation = Operation.NONE;
                    }
                    _PopPossibility(possibilityToSelect);
                    return true;
            }
        }

        void IObjective.DeselectPossibility(Link possibilityToDeselect)
        {
            Debug.Assert(_currentOperation == Operation.NONE,
                $"Cannot deselect a possibility while performing operation {_currentOperation}.");
            Debug.Assert(
                _toPossibility is null ||
                !_toPossibility.GetLinksOnObjective().Contains(possibilityToDeselect),
                "Tried to unselect and reinsert a possibility that was already present.");
            switch (State)
            {
                case NodeState.UNKNOWN:
                case NodeState.DROPPED:
                    _ReinsertPossibility(possibilityToDeselect);
                    --_selectedCount;
                    return;
                case NodeState.SELECTED:
                    _ReinsertPossibility(possibilityToDeselect);
                    Debug.Assert(_toObjective is not null,
                        "At least one objective must be attached.");
                    _currentOperation = Operation.DESELECT;
                    _state = NodeState.UNKNOWN;
                    Links.RevertOnPossibility(
                        _toObjective,
                        toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
                    Links.RevertOthersOnObjective(
                        possibilityToDeselect,
                        toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach));
                    _currentOperation = Operation.NONE;
                    --_selectedCount;
                    return;
            }
        }

        bool IObjective.TryDropPossibility(Link toDrop)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(toDrop),
                "Tried to drop a possibility that's not connected to this objective.");
            if (State == NodeState.DROPPED ||
                State == NodeState.SELECTED ||
                _currentOperation == Operation.DROP ||
                _currentOperation == Operation.SELECT)
            {
                --_possibilityCount;
                // _PopPossibility(toDrop);
                return true;
            }
            Debug.Assert(_currentOperation == Operation.NONE);
            if (_AllPossibilitiesAreRequired)
            {
                _currentOperation = Operation.DROP;
                if (!Links.TryUpdateOthersOnObjective(
                    toDrop,
                    toDetach => toDetach.Possibility.TryNotifyDroppedFromObjective(toDetach),
                    toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach)))
                {
                    _currentOperation = Operation.NONE;
                    return false;
                }
                if (!Links.TryUpdateOnPossibility(
                    _toObjective!,
                    toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                    toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                {
                    Links.RevertOthersOnObjective(
                        toDrop,
                        toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach));
                    _currentOperation = Operation.NONE;
                    return false;
                }
                _state = NodeState.DROPPED;
                _linkThatCausedDrop = toDrop;
                _currentOperation = Operation.NONE;
            }
            _PopPossibility(toDrop);
            --_possibilityCount;
            return true;
        }

        void IObjective.ReturnPossibility(Link toReturn)
        {
            ++_possibilityCount;
            if ((State == NodeState.DROPPED
                && _linkThatCausedDrop != toReturn)
                || State == NodeState.SELECTED
                || _currentOperation != Operation.NONE)
            {
                return;
            }
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toReturn) ?? true,
                "Tried to return a possibility that's already connected to this objective.");
            _ReinsertPossibility(toReturn);
            if (_linkThatCausedDrop != toReturn)
            {
                return;
            }
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            _currentOperation = Operation.RETURN;
            _linkThatCausedDrop = null;
            _state = NodeState.UNKNOWN;
            Links.RevertOnPossibility(
                _toObjective,
                toReturn => toReturn.Objective.ReturnPossibility(toReturn));
            Links.RevertOthersOnObjective(
                toReturn,
                toReattach => toReattach.Possibility.NotifyReattachedToObjective(toReattach));
            _currentOperation = Operation.NONE;
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
            Debug.Assert(_currentOperation == Operation.NONE);
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
            Debug.Assert(_currentOperation == Operation.NONE);
            _toPossibility = _previousFirstPossibilityLinks.Pop();
            toReinsert.ReinsertToObjective();
        }

        private bool _IsObjectiveImportant(Link toObjective)
        {
            return toObjective.Objective.IsRequired ||
                _possibleObjectiveCount == 1 ||
                Objectives.LinksToUniqueRequiredObjective(toCheck: toObjective, toIterate: _toObjective!);
        }
    }
}
