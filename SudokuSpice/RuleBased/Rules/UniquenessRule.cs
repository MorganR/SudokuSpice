using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Simplifies the logic needed to implement a uniqueness rule, such as "all values in a row
    /// must be unique."
    /// </summary>
    public abstract class UniquenessRule : IRule
    {
        private BitVector[]? _dimensions;

        protected UniquenessRule() { }

        protected UniquenessRule(UniquenessRule existing)
        {
            _dimensions = existing._dimensions?.AsSpan().ToArray();
        }

        /// <inheritdoc/>
        public abstract IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle);

        /// <inheritdoc/>
        public virtual bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
        {
            int numDimensions = GetNumDimensions(puzzle);
            if (numDimensions != _dimensions?.Length)
            {
                _dimensions = new BitVector[numDimensions];
            }
            _dimensions.AsSpan().Fill(allPossibleValues);
            int size = puzzle.Size;
            for (int row = 0; row < size; ++row)
            {
                for (int col = 0; col < size; ++col)
                {
                    int dimension = GetDimension(new(row, col));
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_IsPossible(dimension, val.Value))
                    {
                        // Puzzle has duplicate value on this dimension.
                        return false;
                    }
                    _RemovePossible(dimension, val.Value);
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(GetPossibleValues)}.");
            return _dimensions[GetDimension(in c)];
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(Revert)}.");
            _dimensions[GetDimension(in c)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(Revert)}.");
            int dimension = GetDimension(in c);
            _dimensions[dimension].SetBit(val);
            TrackUnsetCoordinatesOnSameDimension(dimension, in c, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(Revert)}.");
            int dimension = GetDimension(in c);
            _dimensions[dimension].UnsetBit(val);
            TrackUnsetCoordinatesOnSameDimension(dimension, in c, coordTracker);
        }

        /// <summary>
        /// Returns the number of dimensions that will be enforced to contain unique values.
        /// </summary>
        /// <param name="puzzle">The puzzle for which to determine the number of dimensions.</param>
        protected abstract int GetNumDimensions(IReadOnlyPuzzle puzzle);
        /// <summary>
        /// Gets the dimension for a coordinate.
        /// </summary>
        protected abstract int GetDimension(in Coordinate c);
        /// <summary>
        /// Adds the coordinates of unset values on the given dimension to the given tracker.
        /// </summary>
        /// <param name="dimension">The dimension to search.</param>
        /// <param name="source">The source coordinate for the change; should be skipped.</param>
        /// <param name="tracker">The tracker to add coordiantes to.</param>
        protected abstract void TrackUnsetCoordinatesOnSameDimension(int dimension, in Coordinate source, CoordinateTracker tracker);

        protected BitVector GetPossibleValues(int dimension)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(GetPossibleValues)}.");
            return _dimensions[dimension];
        }

        private bool _IsPossible(int dimension, int possible)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(_IsPossible)}.");
            return _dimensions[dimension].IsBitSet(possible);
        }

        private void _RemovePossible(int dimension, int possible)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(_RemovePossible)}.");
            _dimensions[dimension].UnsetBit(possible);
        }
    }
}