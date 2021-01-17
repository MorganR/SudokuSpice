using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a location in a puzzle, including tracking the current possible values at that
    /// location.
    /// </summary>
    public class Square
    {
        private readonly PossibleSquareValue?[] _possibleValues;
        private readonly Stack<PossibleSquareValue> _valuesDroppedOnSelect;
        private int? _selectedValueIndex;

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
            _possibleValues = new PossibleSquareValue[numPossibleValues];
            for (int i = 0; i < NumPossibleValues; i++)
            {
                _possibleValues[i] = new PossibleSquareValue(this, i);
            }
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue>(NumPossibleValues);
        }

        private Square(Square other)
        {
            Coordinate = other.Coordinate;
            NumPossibleValues = other.NumPossibleValues;
            _possibleValues = new PossibleSquareValue[other._possibleValues.Length];
            for (int i = 0; i < _possibleValues.Length; i++)
            {
                PossibleSquareValue? otherPossibleValue = other._possibleValues[i];
                if (otherPossibleValue is null || otherPossibleValue.State != PossibilityState.UNKNOWN)
                {
                    continue;
                }
                _possibleValues[i] = new PossibleSquareValue(this, i);
            }
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue>(NumPossibleValues);
        }

        internal Square CopyWithPossibleValues() => new Square(this);

        /// <summary>
        /// Gets the possible value with the given value-index.
        /// </summary>
        public PossibleSquareValue? GetPossibleValue(int index) => _possibleValues[index];

        internal PossibleSquareValue[] GetStillPossibleValues()
        {
            Debug.Assert(
                _selectedValueIndex is null,
                $"Can't retrieve possible values from Square at {Coordinate} when the index {_selectedValueIndex} is already selected.");
            var possibleValues = new PossibleSquareValue[NumPossibleValues];
            int i = 0;
            foreach (PossibleSquareValue? possibleValue in _possibleValues)
            {
                if (possibleValue is not null && possibleValue.State == PossibilityState.UNKNOWN)
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
            Debug.Assert(
                _selectedValueIndex is null,
                $"Tried to set Square {Coordinate} to index {index} when value already set to index {_selectedValueIndex}.");
            Debug.Assert(
                _valuesDroppedOnSelect.Count == 0,
                $"Tried to set Square {Coordinate} to index {index} when {nameof(_valuesDroppedOnSelect)} was non-empty.");
            PossibleSquareValue? possibleValue = _possibleValues[index];
            Debug.Assert(possibleValue != null, $"Tried to set square {Coordinate} to null possible value at index {index}.");
            if (!possibleValue.TrySelect())
            {
                return false;
            }
            foreach (PossibleSquareValue? valueToDrop in _possibleValues)
            {
                if (valueToDrop is null || valueToDrop.State != PossibilityState.UNKNOWN)
                {
                    continue;
                }
                if (!valueToDrop.TryDrop())
                {
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
            PossibleSquareValue? valueToUnset = _possibleValues[_selectedValueIndex.Value];
            Debug.Assert(valueToUnset != null, $"Tried to unset square {Coordinate} but the possible value was null.");
            valueToUnset.Deselect();
            _selectedValueIndex = null;
        }

        private void _ReturnDroppedValues()
        {
            while (_valuesDroppedOnSelect.Count > 0)
            {
                PossibleSquareValue? valueToReturn = _valuesDroppedOnSelect.Pop();
                valueToReturn.Return();
            }
        }
    }
}