using SudokuSpice.Data;
using SudokuSpice.Rules;
using System.Collections.Generic;

namespace SudokuSpice.Heuristics
{
    /// <summary>
    /// Provides the standard <c>UniqueIn*</c> heuristics.
    /// </summary>
    public class StandardHeuristic : ISudokuHeuristic
    {
        private readonly UniqueInRowHeuristic _rowHeuristic;
        private readonly UniqueInColumnHeuristic _columnHeuristic;
        private readonly UniqueInBoxHeuristic _boxHeuristic;
        private readonly Stack<int> _numHeuristicsRan; 

        public StandardHeuristic(IReadOnlyPuzzle puzzle, PossibleValues possibleValues,
            IMissingRowValuesTracker rowRule, IMissingColumnValuesTracker columnRule, IMissingBoxValuesTracker boxRule)
        {
            _rowHeuristic = new UniqueInRowHeuristic(puzzle, possibleValues, rowRule);
            _columnHeuristic = new UniqueInColumnHeuristic(puzzle, possibleValues, columnRule);
            _boxHeuristic = new UniqueInBoxHeuristic(puzzle, possibleValues, boxRule);
            _numHeuristicsRan = new Stack<int>(puzzle.NumEmptySquares);
        }

        private StandardHeuristic(
            StandardHeuristic existing,
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            _rowHeuristic = (UniqueInRowHeuristic)existing._rowHeuristic.CopyWithNewReferences(
                puzzle, possibleValues, rules);
            _columnHeuristic = (UniqueInColumnHeuristic)existing._columnHeuristic
                .CopyWithNewReferences(puzzle, possibleValues, rules);
            _boxHeuristic = (UniqueInBoxHeuristic)existing._boxHeuristic.CopyWithNewReferences(
                puzzle, possibleValues, rules);
            _numHeuristicsRan = new Stack<int>(existing._numHeuristicsRan);
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingBoxValuesTracker"/>, an <see cref="IMissingColumnValuesTracker"/>,
        /// and an <see cref="IMissingRowValuesTracker"/>.
        /// </summary>
        public ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            return new StandardHeuristic(this, puzzle, possibleValues, rules);
        }

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
