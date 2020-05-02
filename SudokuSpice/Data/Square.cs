using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.Data
{
    public class Square
    {
        private readonly IReadOnlyDictionary<int, int> _valueToIndex;
        private readonly PossibleSquareValue[] _possibleValues;
        private readonly Stack<PossibleSquareValue> _valuesDroppedOnSelect;
        private int? _selectedValue;

        public Coordinate Coordinate { get; }
        public int NumPossibleValues { get; internal set; }
        public ReadOnlySpan<PossibleSquareValue> AllPossibleValues => new ReadOnlySpan<PossibleSquareValue>(_possibleValues);

        public Square(Coordinate c, IReadOnlyDictionary<int, int> valueToIndex)
        {
            Coordinate = c;
            NumPossibleValues = valueToIndex.Count;
            _possibleValues = new PossibleSquareValue[valueToIndex.Count];
            foreach ((int value, int index) in valueToIndex)
            {
                _possibleValues[index] = new PossibleSquareValue(this, value);
            }
            _valueToIndex = valueToIndex;
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue>(NumPossibleValues);
        }

        public PossibleSquareValue GetPossibleValue(int value)
        {
            return _possibleValues[_valueToIndex[value]];
        }

        public List<PossibleSquareValue> GetStillPossibleValues()
        {
            Debug.Assert(_selectedValue is null, $"Can't retrieve possible values from Square at {Coordinate} when the value {_selectedValue} is already selected.");
            return _possibleValues.Where(pv => pv.State == PossibleSquareState.UNKNOWN).ToList();
        }

        internal bool TrySetValue(int value)
        {
            Debug.Assert(_selectedValue is null, $"Tried to set Square {Coordinate} to value {value} when value already set to {_selectedValue}.");
            Debug.Assert(_valuesDroppedOnSelect.Count == 0, $"Tried to set Square {Coordinate} to value {value} when {nameof(_valuesDroppedOnSelect)} was non-empty.");
            if (!_possibleValues[_valueToIndex[value]].TrySelect())
            {
                return false;
            }
            foreach (var possibleValue in _possibleValues)
            {
                if (possibleValue.State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                if (!possibleValue.TryDrop()) {
                    _ReturnDroppedValues();
                    _possibleValues[_valueToIndex[value]].Deselect();
                    return false;
                }
               _valuesDroppedOnSelect.Push(possibleValue);
            }
            _selectedValue = value;
            return true;
        }

        internal void UnsetValue()
        {
            Debug.Assert(_selectedValue.HasValue, $"Tried to unset Square {Coordinate} when value was not set.");
            _ReturnDroppedValues();
            _possibleValues[_valueToIndex[_selectedValue.Value]].Deselect();
            _selectedValue = null;
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
