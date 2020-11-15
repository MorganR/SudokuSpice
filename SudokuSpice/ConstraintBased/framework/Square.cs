using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a location in a puzzle, including tracking the current possible values at that
    /// location.
    /// </summary>
    public class Square<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        private readonly PossibleSquareValue<TPuzzle>?[] _possibleValues;
        private readonly Stack<PossibleSquareValue<TPuzzle>> _valuesDroppedOnSelect;
        private int? _selectedValueIndex;
        internal ReadOnlySpan<PossibleSquareValue<TPuzzle>?> AllPossibleValues => new ReadOnlySpan<PossibleSquareValue<TPuzzle>?>(_possibleValues);

        /// <summary>
        /// Gets the <see cref="Coordinate"/> of this square.
        /// </summary>
        public Coordinate Coordinate { get; }
        /// <summary>
        /// Gets the current number of possible values this square has.
        /// </summary>
        public int NumPossibleValues { get; internal set; }
        /// <summary>
        /// Gets whether or not this square's value is currently set.
        /// </summary>
        public bool IsSet => _selectedValueIndex != null;

        internal Square(Coordinate c, int numPossibleValues)
        {
            Coordinate = c;
            NumPossibleValues = numPossibleValues;
            _possibleValues = new PossibleSquareValue<TPuzzle>[numPossibleValues];
            for(int i = 0; i < NumPossibleValues; i++)
            {
                _possibleValues[i] = new PossibleSquareValue<TPuzzle>(this, i);
            }
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue<TPuzzle>>(NumPossibleValues);
        }

        private Square(Square<TPuzzle> other)
        {
            Coordinate = other.Coordinate;
            NumPossibleValues = other.NumPossibleValues;
            _possibleValues = new PossibleSquareValue<TPuzzle>[other._possibleValues.Length];
            for (int i = 0; i < _possibleValues.Length; i++)
            {
                var otherPossibleValue = other._possibleValues[i];
                if (otherPossibleValue is null || otherPossibleValue.State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                _possibleValues[i] = new PossibleSquareValue<TPuzzle>(this, i);
            }
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue<TPuzzle>>(NumPossibleValues);
        }

        internal Square<TPuzzle> CopyWithPossibleValues()
        {
            return new Square<TPuzzle>(this);
        }

        /// <summary>
        /// Gets the possible value with the given value-index.
        /// </summary>
        public PossibleSquareValue<TPuzzle>? GetPossibleValue(int index)
        {
            return _possibleValues[index];
        }

        internal PossibleSquareValue<TPuzzle>[] GetStillPossibleValues()
        {
            Debug.Assert(
                _selectedValueIndex is null,
                $"Can't retrieve possible values from Square at {Coordinate} when the index {_selectedValueIndex} is already selected.");
            var possibleValues = new PossibleSquareValue<TPuzzle>[NumPossibleValues];
            int i = 0;
            foreach (var possibleValue in _possibleValues)
            {
                if (possibleValue != null && possibleValue.State == PossibleSquareState.UNKNOWN)
                {
                    possibleValues[i] = possibleValue;
                    if (++i == NumPossibleValues)
                    {
                        return possibleValues;
                    }
                }
            }
            throw new ApplicationException($"Expected to find {NumPossibleValues} possible values but only found {i}.");
        }

        internal bool TrySet(int index)
        {
            Debug.Assert(_selectedValueIndex is null, $"Tried to set Square {Coordinate} to index {index} when value already set to index {_selectedValueIndex}.");
            Debug.Assert(_valuesDroppedOnSelect.Count == 0, $"Tried to set Square {Coordinate} to index {index} when {nameof(_valuesDroppedOnSelect)} was non-empty.");
            var possibleValue = _possibleValues[index];
            Debug.Assert(possibleValue != null, $"Tried to set square {Coordinate} to null possible value at index {index}.");
            if (!possibleValue.TrySelect())
            {
                return false;
            }
            foreach (var valueToDrop in _possibleValues)
            {
                if (valueToDrop is null || valueToDrop.State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                if (!valueToDrop.TryDrop()) {
                    _ReturnDroppedValues();
                    possibleValue.Deselect();
                    return false;
                }
               _valuesDroppedOnSelect.Push(valueToDrop);
            }
            _selectedValueIndex = index;
            return true;
        }

        internal void Unset()
        {
            Debug.Assert(_selectedValueIndex.HasValue, $"Tried to unset Square {Coordinate} when value was not set.");
            _ReturnDroppedValues();
            var valueToUnset = _possibleValues[_selectedValueIndex.Value];
            Debug.Assert(valueToUnset != null, $"Tried to unset square {Coordinate} but the possible value was null.");
            valueToUnset.Deselect();
            _selectedValueIndex = null;
        }

        private void _ReturnDroppedValues()
        {
            while (_valuesDroppedOnSelect.Count > 0)
            {
                var modifiedValue = _valuesDroppedOnSelect.Pop();
                modifiedValue.Return();
            }
        }
    }
}
