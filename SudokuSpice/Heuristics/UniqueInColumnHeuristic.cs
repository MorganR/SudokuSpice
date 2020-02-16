using System.Collections.Generic;

namespace SudokuSpice
{
    public class UniqueInColumnHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IColumnRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInColumn;
        private readonly Stack<IDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInColumnHeuristic(Puzzle puzzle, PossibleValues possibleValues, IColumnRestrict restrict)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _restrict = restrict;
            _possiblesToCheckInColumn = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IDictionary<Coordinate, BitVector>>(puzzle.NumEmptySquares);
        }

        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int col = 0; col < _puzzle.Size; col++)
            {
                _possiblesToCheckInColumn[col] = _restrict.GetPossibleColumnValues(col);
                _UpdateColumn(col, previousPossibles);
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

        private void _UpdateColumn(int col, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            foreach (var possible in _possiblesToCheckInColumn[col].GetSetBits())
            {
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
