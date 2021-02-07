using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    public class Objective : IObjective
    {
        private readonly int _countToSatisfy;
        private readonly ExactCoverMatrix _matrix;
        private readonly Stack<Link> _previousFirstPossibilityLinks = new();
        private int _possibilityCount;
        private int _selectedCount;
        private Link? _toPossibility;
        private bool _allPossibilitiesAreConcrete;
        private bool _atLeastOnePossibilityIsConcrete;
        private LinkedListNode<Objective>? _linkInMatrix;
        private NodeState _state;

        public NodeState State => _state;

        internal bool AllUnknownPossibilitiesAreConcrete
        {
            get {
                if (_allPossibilitiesAreConcrete)
                {
                    return true;
                }
                if (!_atLeastOnePossibilityIsConcrete || IsSatisfied || _toPossibility is null)
                {
                    return false;
                }
                foreach (var toUnknown in _toPossibility.GetLinksOnObjective())
                {
                    if (toUnknown.Possibility is not Possibility)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        internal bool AllPossibilitiesAreRequired => _possibilityCount == _countToSatisfy;
        internal bool IsSatisfied => _selectedCount == _countToSatisfy;
        internal int CountToSatisfy => _countToSatisfy;
        internal int CountUnknown => _possibilityCount - _selectedCount;

        bool IObjective.IsRequired => true;

        private Objective(ExactCoverMatrix matrix, int countToSatisfy)
        {
            _matrix = matrix;
            _countToSatisfy = countToSatisfy;
            _allPossibilitiesAreConcrete = true;
            _atLeastOnePossibilityIsConcrete = false;
            _state = NodeState.UNKNOWN;
        }

        public static Objective CreateFullyConnected(
            ExactCoverMatrix matrix,
            ReadOnlySpan<IPossibility> possibilities,
            int countToSatisfy)
        {
            if (countToSatisfy < 1 || countToSatisfy > possibilities.Length)
            {
                throw new ArgumentException($"{nameof(countToSatisfy)} must be in the inclusive range [1, {nameof(possibilities)}.Length].");
            }
            var objective = new Objective(matrix, countToSatisfy);
            foreach (var possibility in possibilities)
            {
                Link.CreateConnectedLink(possibility, objective);
            }
            objective._linkInMatrix = matrix.AttachObjective(objective);
            return objective;
        }

        void IObjective.AppendPossibility(Link toNewPossibility)
        {
            ++_possibilityCount;
            if (toNewPossibility.Possibility is not Possibility)
            {
                _allPossibilitiesAreConcrete = false;
            } else
            {
                _atLeastOnePossibilityIsConcrete = true;
            }
            if (_toPossibility is null)
            {
                _toPossibility = toNewPossibility;
                return;
            }
            _toPossibility.PrependToObjective(toNewPossibility);
        }

        IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities()
        {
            if (_toPossibility is null || IsSatisfied)
            {
                return Enumerable.Empty<IPossibility>();
            }
            return _toPossibility.GetLinksOnObjective()
                .Select(link => link.Possibility);
        }

        bool IObjective.TrySelectPossibility(Link toSelect)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(!IsSatisfied,
                "Can't select a possiblity for an already satisfied objective.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(toSelect),
                "Tried to select a link that's not connected to this objective.");
            ++_selectedCount;
            if (IsSatisfied)
            {
                if (!Links.TryUpdateOthersOnObjective(
                    toSelect,
                    toDetach => toDetach.Possibility.TryDropFromObjective(toDetach),
                    toReattach => toReattach.Possibility.ReturnFromObjective(toReattach)))
                {
                    --_selectedCount;
                    return false;
                }
                Debug.Assert(_linkInMatrix is not null,
                    $"{nameof(_linkInMatrix)} should be set during construction.");
                _matrix.DetachObjective(_linkInMatrix);
                _state = NodeState.SELECTED;
            }
            _PopPossibility(toSelect);
            return true;
        }

        void IObjective.DeselectPossibility(Link toDeselect)
        {
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toDeselect) ?? true,
                "Tried to deselect a link that's not connected to this objective.");
            _ReinsertPossibility(toDeselect);
            if (IsSatisfied)
            {
                _state = NodeState.UNKNOWN;
                Debug.Assert(_linkInMatrix is not null,
                    $"{nameof(_linkInMatrix)} should be set during construction.");
                _matrix.ReattachObjective(_linkInMatrix);
                Links.RevertOthersOnObjective(
                    toDeselect,
                    toReattach => toReattach.Possibility.ReturnFromObjective(toReattach));
            }
            --_selectedCount;
        }

        bool IObjective.TryDropPossibility(Link toDrop)
        {
            Debug.Assert(_toPossibility?.GetLinksOnObjective().Contains(toDrop) ?? false,
                "Tried to drop a link that's not connected to this objective.");
            if (AllPossibilitiesAreRequired)
            {
                return false;
            }
            _PopPossibility(toDrop);
            --_possibilityCount;
            return true;
        }

        void IObjective.ReturnPossibility(Link toReturn)
        {
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toReturn) ?? true,
                "Tried to return a possibility that's already connected to this objective.");
            ++_possibilityCount;
            _ReinsertPossibility(toReturn);
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
    }
}
