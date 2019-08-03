using System.Collections.Generic;
using System.Linq;

namespace MorganRoff.Sudoku
{
    public class UniqueInColumnHeuristic : IHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly ColumnRestrict _restrict;
        private readonly bool[] _colsToCheck;
        private readonly int[] _possiblesToCheckInColumn;

        public UniqueInColumnHeuristic(Puzzle puzzle, ColumnRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _colsToCheck = new bool[puzzle.Size];
            _possiblesToCheckInColumn = new int[puzzle.Size];
        }

        public void Update(in Coordinate setCoordinate, IList<Coordinate> modifiedCoords)
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                _colsToCheck[col] = false;
                _possiblesToCheckInColumn[col] = _restrict.GetPossibleColumnValues(col);
            }
            _colsToCheck[setCoordinate.Column] = true;
            var possibles = _puzzle.GetPossibleValues(setCoordinate.Row, setCoordinate.Column);
            if (possibles.CountSetBits() == 1) {
                var setBit = possibles.GetSetBits().First();
                BitVectorUtils.UnsetBit(setBit, ref _possiblesToCheckInColumn[setCoordinate.Column]);
            }
            foreach (var modifiedCoord in modifiedCoords)
            {
                _colsToCheck[modifiedCoord.Column] = true;
                var modifiedPossibles = _puzzle.GetPossibleValues(modifiedCoord.Row, modifiedCoord.Column);
                if (modifiedPossibles.CountSetBits() == 1) {
                    var setBit = modifiedPossibles.GetSetBits().First();
                    BitVectorUtils.UnsetBit(setBit, ref _possiblesToCheckInColumn[modifiedCoord.Column]);
                }
            }
            _CheckColumns();
        }

        public void UpdateAll()
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                _colsToCheck[col] = true;
                _possiblesToCheckInColumn[col] = _restrict.GetPossibleColumnValues(col);
                for (int row = 0; row < _puzzle.Size; row++)
                {
                    if (_puzzle.Get(row, col).HasValue)
                    {
                        continue;
                    }
                    var modifiedPossibles = _puzzle.GetPossibleValues(row, col);
                    if (modifiedPossibles.CountSetBits() == 1)
                    {
                        BitVectorUtils.UnsetBit(modifiedPossibles.GetSetBits().First(), ref _possiblesToCheckInColumn[col]);
                    }
                }
            }
            _CheckColumns();
        }

        private void _CheckColumns()
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                if (!_colsToCheck[col])
                {
                    continue;
                }
                foreach (var possible in _possiblesToCheckInColumn[col].GetSetBits())
                {
                    bool isUniqueCoordForPossible = false;
                    int uniqueRow = -1;
                    for (int row = 0; row < _puzzle.Size; row++)
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
                            uniqueRow = row;
                        }
                    }
                    if (!isUniqueCoordForPossible)
                    {
                        continue;
                    }
                    var possibles = 0;
                    BitVectorUtils.SetBit(possible, ref possibles);
                    _puzzle.SetPossibleValues(uniqueRow, col, possibles);
                }
            }
        }
    }
}
