using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    public abstract class BasicRestrict : ISudokuRestrict
    {
        protected Puzzle Puzzle { get; }

        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is intended to be modified by derived classes. It's marked internal to keep scope limited.")]
        protected internal BitVector[] UnsetValues { get; }

        internal BasicRestrict(Puzzle puzzle)
        {
            Puzzle = puzzle;
            UnsetValues = new BitVector[puzzle.Size];
            for (int i = 0; i < puzzle.Size; i++)
            {
                UnsetValues[i] = BitVector.CreateWithSize(puzzle.Size);
            }
        }

        public BitVector GetPossibleValues(in Coordinate c)
        {
            return UnsetValues[GetIndex(in c)];
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!Puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot update a restrict for an unset puzzle coordinate");
            }
            int idx = GetIndex(in c);
            UnsetValues[idx].UnsetBit(val - 1);
            AddUnsetFromIndex(idx, modifiedCoords);
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!Puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot revert a restrict for an unset puzzle coordinate");
            }
            int idx = GetIndex(in c);
            UnsetValues[idx].SetBit(val - 1);
            AddUnsetFromIndex(idx, modifiedCoords);
        }

        /// <summary>
        /// Gets an internal index for the given coordinate. This index corresponds with the
        /// indices of <c>_unsetValues</c>.
        /// </summary>
        protected internal abstract int GetIndex(in Coordinate c);

        /// <summary>
        /// Appends the unset values from the given internal index to the list.
        /// </summary>
        /// <param name="idx">An implementation specific internal index.</param>
        /// <param name="unsetCoords">A list to append the unset coordinates to.</param>
        protected internal abstract void AddUnsetFromIndex(int idx, IList<Coordinate> unsetCoords);
    }
}
