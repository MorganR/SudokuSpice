using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.Data
{
    /// <summary>Efficiently tracks a group of coordinates.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class CoordinateTracker
    {
        private readonly int[,] _coordToIdx;
        private readonly Coordinate[] _coords;
        public int NumTracked { get; private set; }
        private int _numAdded = 0;

        public enum AddOrTrackResult
        {
            AddedAndTracked = 0,
            Tracked = 1,
            Unchanged = 2
        }

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

        public CoordinateTracker(CoordinateTracker existing)
        {
            _coordToIdx = (int[,])existing._coordToIdx.Clone();
            _coords = (Coordinate[])existing._coords.Clone();
            NumTracked = existing.NumTracked;
            _numAdded = existing._numAdded;
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
            Debug.Assert(NumTracked != _numAdded, "The tracker is full.");
            int idx = _coordToIdx[c.Row, c.Column];
            _Track(in c, idx);
        }

        public AddOrTrackResult AddOrTrackIfUntracked(in Coordinate c)
        {
            if (_coordToIdx[c.Row, c.Column] == -1)
            {
                Add(in c);
                return AddOrTrackResult.AddedAndTracked;
            }
            int idx = _coordToIdx[c.Row, c.Column];
            if (idx < NumTracked)
            {
                return AddOrTrackResult.Unchanged;
            }
            _Track(in c, idx);
            return AddOrTrackResult.Tracked;
        }

        public void Untrack(in Coordinate c)
        {
            Debug.Assert(NumTracked > 0, "The tracker is empty");
            var idx = _coordToIdx[c.Row, c.Column];
            Debug.Assert(idx >= 0, $"Coordinate {c} was never added.");
            Debug.Assert(idx < NumTracked, $"Coordinate {c} is already untracked.");
            NumTracked--;
            var lastTrackedCoord = _coords[NumTracked];
            _coords[idx] = lastTrackedCoord;
            _coords[NumTracked] = c;
            _coordToIdx[lastTrackedCoord.Row, lastTrackedCoord.Column] = idx;
            _coordToIdx[c.Row, c.Column] = NumTracked;
        }

        public void UntrackAll()
        {
            NumTracked = 0;
        }

        public ReadOnlySpan<Coordinate> GetTrackedCoords()
        {
            return new ReadOnlySpan<Coordinate>(_coords, 0, NumTracked);
        }

        private void _Track(in Coordinate c, int index)
        {
            Debug.Assert(index >= 0, $"Coordinate {c} was never added.");
            Debug.Assert(index >= NumTracked, $"Coordinate {c} is already tracked.");
            var otherUntrackedCoord = _coords[NumTracked];
            _coords[index] = otherUntrackedCoord;
            _coords[NumTracked] = c;
            _coordToIdx[otherUntrackedCoord.Row, otherUntrackedCoord.Column] = index;
            _coordToIdx[c.Row, c.Column] = NumTracked;
            NumTracked++;
        }
    }
}
