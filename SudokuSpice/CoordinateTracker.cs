using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>Efficiently tracks a set of <see cref="Coordinate"/>s.</summary>
    /// <remarks>
    /// This swaps coordinates within an internal array to provide O(1) tracking and untracking
    /// operations. It also provides immediate, copy-free access to tracked coordinates using a
    /// <c>ReadOnlySpan</c>.
    /// </remarks>
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class CoordinateTracker
    {
        private readonly int[,] _coordToIdx;
        private readonly Coordinate[] _coords;
        private int _numAdded = 0;
        /// <summary>
        /// The number of coordinates currently considered to be 'tracked'.
        /// </summary>
        public int NumTracked { get; private set; }

        public int Size { get; }

        /// <summary>
        /// Indicates the action taken during an
        /// <see cref="AddOrTrackIfUntracked(in Coordinate)">AddOrTrackIfUntracked</see>
        /// operation.
        /// </summary>
        public enum AddOrTrackResult
        {
            /// <summary>
            /// The given <see cref="Coordinate"/> was previously unknown, and has been added and
            /// tracked.
            /// </summary>
            AddedAndTracked = 0,
            /// <summary>
            /// The given <see cref="Coordinate"/> was already added but was untracked, and is now
            /// tracked.
            /// </summary>
            Tracked = 1,
            /// <summary>
            /// The given <see cref="Coordinate"/> was already tracked. No changes were needed.
            /// </summary>
            Unchanged = 2
        }

        /// <summary>
        /// Constructs a <c>CoordinateTracker</c> to track coordinates within a
        /// <paramref name="size"/>-by-<paramref name="size"/> square.
        /// </summary>
        /// <param name="size">The side length of a square of valid coordinates.</param>
        public CoordinateTracker(int size)
        {
            _coordToIdx = new int[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    _coordToIdx[row, col] = -1;
                }
            }
            _coords = new Coordinate[size * size];
            Size = size;
        }

        /// <summary>
        /// Copy-constructor to provide a copy of the given CoordinateTracker.
        /// </summary>
        /// <param name="existing"></param>
        public CoordinateTracker(CoordinateTracker existing)
        {
            _coordToIdx = (int[,])existing._coordToIdx.Clone();
            _coords = (Coordinate[])existing._coords.Clone();
            NumTracked = existing.NumTracked;
            _numAdded = existing._numAdded;
        }

        /// <summary>
        /// Adds and tracks a previously unknown <see cref="Coordinate"/>. This must only be called
        /// once for any given Coordinate.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> to add.</param>
        public void Add(in Coordinate c)
        {
            _coords[_numAdded] = c;
            _coordToIdx[c.Row, c.Column] = _numAdded;
            _numAdded++;
            Track(in c);
        }

        /// <summary>
        /// Tracks a currently untracked <see cref="Coordinate"/>. The coordinate must not be
        /// tracked already.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> to track.</param>
        public void Track(in Coordinate c)
        {
            Debug.Assert(NumTracked != _numAdded, "The tracker is full.");
            int idx = _coordToIdx[c.Row, c.Column];
            _Track(in c, idx);
        }

        /// <summary>
        /// Ensures a given <see cref="Coordinate"/> is both added and tracked. Useful when callers
        /// are not certain of the status of the given <c>Coordinate</c>.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> to track.</param>
        /// <returns>The action that was taken.</returns>
        public AddOrTrackResult AddOrTrackIfUntracked(in Coordinate c)
        {
            int idx = _coordToIdx[c.Row, c.Column];
            if (idx == -1)
            {
                Add(in c);
                return AddOrTrackResult.AddedAndTracked;
            }
            if (idx < NumTracked)
            {
                return AddOrTrackResult.Unchanged;
            }
            _Track(in c, idx);
            return AddOrTrackResult.Tracked;
        }

        /// <summary>
        /// Untracks a given <see cref="Coordinate"/>. The <c>Coordinate</c> must be currently
        /// tracked.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> to untrack.</param>
        public void Untrack(in Coordinate c)
        {
            Debug.Assert(NumTracked > 0, "The tracker is empty");
            int idx = _coordToIdx[c.Row, c.Column];
            Debug.Assert(idx >= 0, $"Coordinate {c} was never added.");
            Debug.Assert(idx < NumTracked, $"Coordinate {c} is already untracked.");
            NumTracked--;
            Coordinate lastTrackedCoord = _coords[NumTracked];
            _coords[idx] = lastTrackedCoord;
            _coords[NumTracked] = c;
            _coordToIdx[lastTrackedCoord.Row, lastTrackedCoord.Column] = idx;
            _coordToIdx[c.Row, c.Column] = NumTracked;
        }

        /// <summary>
        /// Untracks all <see cref="Coordinate"/>s.
        /// </summary>
        public void UntrackAll() => NumTracked = 0;

        /// <summary>
        /// Provides readonly access to the currently tracked <see cref="Coordinate"/>s.
        /// </summary>
        public ReadOnlySpan<Coordinate> GetTrackedCoords() => new ReadOnlySpan<Coordinate>(_coords, 0, NumTracked);

        private void _Track(in Coordinate c, int index)
        {
            Debug.Assert(index >= 0, $"Coordinate {c} was never added.");
            Debug.Assert(index >= NumTracked, $"Coordinate {c} is already tracked.");
            Coordinate otherUntrackedCoord = _coords[NumTracked];
            _coords[index] = otherUntrackedCoord;
            _coords[NumTracked] = c;
            _coordToIdx[otherUntrackedCoord.Row, otherUntrackedCoord.Column] = index;
            _coordToIdx[c.Row, c.Column] = NumTracked;
            NumTracked++;
        }
    }
}