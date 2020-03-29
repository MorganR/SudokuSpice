using SudokuSpice.Data;
using SudokuSpice.Rules;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.Heuristics
{
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

        public ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            return new UniqueInRowHeuristic(
                this, puzzle, possibleValues,
                (IMissingRowValuesTracker)rules.First(r => r is IMissingRowValuesTracker));
        }

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
