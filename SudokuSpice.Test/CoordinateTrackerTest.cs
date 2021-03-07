using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    public class CoordinateTrackerTest
    {
        [Fact]
        public void CopyConstructor_CreatesDeepCopy()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            tracker.Add(new Coordinate(0, 0));

            var trackerCopy = new CoordinateTracker(tracker);
            trackerCopy.Add(new Coordinate(1, 1));

            Assert.Equal(1, tracker.NumTracked);
            Assert.Equal(2, trackerCopy.NumTracked);
            Assert.Equal(new Coordinate[] { new Coordinate(0, 0) }, tracker.TrackedCoords.ToArray());
            Assert.Equal(new HashSet<Coordinate> { new Coordinate(0, 0), new Coordinate(1, 1) },
                new HashSet<Coordinate>(trackerCopy.TrackedCoords.ToArray()));
        }

        [Fact]
        public void Add_UpToSize_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);

            foreach (Coordinate c in coords)
            {
                tracker.Add(c);
            }

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(coords.Count, trackedCoords.Length);
            for (int i = 0; i < coords.Count; i++)
            {
                Assert.Equal(coords[i], trackedCoords[i]);
            }
        }

        [Fact]
        public void Add_OverSize_Throws()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength + 1);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                foreach (Coordinate c in coords)
                {
                    tracker.Add(c);
                }
            });
        }

        [Fact]
        public void Untrack_One_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);
            foreach (Coordinate c in coords)
            {
                tracker.Add(c);
            }
            var untrackedCoord = new Coordinate(0, 1);

            tracker.Untrack(in untrackedCoord);

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(coords.Count - 1, trackedCoords.Length);
            Assert.DoesNotContain(untrackedCoord, trackedCoords.ToArray());
        }

        [Fact]
        public void Untrack_All_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);
            foreach (Coordinate c in coords)
            {
                tracker.Add(c);
            }

            foreach (Coordinate c in coords)
            {
                tracker.Untrack(in c);
            }

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(0, trackedCoords.Length);
        }

        [Fact]
        public void UntrackAll_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);
            for (int i = 0; i < coords.Count / 2; i++)
            {
                tracker.Add(coords[i]);
            }

            tracker.UntrackAll();

            Assert.Equal(0, tracker.NumTracked);
            Assert.Equal(0, tracker.TrackedCoords.Length);

            foreach (Coordinate c in coords)
            {
                tracker.AddOrTrackIfUntracked(in c);
            }

            Assert.Equal(coords.Count, tracker.NumTracked);
            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(coords.Count, trackedCoords.Length);
            Assert.Equal(
                new HashSet<Coordinate>(coords), new HashSet<Coordinate>(trackedCoords.ToArray()));
        }


        [Fact]
        public void Track_One_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);
            foreach (Coordinate c in coords)
            {
                tracker.Add(c);
            }
            var trackedCoord = new Coordinate(0, 1);

            tracker.Untrack(in trackedCoord);
            tracker.Track(in trackedCoord);

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(coords.Count, trackedCoords.Length);
            Assert.Contains(trackedCoord, trackedCoords.ToArray());
        }

        [Fact]
        public void MixedTrackAndUntrack_Succeeds()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);
            IList<Coordinate> coords = _CreateCoordinateListForLength(sideLength);
            foreach (Coordinate c in coords)
            {
                tracker.Add(c);
            }
            int numFirstUntracked = 4;
            int numRetracked = 2;

            for (int i = 0; i < numFirstUntracked; i++)
            {
                tracker.Untrack(coords[i]);
            }
            for (int i = 0; i < numRetracked; i++)
            {
                tracker.Track(coords[i]);
            }

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(7, trackedCoords.Length);
            Assert.Contains(coords[0], trackedCoords.ToArray());
            Assert.Contains(coords[1], trackedCoords.ToArray());
            Assert.DoesNotContain(coords[2], trackedCoords.ToArray());
            Assert.DoesNotContain(coords[3], trackedCoords.ToArray());
        }

        [Fact]
        public void AddOrTrackIfUntracked_WithUnadded_ReturnsAddedAndTracked()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);

            var coord = new Coordinate(1, 1);
            Assert.Equal(
                CoordinateTracker.AddOrTrackResult.AddedAndTracked,
                tracker.AddOrTrackIfUntracked(coord));

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(1, trackedCoords.Length);
            Assert.Contains(coord, trackedCoords.ToArray());
        }

        [Fact]
        public void AddOrTrackIfUntracked_WithUntracked_ReturnsTracked()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);

            var coord = new Coordinate(1, 1);
            tracker.Add(coord);
            tracker.Untrack(coord);

            Assert.Equal(
                CoordinateTracker.AddOrTrackResult.Tracked,
                tracker.AddOrTrackIfUntracked(coord));

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(1, trackedCoords.Length);
            Assert.Contains(coord, trackedCoords.ToArray());
        }

        [Fact]
        public void AddOrTrackIfUntracked_WithTracked_ReturnsUnchanged()
        {
            int sideLength = 3;
            var tracker = new CoordinateTracker(sideLength);

            var coord = new Coordinate(1, 1);
            tracker.Add(coord);

            Assert.Equal(
                CoordinateTracker.AddOrTrackResult.Unchanged,
                tracker.AddOrTrackIfUntracked(coord));

            ReadOnlySpan<Coordinate> trackedCoords = tracker.TrackedCoords;
            Assert.Equal(1, trackedCoords.Length);
            Assert.Contains(coord, trackedCoords.ToArray());
        }

        private IList<Coordinate> _CreateCoordinateListForLength(int sideLength)
        {
            var coordinates = new List<Coordinate>(sideLength * sideLength);
            for (int row = 0; row < sideLength; row++)
            {
                for (int col = 0; col < sideLength; col++)
                {
                    coordinates.Add(new Coordinate(row, col));
                }
            }
            return coordinates;
        }
    }
}