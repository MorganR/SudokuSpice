using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class UniqueInRowHeuristic : IHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly RowRestrict _restrict;
        private readonly bool[] _rowsToCheck;
        private readonly int[] _possiblesToCheckInRow;

        public UniqueInRowHeuristic(Puzzle puzzle, RowRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _rowsToCheck = new bool[puzzle.Size];
            _possiblesToCheckInRow = new int[puzzle.Size];
        }

        public void Update(in Coordinate setCoordinate, IList<Coordinate> modifiedCoords)
        {
            for (int row = 0; row < _puzzle.Size; row++)
            {
                _rowsToCheck[row] = false;
                _possiblesToCheckInRow[row] = _restrict.GetPossibleRowValues(row);
            }
            _rowsToCheck[setCoordinate.Row] = true;
            var possibles = _puzzle.GetPossibleValues(setCoordinate.Row, setCoordinate.Column);
            if (possibles.CountSetBits() == 1) {
                var setBit = possibles.GetSetBits().First();
                BitVectorUtils.UnsetBit(setBit, ref _possiblesToCheckInRow[setCoordinate.Row]);
            }
            foreach (var modifiedCoord in modifiedCoords)
            {
                _rowsToCheck[modifiedCoord.Row] = true;
                var modifiedPossibles = _puzzle.GetPossibleValues(modifiedCoord.Row, modifiedCoord.Column);
                if (modifiedPossibles.CountSetBits() == 1) {
                    var setBit = modifiedPossibles.GetSetBits().First();
                    BitVectorUtils.UnsetBit(setBit, ref _possiblesToCheckInRow[modifiedCoord.Row]);
                }
            }
            _CheckRows();
        }

        public void UpdateAll()
        {
            for (int row = 0; row < _puzzle.Size; row++)
            {
                _rowsToCheck[row] = true;
                _possiblesToCheckInRow[row] = _restrict.GetPossibleRowValues(row);
                for (int col = 0; col < _puzzle.Size; col++)
                {
                    if (_puzzle.Get(row, col).HasValue)
                    {
                        continue;
                    }
                    var modifiedPossibles = _puzzle.GetPossibleValues(row, col);
                    if (modifiedPossibles.CountSetBits() == 1)
                    {
                        BitVectorUtils.UnsetBit(modifiedPossibles.GetSetBits().First(), ref _possiblesToCheckInRow[row]);
                    }
                }
            }
            _CheckRows();
        }

        private void _CheckRows()
        {
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (!_rowsToCheck[row])
                {
                    continue;
                }
                foreach (var possible in _possiblesToCheckInRow[row].GetSetBits())
                {
                    bool isUniqueCoordForPossible = false;
                    int uniqueCol = -1;
                    for (int col = 0; col < _puzzle.Size; col++)
                    {
                        if (_puzzle.Get(row, col).HasValue)
                        {
                            continue;
                        }
                        if (_puzzle.GetPossibleValues(row, col).IsBitSet(possible))
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
                    var possibles = 0;
                    BitVectorUtils.SetBit(possible, ref possibles);
                    _puzzle.SetPossibleValues(row, uniqueCol, possibles);
                }
            }
        }
    }
}
