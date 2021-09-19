using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Simplifies the logic needed to implement a "max-count" rule, such as "each row must contain
    /// n copies of value m." In this case, the count n can vary for each value m.
    /// </summary>
    public abstract class MaxCountRule : IRule
    {
        private BitVector[]? _dimensions;
        private int[][]? _remainingCountOnDimension;
        private IReadOnlyDictionary<int, int>? _requiredCountPerValue;

        protected MaxCountRule() { }

        protected MaxCountRule(MaxCountRule existing)
        {
            _dimensions = existing._dimensions?.AsSpan().ToArray();
            if (existing._remainingCountOnDimension is not null)
            {
                _remainingCountOnDimension = new int[existing._remainingCountOnDimension.Length][];
                for (int d = 0; d < _remainingCountOnDimension.Length; ++d)
                {
                    _remainingCountOnDimension[d] = existing._remainingCountOnDimension[d].AsSpan().ToArray();
                }
            }
            _requiredCountPerValue = existing._requiredCountPerValue;
        }

        /// <inheritdoc/>
        public abstract IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle);

        /// <inheritdoc/>
        public virtual bool TryInit(IReadOnlyPuzzle puzzle, BitVector uniquePossibleValues)
        {
            int numDimensions = GetNumDimensions(puzzle);
            if (numDimensions != _dimensions?.Length)
            {
                _dimensions = new BitVector[numDimensions];
                _remainingCountOnDimension = new int[numDimensions][];
            }
            int maxValue = uniquePossibleValues.ComputeLastSetBit();
            Span<int> requiredCounts = stackalloc int[maxValue + 1];
            requiredCounts.Clear();
            foreach ((int value, int count) in puzzle.CountPerUniqueValue)
            {
                requiredCounts[value] = count;
            }
            for (int d = 0; d < numDimensions; ++d)
            {
                _remainingCountOnDimension![d] = requiredCounts.ToArray();
            }
            _dimensions.AsSpan().Fill(uniquePossibleValues);
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
                        // Puzzle has too many values on this dimension.
                        return false;
                    }
                    _SetValue(dimension, val.Value);
                }
            }
            _requiredCountPerValue = puzzle.CountPerUniqueValue;
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
            _UnsetValue(GetDimension(in c), val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(Revert)}.");
            int dimension = GetDimension(in c);
            _UnsetValue(dimension, val);
            TrackUnsetCoordinatesOnSameDimension(dimension, in c, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(Revert)}.");
            int dimension = GetDimension(in c);
            _SetValue(dimension, val);
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

        private bool _IsPossible(int dimension, int value)
        {
            Debug.Assert(_dimensions is not null, $"Must initialize before calling {nameof(_IsPossible)}.");
            return _dimensions[dimension].IsBitSet(value);
        }

        private void _SetValue(int dimension, int value)
        {
            Debug.Assert(_dimensions is not null && _remainingCountOnDimension is not null,
                         $"Must initialize before calling {nameof(_SetValue)}.");
            int count = --_remainingCountOnDimension![dimension][value];
            Debug.Assert(count >= 0, $"Value {value} is already at the max count on dimension {dimension}.");
            if (count == 0)
            {
                _dimensions[dimension].UnsetBit(value);
            }
        }
        private void _UnsetValue(int dimension, int value)
        {
            Debug.Assert(_dimensions is not null
                         && _remainingCountOnDimension is not null
                         && _requiredCountPerValue is not null,
                         $"Must initialize before calling {nameof(_UnsetValue)}.");
            int count = ++_remainingCountOnDimension[dimension][value];
            Debug.Assert(count <= _requiredCountPerValue[value],
                $"Value {value} on dimension {dimension} was unset more times than it was set.");
            _dimensions[dimension].SetBit(value);
        }
    }
}