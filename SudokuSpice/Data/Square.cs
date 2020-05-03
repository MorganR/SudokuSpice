using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.Data
{
    public class Square
    {
        private readonly PossibleSquareValue[] _possibleValues;
        private readonly Stack<PossibleSquareValue> _valuesDroppedOnSelect;
        private int? _selectedValueIndex;

        public Coordinate Coordinate { get; }
        public int NumPossibleValues { get; internal set; }
        public ReadOnlySpan<PossibleSquareValue> AllPossibleValues => new ReadOnlySpan<PossibleSquareValue>(_possibleValues);

        public Square(Coordinate c, int numPossibleValues)
        {
            Coordinate = c;
            NumPossibleValues = numPossibleValues;
            _possibleValues = new PossibleSquareValue[numPossibleValues];
            for(int i = 0; i < NumPossibleValues; i++)
            {
                _possibleValues[i] = new PossibleSquareValue(this, i);
            }
            _valuesDroppedOnSelect = new Stack<PossibleSquareValue>(NumPossibleValues);
        }

        public PossibleSquareValue GetPossibleValue(int index)
        {
            return _possibleValues[index];
        }

        public PossibleSquareValue[] GetStillPossibleValues()
        {
            Debug.Assert(_selectedValueIndex is null, $"Can't retrieve possible values from Square at {Coordinate} when the index {_selectedValueIndex} is already selected.");
            var possibleValues = new PossibleSquareValue[NumPossibleValues];
            int i = 0;
            foreach (var possibleValue in _possibleValues)
            {
                if (possibleValue.State == PossibleSquareState.UNKNOWN)
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
            if (!_possibleValues[index].TrySelect())
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
                    _possibleValues[index].Deselect();
                    return false;
                }
               _valuesDroppedOnSelect.Push(possibleValue);
            }
            _selectedValueIndex = index;
            return true;
        }

        internal void Unset()
        {
            Debug.Assert(_selectedValueIndex.HasValue, $"Tried to unset Square {Coordinate} when value was not set.");
            _ReturnDroppedValues();
            _possibleValues[_selectedValueIndex.Value].Deselect();
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
