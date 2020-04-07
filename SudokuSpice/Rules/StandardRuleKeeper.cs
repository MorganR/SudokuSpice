using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Enforces the standard rules: uniqueness in each row, column, and box.
    /// </summary>
    public class StandardRuleKeeper : ISudokuRuleKeeper, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private readonly IReadOnlyBoxPuzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColumnValues;
        private readonly BitVector[] _unsetBoxValues;

        public StandardRuleKeeper(IReadOnlyBoxPuzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _unsetRowValues = new BitVector[puzzle.Size];
            _unsetRowValues.AsSpan().Fill(possibleValues.AllPossible);
            _unsetColumnValues = _unsetRowValues.AsSpan().ToArray();
            _unsetBoxValues = _unsetRowValues.AsSpan().ToArray();

            int boxIdx = 0;
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / puzzle.BoxSize) * puzzle.BoxSize;
                    }
                    else if (col % puzzle.BoxSize == 0)
                    {
                        boxIdx++;
                    }
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetRowValues[row].IsBitSet(val.Value))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-row rule at ({row}, {col}).");
                    }
                    if (!_unsetColumnValues[col].IsBitSet(val.Value))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-column rule at ({row}, {col}).");
                    }
                    if (!_unsetBoxValues[boxIdx].IsBitSet(val.Value))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-box rule at ({row}, {col}).");
                    }
                    _unsetRowValues[row].UnsetBit(val.Value);
                    _unsetColumnValues[col].UnsetBit(val.Value);
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
            foreach (var c in puzzle.GetUnsetCoords())
            {
                _possibleValues.Intersect(in c, _GetPossibleValues(in c));
            }
        }

        private StandardRuleKeeper(
            StandardRuleKeeper existing, IReadOnlyBoxPuzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _unsetRowValues = existing._unsetRowValues.AsSpan().ToArray();
            _unsetColumnValues = existing._unsetColumnValues.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
        }

        /// <inheritdoc/>
        public ISudokuRuleKeeper CopyWithNewReferences(
            IReadOnlyPuzzle puzzle, PossibleValues possibleValues)
        {
            if (puzzle is IReadOnlyBoxPuzzle boxPuzzle)
            {
                return new StandardRuleKeeper(this, boxPuzzle, possibleValues);
            }
            throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(StandardRuleKeeper)}.");
        }

        /// <inheritdoc/>
        public bool TrySet(in Coordinate c, int value)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot run rule checks for an already set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(value);
            _unsetColumnValues[c.Column].UnsetBit(value);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].UnsetBit(value);
            BitVector updatedPossibles;
            Coordinate workingCoord;
            for (int col = 0; col < _puzzle.Size; col++)
            {
                workingCoord = new Coordinate(c.Row, col);
                if (col == c.Column || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                    _possibleValues[in workingCoord],
                    _unsetRowValues[c.Row]);
                if (updatedPossibles.IsEmpty())
                {
                    _unsetRowValues[c.Row].SetBit(value);
                    _unsetColumnValues[c.Column].SetBit(value);
                    _unsetBoxValues[boxIdx].SetBit(value);
                    _UnsetRowValues(in c, col);
                    return false;
                }
                _possibleValues[in workingCoord] = updatedPossibles;
            }
            for (int row = 0; row < _puzzle.Size; row++)
            {
                workingCoord = new Coordinate(row, c.Column);
                if (row == c.Row || _puzzle[row, c.Column].HasValue)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                    _possibleValues[in workingCoord],
                    _unsetColumnValues[c.Column]);
                if (updatedPossibles.IsEmpty())
                {
                    _unsetRowValues[c.Row].SetBit(value);
                    _unsetColumnValues[c.Column].SetBit(value);
                    _unsetBoxValues[boxIdx].SetBit(value);
                    _UnsetColumnValues(in c, row);
                    _UnsetRowValues(in c, _puzzle.Size);
                    return false;
                }
                _possibleValues[in workingCoord] = updatedPossibles;
            }
            foreach (var inBoxCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
            {
                if (inBoxCoord.Row == c.Row || inBoxCoord.Column == c.Column)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                        _possibleValues[in inBoxCoord],
                        _unsetBoxValues[boxIdx]);
                if (updatedPossibles.IsEmpty())
                {
                    _unsetRowValues[c.Row].SetBit(value);
                    _unsetColumnValues[c.Column].SetBit(value);
                    _unsetBoxValues[boxIdx].SetBit(value);
                    _UnsetBoxValues(in c, boxIdx);
                    _UnsetColumnValues(in c, _puzzle.Size);
                    _UnsetRowValues(in c, _puzzle.Size);
                    return false;
                }
                _possibleValues[in inBoxCoord] = updatedPossibles;
            }
            return true;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISudokuRule> GetRules()
        {
            return new List<ISudokuRule>() { this };
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues[row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int column) => _unsetColumnValues[column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        /// <inheritdoc/>
        public void Unset(in Coordinate c, int value)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot undo rule checks for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(value);
            _unsetColumnValues[c.Column].SetBit(value);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].SetBit(value);
            _UnsetBoxValues(in c, boxIdx);
            _UnsetColumnValues(in c, _puzzle.Size);
            _UnsetRowValues(in c, _puzzle.Size);
        }

        private BitVector _GetPossibleValues(in Coordinate c)
        {
            return BitVector.FindIntersect(
                _unsetRowValues[c.Row],
                BitVector.FindIntersect(
                    _unsetColumnValues[c.Column],
                    _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)]));
        }

        private void _UnsetRowValues(in Coordinate c, int numToUnset)
        {
            Coordinate workingCoord;
            for (var col = numToUnset - 1; col >= 0; col--)
            {
                workingCoord = new Coordinate(c.Row, col);
                if (col == c.Column || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                _possibleValues[in workingCoord] = _GetPossibleValues(in workingCoord);
            }
        }

        private void _UnsetColumnValues(in Coordinate c, int numToUnset)
        {
            Coordinate workingCoord;
            for (var row = numToUnset - 1; row >= 0; row--)
            {
                workingCoord = new Coordinate(row, c.Column);
                if (row == c.Row || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                _possibleValues[in workingCoord] = _GetPossibleValues(in workingCoord);
            }
        }

        private void _UnsetBoxValues(in Coordinate c, int boxIdx)
        {
            foreach (var workingCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
            {
                if (workingCoord.Row == c.Row || workingCoord.Column == c.Column)
                {
                    continue;
                }
                _possibleValues[in workingCoord] = _GetPossibleValues(in workingCoord);
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Revert(in Coordinate c, int val)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            throw new NotImplementedException();
        }
    }
}
