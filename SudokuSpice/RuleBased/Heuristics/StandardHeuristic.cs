using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased.Heuristics
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

        /// <summary>
        /// Creates a standard heuristic that combines the <see cref="UniqueInRowHeuristic"/>,
        /// <see cref="UniqueInColumnHeuristic"/>, and <see cref="UniqueInBoxHeuristic"/>.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use when solving.
        /// </param>
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
            PossibleValues possibleValues,
            IMissingRowValuesTracker rowValuesTracker,
            IMissingColumnValuesTracker columnValuesTracker,
            IMissingBoxValuesTracker boxValuesTracker)
        {
            _rowHeuristic = new UniqueInRowHeuristic(possibleValues, rowValuesTracker);
            _columnHeuristic = new UniqueInColumnHeuristic(possibleValues, columnValuesTracker);
            _boxHeuristic = new UniqueInBoxHeuristic(possibleValues, boxValuesTracker);
            _numHeuristicsRan = new Stack<int>();
        }

        /// <summary>
        /// Tries to initialize this heuristic for solving the given puzzle.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the heuristic's state is unchanged
        /// on failure.
        /// </remarks>
        /// <param name="puzzle">
        /// The puzzle to solve. This must implement <see cref="IReadOnlyBoxPuzzle"/>.
        /// </param>
        /// <returns>
        /// False if this heuristic cannot be initialized for the given puzzle, else true.
        /// </returns>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
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
            IReadOnlyList<ISudokuRule> rules) => new StandardHeuristic(this, puzzle, possibleValues, rules);

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
