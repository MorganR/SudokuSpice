using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSpice
{
    public class StandardSquareTracker : ISquareTracker
    {
        public Coordinate GetBestCoordinateToGuess()
        {
            throw new NotImplementedException();
        }

        public int GetNumEmptySquares()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetPossibleValues(in Coordinate c)
        {
            throw new NotImplementedException();
        }

        public bool TrySet(in Coordinate c, int possibleValue)
        {
            throw new NotImplementedException();
        }

        public void Unset(in Coordinate c)
        {
            throw new NotImplementedException();
        }
    }
}
