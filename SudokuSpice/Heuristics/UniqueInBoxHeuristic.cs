namespace SudokuSpice
{
    public class UniqueInBoxHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly BoxRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInBox;

        public UniqueInBoxHeuristic(Puzzle puzzle, BoxRestrict restrict)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _possiblesToCheckInBox = new BitVector[puzzle.Size];
        }

        public void UpdateAll()
        {
            for (int box = 0; box < _puzzle.Size; box++)
            {
                _PreparePossiblesToCheckInBox(box);
                _CheckBox(box);
            }
        }

        private void _PreparePossiblesToCheckInBox(int box)
        {
            _possiblesToCheckInBox[box] = _restrict.GetPossibleBoxValues(box);
        }

        private void _CheckBox(int box)
        {
            foreach (var possible in _possiblesToCheckInBox[box].GetSetBits())
            {
                Coordinate? uniqueCoord = null;
                foreach (var c in _puzzle.YieldUnsetCoordsForBox(box))
                {
                    if (_puzzle.GetPossibleValues(c.Row, c.Column).IsBitSet(possible))
                    {
                        if (uniqueCoord.HasValue)
                        {
                            uniqueCoord = null;
                            break;
                        }
                        uniqueCoord = c;
                    }
                }
                if (!uniqueCoord.HasValue)
                {
                    continue;
                }
                var possibles = new BitVector();
                possibles.SetBit(possible);
                _puzzle.SetPossibleValues(uniqueCoord.Value.Row, uniqueCoord.Value.Column, possibles);
            }
        }
    }
}
