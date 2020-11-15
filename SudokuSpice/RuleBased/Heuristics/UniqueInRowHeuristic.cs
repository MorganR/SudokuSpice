using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a row.
    /// Sets the possible values for those squares to just their unique value.
    /// </summary>
    /// <remarks>
    /// For example, if a row had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </remarks>
    public class UniqueInRowHeuristic : ISudokuHeuristic
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IMissingRowValuesTracker _rowTracker;
        private readonly BitVector[] _possiblesToCheckInRow;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInRowHeuristic(IReadOnlyPuzzle puzzle, PossibleValues possibleValues, IMissingRowValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _rowTracker = rule;
            _possiblesToCheckInRow = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>();
        }

        private UniqueInRowHeuristic(
            UniqueInRowHeuristic existing,
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IMissingRowValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _rowTracker = rule;
            _possiblesToCheckInRow = (BitVector[])existing._possiblesToCheckInRow.Clone();
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>(
                existing._previousPossiblesStack);
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingRowValuesTracker"/>.
        /// </summary>
        public ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            return new UniqueInRowHeuristic(
                this, puzzle, possibleValues,
                (IMissingRowValuesTracker)rules.First(r => r is IMissingRowValuesTracker));
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int row = 0; row < _puzzle.Size; row++)
            {
                _possiblesToCheckInRow[row] = _rowTracker.GetMissingValuesForRow(row);
                _UpdateRow(row, previousPossibles);
            }
            _previousPossiblesStack.Push(previousPossibles);
            return previousPossibles.Count > 0;
        }

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            var overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach (var coordPossiblesPair in overwrittenPossibles)
            {
                _possibleValues[coordPossiblesPair.Key] = coordPossiblesPair.Value;
            }
        }

        private void _UpdateRow(int row, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            foreach (var possible in _possiblesToCheckInRow[row].GetSetBits())
            {
                bool isUniqueCoordForPossible = false;
                int uniqueCol = -1;
                for (int col = 0; col < _puzzle.Size; col++)
                {
                    if (_puzzle[row, col].HasValue)
                    {
                        continue;
                    }
                    if (_possibleValues[new Coordinate(row, col)].IsBitSet(possible))
                    {
                        if (isUniqueCoordForPossible)
                        {
                            isUniqueCoordForPossible = false;
                            break;
                        }
                        isUniqueCoordForPossible = true;
                        uniqueCol = col;
                    }
                }
                if (!isUniqueCoordForPossible)
                {
                    continue;
                }
                var possibles = new BitVector();
                possibles.SetBit(possible);
                var uniqueCoord = new Coordinate(row, uniqueCol);
                previousPossibles[uniqueCoord] = _possibleValues[in uniqueCoord];
                _possibleValues[in uniqueCoord] = possibles;
            }
        }
    }
}
