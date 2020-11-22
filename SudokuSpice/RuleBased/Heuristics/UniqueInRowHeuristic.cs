using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly PossibleValues _possibleValues;
        private readonly IMissingRowValuesTracker _rowTracker;
        private readonly BitVector[] _possiblesToCheckInRow;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;
        private IReadOnlyPuzzle? _puzzle;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use when solving.
        /// </param>
        /// <param name="rowValuesTracker">
        /// Something that tracks the possible values for each row. Rules often do this already,
        /// for example.
        /// </param>

        public UniqueInRowHeuristic(PossibleValues possibleValues, IMissingRowValuesTracker rowValuesTracker)
        {
            int size = possibleValues.Size;
            _possibleValues = possibleValues;
            _rowTracker = rowValuesTracker;
            _possiblesToCheckInRow = new BitVector[size];
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
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match current heuristic size ({_possibleValues.Size})");
            Debug.Assert(puzzle.Size == possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match possible values size ({possibleValues.Size})");
            return new UniqueInRowHeuristic(
                this, puzzle, possibleValues,
                (IMissingRowValuesTracker)rules.First(r => r is IMissingRowValuesTracker));
        }

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_possibleValues.Size}).");
            _puzzle = puzzle;
            _previousPossiblesStack.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(UpdateAll)} with a null puzzle.");
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
            IReadOnlyDictionary<Coordinate, BitVector>? overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach (KeyValuePair<Coordinate, BitVector> coordPossiblesPair in overwrittenPossibles)
            {
                _possibleValues[coordPossiblesPair.Key] = coordPossiblesPair.Value;
            }
        }

        private void _UpdateRow(int row, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_UpdateRow)} with a null puzzle.");
            Span<int> possibleValues = stackalloc int[_puzzle.Size];
            int numPossible = _possiblesToCheckInRow[row].PopulateSetBits(possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possible = possibleValues[i];
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
