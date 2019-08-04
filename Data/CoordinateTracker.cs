using System;

namespace MorganRoff.Sudoku
{
    // Efficiently tracks a group of coordinates.
    public class CoordinateTracker
    {
        private readonly int[,] _coordToIdx;
        private readonly Coordinate[] _coords;
        private int _numTracked = 0;
        private int _numAdded = 0;

        public CoordinateTracker(int sideLength)
        {
            _coordToIdx = new int[sideLength, sideLength];
            for (int row = 0; row < sideLength; row++)
            {
                for (int col = 0; col < sideLength; col++)
                {
                    _coordToIdx[row, col] = -1;
                }
            }
            _coords = new Coordinate[sideLength * sideLength];
        }

        public void Add(in Coordinate c)
        {
            _coords[_numAdded] = c;
            _coordToIdx[c.Row, c.Column] = _numAdded;
            _numAdded++;
            Track(in c);
        }

        public void Track(in Coordinate c)
        {
            if (_numTracked == _numAdded)
            {
                throw new InvalidOperationException("The tracker is full.");
            }
            int idx = _coordToIdx[c.Row, c.Column];
            if (idx < _numTracked)
            {
                throw new InvalidOperationException($"Coordinate {c} is already tracked.");
            }
            var otherUntrackedCoord = _coords[_numTracked];
            _coords[idx] = otherUntrackedCoord;
            _coordToIdx[otherUntrackedCoord.Row, otherUntrackedCoord.Column] = idx;
            _coords[_numTracked] = c;
            _coordToIdx[c.Row, c.Column] = _numTracked;
            _numTracked++;
        }

        public void Untrack(in Coordinate c)
        {
            if (_numTracked == 0)
            {
                throw new InvalidOperationException("The tracker is empty.");
            }
            var idx = _coordToIdx[c.Row, c.Column];
            if (idx >= _numTracked)
            {
                throw new InvalidOperationException($"Coordinate {c} is already untracked.");
            }
            _numTracked--;
            var lastTrackedCoord = _coords[_numTracked];
            _coords[idx] = lastTrackedCoord;
            _coordToIdx[lastTrackedCoord.Row, lastTrackedCoord.Column] = idx;
            _coords[_numTracked] = c;
            _coordToIdx[c.Row, c.Column] = _numTracked;
        }

        public ReadOnlySpan<Coordinate> GetTrackedCoords()
        {
            return new ReadOnlySpan<Coordinate>(_coords, 0, _numTracked);
        }
    }
}
