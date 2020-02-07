using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    public abstract class BasicRestrict : ISudokuRestrict
    {
        protected readonly Puzzle puzzle;
        protected readonly BitVector[] unsetValues;

        public BasicRestrict(Puzzle puzzle)
        {
            if (puzzle.Size > 32)
            {
                throw new ArgumentException("Max puzzle size is 32.");
            }
            this.puzzle = puzzle;
            unsetValues = new BitVector[puzzle.Size];
            for (int i = 0; i < puzzle.Size; i++)
            {
                unsetValues[i] = BitVector.CreateWithSize(puzzle.Size);
            }
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    int bit = val.Value - 1;
                    int idx = GetIndex(new Coordinate(row, col));
                    if (!unsetValues[idx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    unsetValues[idx].UnsetBit(bit);
                }
            }
        }

        public BitVector GetPossibleValues(in Coordinate c)
        {
            return unsetValues[GetIndex(in c)];
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot update a restrict for an unset puzzle coordinate");
            }
            int idx = GetIndex(in c);
            unsetValues[idx].UnsetBit(val - 1);
            AddUnsetFromIndex(idx, modifiedCoords);
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot revert a restrict for an unset puzzle coordinate");
            }
            int idx = GetIndex(in c);
            unsetValues[idx].SetBit(val - 1);
            AddUnsetFromIndex(idx, modifiedCoords);
        }

        /// <summary>
        /// Gets an internal index for the given coordinate. This index corresponds with the
        /// indices of <c>_unsetValues</c>.
        /// </summary>
        protected abstract int GetIndex(in Coordinate c);

        /// <summary>
        /// Appends the unset values from the given internal index to the list.
        /// </summary>
        /// <param name="idx">An implementation specific internal index.</param>
        /// <param name="unsetCoords">A list to append the unset coordinates to.</param>
        protected abstract void AddUnsetFromIndex(int idx, IList<Coordinate> unsetCoords);
    }
}
