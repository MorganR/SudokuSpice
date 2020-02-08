namespace SudokuSpice
{
    public class UniqueInRowHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly RowRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInRow;

        public UniqueInRowHeuristic(Puzzle puzzle, RowRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _possiblesToCheckInRow = new BitVector[puzzle.Size];
        }

        public void UpdateAll()
        {
            for (int row = 0; row < _puzzle.Size; row++)
            {
                _PreparePossiblesToCheckInRow(row);
                _CheckRow(row);
            }
        }

        private void _PreparePossiblesToCheckInRow(int row)
        {
            _possiblesToCheckInRow[row] = _restrict.GetPossibleRowValues(row);
        }

        private void _CheckRow(int row)
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
                var possibles = new BitVector();
                possibles.SetBit(possible);
                _puzzle.SetPossibleValues(row, uniqueCol, possibles);
            }
        }
    }
}
