namespace SudokuSpice
{
    public class UniqueInColumnHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly ColumnRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInColumn;

        public UniqueInColumnHeuristic(Puzzle puzzle, ColumnRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _possiblesToCheckInColumn = new BitVector[puzzle.Size];
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
                var possibles = new BitVector();
                possibles.SetBit(possible);
                _puzzle.SetPossibleValues(uniqueRow, col, possibles);
            }
        }
    }
}
