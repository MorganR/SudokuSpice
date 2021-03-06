﻿using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Provides the standard <c>UniqueIn*</c> heuristics.
    /// </summary>
    public class StandardHeuristic : IHeuristic
    {
        private readonly UniqueInRowHeuristic _rowHeuristic;
        private readonly UniqueInColumnHeuristic _columnHeuristic;
        private readonly UniqueInBoxHeuristic _boxHeuristic;
        private readonly Stack<int> _numHeuristicsRan;

        /// <summary>
        /// Creates a standard heuristic that combines the <see cref="UniqueInRowHeuristic"/>,
        /// <see cref="UniqueInColumnHeuristic"/>, and <see cref="UniqueInBoxHeuristic"/>.
        /// </summary>
        /// <param name="rowValuesTracker">
        /// Something that tracks the possible values for each row.
        /// </param>
        /// <param name="columnValuesTracker">
        /// Something that tracks the possible values for each column.
        /// </param>
        /// <param name="boxValuesTracker">
        /// Something that tracks the possible values for each box.
        /// </param>
        public StandardHeuristic(
            IMissingRowValuesTracker rowValuesTracker,
            IMissingColumnValuesTracker columnValuesTracker,
            IMissingBoxValuesTracker boxValuesTracker)
        {
            _rowHeuristic = new UniqueInRowHeuristic(rowValuesTracker);
            _columnHeuristic = new UniqueInColumnHeuristic(columnValuesTracker);
            _boxHeuristic = new UniqueInBoxHeuristic(boxValuesTracker);
            _numHeuristicsRan = new Stack<int>();
        }

        /// <inheritdoc/> 
        public bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            if (!_rowHeuristic.TryInitFor(puzzle)
                || !_columnHeuristic.TryInitFor(puzzle)
                || !_boxHeuristic.TryInitFor(puzzle))
            {
                return false;
            }
            _numHeuristicsRan.Clear();
            return true;
        }

        private StandardHeuristic(
            StandardHeuristic existing,
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            ReadOnlySpan<IRule> rules)
        {
            _rowHeuristic = (UniqueInRowHeuristic)existing._rowHeuristic.CopyWithNewReferences(
                puzzle, rules);
            _columnHeuristic = (UniqueInColumnHeuristic)existing._columnHeuristic
                .CopyWithNewReferences(puzzle, rules);
            _boxHeuristic = (UniqueInBoxHeuristic)existing._boxHeuristic.CopyWithNewReferences(
                puzzle, rules);
            _numHeuristicsRan = new Stack<int>(existing._numHeuristicsRan);
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingBoxValuesTracker"/>, an <see cref="IMissingColumnValuesTracker"/>,
        /// and an <see cref="IMissingRowValuesTracker"/>.
        /// </summary>
        public IHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            ReadOnlySpan<IRule> rules) => new StandardHeuristic(this, puzzle, rules);

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            int numToUndo = _numHeuristicsRan.Pop();
            if (numToUndo > 2)
            {
                _boxHeuristic.UndoLastUpdate();
            }
            if (numToUndo > 1)
            {
                _columnHeuristic.UndoLastUpdate();
            }
            _rowHeuristic.UndoLastUpdate();
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            //// Note: we can't simply do _h1.UpdateAll() | _h2.UpdateAll() | _h3.UpdateAll because
            //// we need to have a definite ordering of which heuristic ran first. This is
            //// necessary in order to accurately undo the heuristics.
            int numRan = 1;
            if (_rowHeuristic.UpdateAll())
            {
                _numHeuristicsRan.Push(numRan);
                return true;
            }
            numRan++;
            if (_columnHeuristic.UpdateAll())
            {
                _numHeuristicsRan.Push(numRan);
                return true;
            }
            numRan++;
            _numHeuristicsRan.Push(numRan);
            return _boxHeuristic.UpdateAll();
        }
    }
}