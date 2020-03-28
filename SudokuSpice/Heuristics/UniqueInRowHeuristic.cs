using SudokuSpice.Data;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice
{
    public class UniqueInRowHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IRowRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInRow;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInRowHeuristic(Puzzle puzzle, PossibleValues possibleValues, IRowRestrict restrict)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _restrict = restrict;
            _possiblesToCheckInRow = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>();
        }

        private UniqueInRowHeuristic(
            UniqueInRowHeuristic existing,
            Puzzle puzzle,
            PossibleValues possibleValues,
            IRowRestrict restrict)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _restrict = restrict;
            _possiblesToCheckInRow = (BitVector[])existing._possiblesToCheckInRow.Clone();
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>(
                existing._previousPossiblesStack);
        }

        public ISudokuHeuristic CopyWithNewReferences(
            Puzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRestrict> restricts)
        {
            return new UniqueInRowHeuristic(
                this, puzzle, possibleValues,
                (IRowRestrict)restricts.First(r => r is IRowRestrict));
        }

        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int row = 0; row < _puzzle.Size; row++)
            {
                _possiblesToCheckInRow[row] = _restrict.GetPossibleRowValues(row);
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
