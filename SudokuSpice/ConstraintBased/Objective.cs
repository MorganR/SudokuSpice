﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// A required objective in the <see cref="ExactCoverGraph"/>. This enforces that one or more
    /// <see cref="IPossibility"/>s must be selected.
    /// </summary>
    public sealed class Objective : IObjective
    {
        private readonly int _countToSatisfy;
        private readonly ExactCoverGraph _graph;
        private int _possibilityCount;
        private int _selectedCount;
        private Link? _toPossibility;
        private bool _allPossibilitiesAreConcrete;
        private bool _atLeastOnePossibilityIsConcrete;
        private Objective _nextObjectiveInGraph;
        private Objective _previousObjectiveInGraph;
        private NodeState _state;

        internal Objective NextObjectiveInGraph => _nextObjectiveInGraph;

        /// <inheritdoc />
        public NodeState State => _state;

        /// <summary>
        /// True if at least one attached possibility is a concrete possibility, i.e. a leaf node.
        /// </summary>
        internal bool AtLeastOnePossibilityIsConcrete => _atLeastOnePossibilityIsConcrete;

        /// <summary>
        /// True if all attached possibility are concrete possibilities, i.e. leaf nodes.
        /// 
        /// If no possibilities are unknown, this returns false.
        /// </summary>
        internal bool AllUnknownPossibilitiesAreConcrete
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return _allPossibilitiesAreConcrete ||
                    (_atLeastOnePossibilityIsConcrete && _state != NodeState.SELECTED
                     && _toPossibility!.GetPossibilitiesOnObjective().All(p => p.IsConcrete));
            }
        }
        /// <summary>
        /// Whether or not all remaining unknown possibilities are required.
        /// </summary>
        internal bool AllUnknownPossibilitiesAreRequired => _possibilityCount == _countToSatisfy;
        /// <summary>
        /// The total number of possibilities that must be selected for this objective to be
        /// satisfied.
        /// </summary>
        internal int TotalCountToSatisfy => _countToSatisfy;
        /// <summary>
        /// The number of possibilities that are in an unknown state.
        /// </summary>
        internal int CountUnknown => _possibilityCount - _selectedCount;

        /// <inheritdoc />
        bool IObjective.IsRequired => true;

        private Objective(ExactCoverGraph graph, int countToSatisfy)
        {
            _graph = graph;
            _countToSatisfy = countToSatisfy;
            _allPossibilitiesAreConcrete = true;
            _atLeastOnePossibilityIsConcrete = false;
            _state = NodeState.UNKNOWN;
            _nextObjectiveInGraph = this;
            _previousObjectiveInGraph = this;
        }

        /// <summary>
        /// Constructs an objective that's fully connected to the given
        /// <paramref name="possibilities"/> and into the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The graph to attach this to.</param>
        /// <param name="possibilities">The possibilities that could satisfy this objective.</param>
        /// <param name="countToSatisfy">
        /// The number of possibilities that must be satisfied in order to satisfy this objective.
        /// Once this number of possibilities are selected, all other possibilities on this
        /// objective will be dropped.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="countToSatisfy"/> is less than 1 or is impossible with the
        /// given number of <paramref name="possibilities"/>.
        /// </exception>
        /// <returns>The newly constructed objective.</returns>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static Objective CreateFullyConnected(
            ExactCoverGraph graph,
            ReadOnlySpan<IPossibility> possibilities,
            int countToSatisfy)
        {
            if (countToSatisfy < 1 || countToSatisfy > possibilities.Length)
            {
                throw new ArgumentException($"{nameof(countToSatisfy)} must be in the inclusive range [1, {nameof(possibilities)}.Length].");
            }
            var objective = new Objective(graph, countToSatisfy);
            foreach (var possibility in possibilities)
            {
                Link.CreateConnectedLink(possibility, objective);
            }
            graph.AttachObjective(objective);
            return objective;
        }

        /// <summary>
        /// Constructs an objective that's fully connected to the given
        /// <paramref name="possibilities"/> and into the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The graph to attach this to.</param>
        /// <param name="possibilities">The possibilities that could satisfy this objective.</param>
        /// <param name="countToSatisfy">
        /// The number of possibilities that must be satisfied in order to satisfy this objective.
        /// Once this number of possibilities are selected, all other possibilities on this
        /// objective will be dropped.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="countToSatisfy"/> is less than 1 or is impossible with the
        /// given number of <paramref name="possibilities"/>.
        /// </exception>
        /// <returns>The newly constructed objective.</returns>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static Objective CreateFullyConnected(
            ExactCoverGraph graph,
            ReadOnlySpan<Possibility> possibilities,
            int countToSatisfy)
        {
            if (countToSatisfy < 1 || countToSatisfy > possibilities.Length)
            {
                throw new ArgumentException($"{nameof(countToSatisfy)} must be in the inclusive range [1, {nameof(possibilities)}.Length].");
            }
            var objective = new Objective(graph, countToSatisfy);
            foreach (var possibility in possibilities)
            {
                Link.CreateConnectedLink(possibility, objective);
            }
            graph.AttachObjective(objective);
            return objective;
        }

        /// <inheritdoc />
        public IEnumerable<IPossibility> GetUnknownDirectPossibilities()
        {
            if (State == NodeState.SELECTED)
            {
                return Enumerable.Empty<IPossibility>();
            }
            return _toPossibility!.GetPossibilitiesOnObjective();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IObjective.AppendPossibility(Link toNewPossibility)
        {
            ++_possibilityCount;
            if (toNewPossibility.Possibility.IsConcrete)
            {
                _atLeastOnePossibilityIsConcrete = true;
            } else
            {
                _allPossibilitiesAreConcrete = false;
            }
            if (_toPossibility is null)
            {
                _toPossibility = toNewPossibility;
                return;
            }
            _toPossibility.PrependToObjective(toNewPossibility);
        }

        /// <inheritdoc />
        bool IObjective.TrySelectPossibility(Link toSelect)
        {
            Debug.Assert(_toPossibility is not null,
                "At least one possibility must be attached.");
            Debug.Assert(State != NodeState.SELECTED,
                "Can't select a possiblity for an already satisfied objective.");
            Debug.Assert(_toPossibility.GetLinksOnObjective().Contains(toSelect),
                "Tried to select a link that's not connected to this objective.");
            if (++_selectedCount == _countToSatisfy)
            {
                if (!Links.TryUpdateOthersOnObjective(
                    toSelect,
                    toDetach => toDetach.Possibility.TryDropFromObjective(toDetach),
                    toReattach => toReattach.Possibility.ReturnFromObjective(toReattach)))
                {
                    --_selectedCount;
                    return false;
                }
                _graph.DetachObjective(this);
                _state = NodeState.SELECTED;
            }
            _PopPossibility(toSelect);
            return true;
        }

        /// <inheritdoc />
        void IObjective.DeselectPossibility(Link toDeselect)
        {
            Debug.Assert(!_toPossibility!.GetLinksOnObjective().Contains(toDeselect) ||
                _toPossibility.GetLinksOnObjective().Count() == 1,
                "Tried to deselect a link that's not connected to this objective.");
            _ReinsertPossibility(toDeselect);
            if (_selectedCount == _countToSatisfy)
            {
                _state = NodeState.UNKNOWN;
                ExactCoverGraph.ReattachObjective(this);
                Links.RevertOthersOnObjective(
                    toDeselect,
                    toReattach => toReattach.Possibility.ReturnFromObjective(toReattach));
            }
            --_selectedCount;
        }

        /// <inheritdoc />
        bool IObjective.TryDropPossibility(Link toDrop)
        {
            Debug.Assert(_toPossibility?.GetLinksOnObjective().Contains(toDrop) ?? false,
                "Tried to drop a link that's not connected to this objective.");
            if (AllUnknownPossibilitiesAreRequired)
            {
                return false;
            }
            _PopPossibility(toDrop);
            --_possibilityCount;
            return true;
        }

        /// <inheritdoc />
        void IObjective.ReturnPossibility(Link toReturn)
        {
            Debug.Assert(!_toPossibility?.GetLinksOnObjective().Contains(toReturn) ?? true,
                "Tried to return a possibility that's already connected to this objective.");
            ++_possibilityCount;
            _ReinsertPossibility(toReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PrependToGraphBefore(Objective next)
        {
            _nextObjectiveInGraph = next;
            _previousObjectiveInGraph = next._previousObjectiveInGraph;
            next._previousObjectiveInGraph = this;
            _previousObjectiveInGraph._nextObjectiveInGraph = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PopFromGraph()
        {
            _previousObjectiveInGraph._nextObjectiveInGraph = _nextObjectiveInGraph;
            _nextObjectiveInGraph._previousObjectiveInGraph = _previousObjectiveInGraph;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReinsertToGraph()
        {
            _previousObjectiveInGraph._nextObjectiveInGraph = this;
            _nextObjectiveInGraph._previousObjectiveInGraph = this;
        }

        internal IEnumerable<Objective> GetConnectedObjectives()
        {
            var link = this;
            do
            {
                yield return link;
                link = link._nextObjectiveInGraph;
            } while (link != this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _PopPossibility(Link toPop)
        {
            toPop.PopFromObjective();
            if (_toPossibility == toPop)
            {
                _toPossibility = toPop.NextOnObjective;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _ReinsertPossibility(Link toReinsert)
        {
            toReinsert.ReinsertToObjective();
        }
    }
}