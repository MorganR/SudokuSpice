using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a
    /// column. Sets the possible values for those squares to just their unique value.
    /// </summary>
    /// <remarks>
    /// For example, if a column had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </remarks>
    public class UniqueInColumnHeuristic : ISudokuHeuristic
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IMissingColumnValuesTracker _columnTracker;
        private readonly BitVector[] _possiblesToCheckInColumn;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInColumnHeuristic(IReadOnlyPuzzle puzzle, PossibleValues possibleValues, IMissingColumnValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _columnTracker = rule;
            _possiblesToCheckInColumn = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>(puzzle.NumEmptySquares);
        }

        private UniqueInColumnHeuristic(
            UniqueInColumnHeuristic existing,
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IMissingColumnValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _columnTracker = rule;
            _possiblesToCheckInColumn = (BitVector[])existing._possiblesToCheckInColumn.Clone();
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>(
                existing._previousPossiblesStack);
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingColumnValuesTracker"/>.
        /// </summary>
        public ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            return new UniqueInColumnHeuristic(
                this, puzzle, possibleValues,
                (IMissingColumnValuesTracker)rules.First(r => r is IMissingColumnValuesTracker));
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int col = 0; col < _puzzle.Size; col++)
            {
                _possiblesToCheckInColumn[col] = _columnTracker.GetMissingValuesForColumn(col);
                _UpdateColumn(col, previousPossibles);
            }
            _previousPossiblesStack.Push(previousPossibles);
            return previousPossibles.Count > 0;
        }

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            IReadOnlyDictionary<Coordinate, BitVector>? overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach (KeyValuePair<Coordinate, BitVector> coordPossiblesPair in overwrittenPossibles)
            {
                _possibleValues[coordPossiblesPair.Key] = coordPossiblesPair.Value;
            }
        }

        private void _UpdateColumn(int col, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            Span<int> possibleValues = stackalloc int[_puzzle.Size];
            int numPossible = _possiblesToCheckInColumn[col].PopulateSetBits(possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possible = possibleValues[i];
                bool isUniqueCoordForPossible = false;
                int uniqueRow = -1;
                for (int row = 0; row < _puzzle.Size; row++)
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
                        uniqueRow = row;
                    }
                }
                if (!isUniqueCoordForPossible)
                {
                    continue;
                }
                var possibles = new BitVector();
                possibles.SetBit(possible);
                var uniqueCoord = new Coordinate(uniqueRow, col);
                previousPossibles[uniqueCoord] = _possibleValues[in uniqueCoord];
                _possibleValues[in uniqueCoord] = possibles;
            }
        }
    }
}
