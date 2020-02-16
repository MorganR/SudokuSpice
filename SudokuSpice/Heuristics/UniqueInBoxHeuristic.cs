using System.Collections.Generic;

namespace SudokuSpice
{
    public class UniqueInBoxHeuristic : ISudokuHeuristic
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IBoxRestrict _restrict;
        private readonly BitVector[] _possiblesToCheckInBox;
        private readonly Stack<IDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInBoxHeuristic(Puzzle puzzle, PossibleValues possibleValues, IBoxRestrict restrict)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _restrict = restrict;
            _possiblesToCheckInBox = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IDictionary<Coordinate, BitVector>>();
        }

        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int box = 0; box < _puzzle.Size; box++)
            {
                _possiblesToCheckInBox[box] = _restrict.GetPossibleBoxValues(box);
                _UpdateBox(box, previousPossibles);
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

        private void _UpdateBox(int box, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            foreach (var possible in _possiblesToCheckInBox[box].GetSetBits())
            {
                Coordinate? uniqueCoord = null;
                foreach (var c in _puzzle.YieldUnsetCoordsForBox(box))
                {
                    if (_possibleValues[in c].IsBitSet(possible))
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
                previousPossibles[uniqueCoord.Value] = _possibleValues[uniqueCoord.Value];
                _possibleValues[uniqueCoord.Value] = possibles;
            }
        }
    }
}
