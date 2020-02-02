using System.Linq;

namespace MorganRoff.Sudoku
{
    public class UniqueInColumnHeuristic : IHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly ColumnRestrict _restrict;
        private readonly int[] _possiblesToCheckInColumn;

        public UniqueInColumnHeuristic(Puzzle puzzle, ColumnRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _possiblesToCheckInColumn = new int[puzzle.Size];
        }

        public void UpdateAll()
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                _PreparePossiblesToCheckInColumn(col);
                _CheckColumn(col);
            }
        }

        private void _PreparePossiblesToCheckInColumn(int col)
        {
            _possiblesToCheckInColumn[col] = _restrict.GetPossibleColumnValues(col);
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (_puzzle[row, col].HasValue)
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

        private void _CheckColumn(int col)
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
