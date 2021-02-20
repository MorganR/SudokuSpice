using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// An optional objective in an <see cref="ExactCoverGraph"/>. An optional objective is both
    /// an <see cref="IObjective"/> and an <see cref="IPossibility"/>.
    ///
    /// This can be satisfied if one or more attached <see cref="IPossibility"/> objects are
    /// selected. Once satisfied, this drops any remaining unknown possibilities, and notifies
    /// parent objectives that to select this possibility.
    ///
    /// Unlike <see cref="Objective"/>s, this class can be dropped. Dropping this objetive does
    /// not immediately impact attached possibilities. These possibilities may still be dropped
    /// or selected, as long as this objective does not become satisfied. If selecting a
    /// possibility would satisfy a dropped optional objective, then the call to
    /// <see cref="IObjective.TrySelectPossibility(Link)"/> will fail.
    /// 
    /// All <c>OptionalObjective</c> objects must eventually be descendents of at least one
    /// <see cref="Objective"/>.
    /// </summary>
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
        private readonly Stack<Link> _previousFirstPossibilityLinks;
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
        /// <summary>
        /// The total count of possibilities that must be selected in order to satisfy this
        /// objective.
        /// </summary>
        internal int TotalCountToSatisfy => _totalCountToSatisfy;
        /// <summary>
        /// The number of possibilities that must still be selected in order to satisfy this
        /// objective.
        /// </summary>
        internal int RemainingCountToSatisfy => _totalCountToSatisfy - _selectedCount;

        /// <inheritdoc />
        bool IObjective.IsRequired => false;

        /// <inheritdoc />
        bool IPossibility.IsConcrete => false;
        /// <inheritdoc />
        public NodeState State => _state;

        private OptionalObjective(int countToSatisfy, int expectedPossibilitiesCount)
        {
            _totalCountToSatisfy = countToSatisfy;
            _state = NodeState.UNKNOWN;
            _previousFirstPossibilityLinks = new Stack<Link>(expectedPossibilitiesCount);
        }

        /// <summary>
        /// Creates an optional objective with the given possibilities attached. This objective
        /// will be satisfied when <paramref name="countToSatisfy"/> of these possibilities are
        /// selected.
        /// 
        /// Note that this results in an <see cref="OptionalObjective"/> with no parent objectives.
        /// The returned objective must eventually be connected to at least one top-level required
        /// <see cref="Objective"/> object in order to behave correctly.
        /// </summary>
        /// <param name="possibilities">The possibilities that could satisfy this objective.</param>
        /// <param name="countToSatisfy">
        /// The number of possibilities that must be selected to satisfy this objective.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="countToSatisfy"/> is less than 1 or is impossible with the
        /// given number of <paramref name="possibilities"/>.
        /// </exception>
        /// <returns>The newly constructed optional objective.</returns>
        public static OptionalObjective CreateWithPossibilities(ReadOnlySpan<IPossibility> possibilities, int countToSatisfy)
        {
            if (countToSatisfy < 1 || countToSatisfy > possibilities.Length)
            {
                throw new ArgumentException($"{nameof(countToSatisfy)} must be in the inclusive range [1, {nameof(possibilities)}.Length]. Received: {countToSatisfy} with {possibilities.Length} possibilities.");
            }
            var objective = new OptionalObjective(countToSatisfy, possibilities.Length);
            foreach (var possibility in possibilities)
            {
                Link.CreateConnectedLink(possibility, objective);
            }
            return objective;
        }

        /// <inheritdoc />
        IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities()
        {
            if (_toPossibility is null)
            {
                return Enumerable.Empty<IPossibility>();
            }
            return _toPossibility.GetLinksOnObjective().Select(link => link.Possibility);
        }

        /// <inheritdoc />
        void IPossibility.AppendObjective(Link toNewObjective)
        {
            if (_state != NodeState.UNKNOWN)
            {
                throw new InvalidOperationException($"Can't append a new objective to a possibility in state {_state}.");
            }
            ++_possibleObjectiveCount;
            if (_toObjective is null)
            {
                _toObjective = toNewObjective;
                return;
            }
            _toObjective.PrependToPossibility(toNewObjective);
        }

        /// <inheritdoc />
        bool IPossibility.TryDropFromObjective(Link dropSource)
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
                    return false;
                default:
                    Debug.Assert(_toPossibility is not null,
                        "At least one possibility must be attached.");
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
                        dropSource,
                        toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                    {
                        _currentOperation = Operation.NONE;
                        return false;
                    }
                    _state = NodeState.DROPPED;
                    _linkThatCausedDrop = dropSource;
                    _currentOperation = Operation.NONE;
                    --_possibleObjectiveCount;
                    return true;
            }
        }

        /// <inheritdoc />
        void IPossibility.ReturnFromObjective(Link returnSource)
        {
            ++_possibleObjectiveCount;
            switch (State)
            {
                case NodeState.DROPPED:
                    if (_linkThatCausedDrop != returnSource)
                    {
                        return;
                    }
                    Debug.Assert(_toPossibility is not null,
                        "At least one possibility must be attached.");
                    _linkThatCausedDrop = null;
                    _currentOperation = Operation.RETURN;
                    Links.RevertOthersOnPossibility(
                        returnSource,
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn));
                    _state = NodeState.UNKNOWN;
                    _currentOperation = Operation.NONE;
                    return;
                default:
                    Debug.Assert(State == NodeState.UNKNOWN);
                    Debug.Assert(_currentOperation == Operation.NONE
                        || _currentOperation == Operation.RETURN);
                    return;
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                        if (!Links.TryUpdateOnPossibility(
                            _toObjective,
                            toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                            toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
                        {
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

        /// <inheritdoc />
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
                    _currentOperation = Operation.NONE;
                    --_selectedCount;
                    return;
            }
        }

        /// <inheritdoc />
        bool IObjective.TryDropPossibility(Link toDrop)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(toDrop),
                "Tried to drop a possibility that's not connected to this objective.");
            if (State == NodeState.DROPPED ||
                State == NodeState.SELECTED)
            {
                --_possibilityCount;
                _PopPossibility(toDrop);
                return true;
            }
            if (_currentOperation == Operation.DROP ||
                _currentOperation == Operation.SELECT)
            {
                --_possibilityCount;
                // Don't pop the possibility in case we're currently operating over them during
                // a drop/select operation.
                return true;
            }
            Debug.Assert(_currentOperation == Operation.NONE);
            if (_AllPossibilitiesAreRequired)
            {
                _currentOperation = Operation.DROP;
                // _toObjective can be null if we drop this during setup because it turns out to
                // be impossible at higher levels.
                if (_toObjective is not null)
                {
                    if (!Links.TryUpdateOnPossibility(
                        _toObjective,
                        toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                        toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
                    {
                        _currentOperation = Operation.NONE;
                        return false;
                    }

                }
                _state = NodeState.DROPPED;
                _linkThatCausedDrop = toDrop;
                _currentOperation = Operation.NONE;
            }
            _PopPossibility(toDrop);
            --_possibilityCount;
            return true;
        }

        /// <inheritdoc />
        void IObjective.ReturnPossibility(Link toReturn)
        {
            ++_possibilityCount;
            if (State != NodeState.DROPPED ||
                _linkThatCausedDrop != toReturn)
            {
                _ReinsertPossibility(toReturn);
                return;
            }
            if (_currentOperation != Operation.NONE)
            {
                return;
            }
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toReturn) ?? true,
                "Tried to return a possibility that's already connected to this objective.");
            _ReinsertPossibility(toReturn);
            Debug.Assert(_toObjective is not null,
                "At least one objective must be attached.");
            _currentOperation = Operation.RETURN;
            _linkThatCausedDrop = null;
            _state = NodeState.UNKNOWN;
            Links.RevertOnPossibility(
                _toObjective,
                toReturn => toReturn.Objective.ReturnPossibility(toReturn));
            _currentOperation = Operation.NONE;
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
    }
}
