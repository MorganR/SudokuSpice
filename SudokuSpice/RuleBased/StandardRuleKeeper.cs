using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Enforces the standard rules: uniqueness in each row, column, and box.
    /// </summary>
    public class StandardRuleKeeper : IRuleKeeper, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private int _boxSize;
        private BitVector[]? _unsetRowValues;
        private BitVector[]? _unsetColumnValues;
        private BitVector[]? _unsetBoxValues;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;

        /// <summary>
        /// Constructs rule keeper that enforces standard Sudoku rules.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use while solving.
        /// </param>
        public StandardRuleKeeper() { }

        /// <inheritdoc/>
        public bool TryInit(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            int size = puzzle.Size;
            _puzzle = puzzle;
            _boxSize = Boxes.CalculateBoxSize(puzzle.Size);
            _unsetRowValues = new BitVector[size];
            _unsetRowValues.AsSpan().Fill(_puzzle.AllPossibleValues);
            Span<BitVector> possibleValues = _unsetRowValues.AsSpan();
            _unsetColumnValues = possibleValues.ToArray();
            _unsetBoxValues = possibleValues.ToArray();

            int boxIdx = 0;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / _boxSize) * _boxSize;
                    } else if (col % _boxSize == 0)
                    {
                        boxIdx++;
                    }
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetRowValues[row].IsBitSet(val.Value)
                        || !_unsetColumnValues[col].IsBitSet(val.Value)
                        || !_unsetBoxValues[boxIdx].IsBitSet(val.Value))
                    {
                        return false;
                    }
                    _unsetRowValues[row].UnsetBit(val.Value);
                    _unsetColumnValues[col].UnsetBit(val.Value);
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                _puzzle.IntersectPossibleValues(in c, _GetPossibleValues(in c));
                if (_puzzle.GetPossibleValues(in c).IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        private StandardRuleKeeper(
            StandardRuleKeeper existing, IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            _boxSize = existing._boxSize;
            _unsetRowValues = existing._unsetRowValues?.AsSpan().ToArray();
            _unsetColumnValues = existing._unsetColumnValues?.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues?.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public IRuleKeeper CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            Debug.Assert((puzzle is null && _puzzle is null) || puzzle?.Size == _puzzle?.Size,
                $"Puzzle size ({puzzle?.Size}) must match current rule keeper size ({_puzzle?.Size})");
            return new StandardRuleKeeper(this, puzzle);
        }

        /// <inheritdoc/>
        public bool TrySet(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null
                && _unsetRowValues is not null
                && _unsetColumnValues is not null
                && _unsetBoxValues is not null, $"Rule keeper must be initialized before calling {nameof(TrySet)}.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot run rule checks for an already set puzzle coordinate");
            if (!_puzzle.GetPossibleValues(in c).IsBitSet(value))
            {
                return false;
            }
            _unsetRowValues[c.Row].UnsetBit(value);
            _unsetColumnValues[c.Column].UnsetBit(value);
            int boxIdx = Boxes.CalculateBoxIndex(in c, _boxSize);
            _unsetBoxValues[boxIdx].UnsetBit(value);
            BitVector updatedPossibles;
            Coordinate workingCoord;
            int size = _puzzle.Size;
            for (int col = 0; col < size; col++)
            {
                workingCoord = new Coordinate(c.Row, col);
                if (col == c.Column || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                    _puzzle.GetPossibleValues(in workingCoord),
                    _unsetRowValues[c.Row]);
                if (updatedPossibles.IsEmpty())
                {
                    _unsetRowValues[c.Row].SetBit(value);
                    _unsetColumnValues[c.Column].SetBit(value);
                    _unsetBoxValues[boxIdx].SetBit(value);
                    _UnsetRowValues(in c, col);
                    return false;
                }
                _puzzle.SetPossibleValues(in workingCoord, updatedPossibles);
            }
            for (int row = 0; row < size; row++)
            {
                workingCoord = new Coordinate(row, c.Column);
                if (row == c.Row || _puzzle[row, c.Column].HasValue)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                    _puzzle.GetPossibleValues(in workingCoord),
                    _unsetColumnValues[c.Column]);
                if (updatedPossibles.IsEmpty())
                {
                    _unsetRowValues[c.Row].SetBit(value);
                    _unsetColumnValues[c.Column].SetBit(value);
                    _unsetBoxValues[boxIdx].SetBit(value);
                    _UnsetColumnValues(in c, row);
                    _UnsetRowValues(in c, size);
                    return false;
                }
                _puzzle.SetPossibleValues(in workingCoord, updatedPossibles);
            }
            foreach (Coordinate inBoxCoord in Boxes.YieldUnsetCoordsForBox(boxIdx, _boxSize, _puzzle))
            {
                if (inBoxCoord.Row == c.Row || inBoxCoord.Column == c.Column)
                {
                    continue;
                }
                updatedPossibles = BitVector.FindIntersect(
                        _puzzle.GetPossibleValues(in inBoxCoord),
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
                _puzzle.SetPossibleValues(in inBoxCoord, updatedPossibles);
            }
            return true;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IRule> GetRules() => new List<IRule>() { this };

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues![row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int column) => _unsetColumnValues![column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues![box];

        /// <inheritdoc/>
        public void Unset(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null
                && _unsetRowValues is not null
                && _unsetColumnValues is not null
                && _unsetBoxValues is not null, $"Rule keeper must be initialized before calling {nameof(Unset)}.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot undo rule checks for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(value);
            _unsetColumnValues[c.Column].SetBit(value);
            int boxIdx = Boxes.CalculateBoxIndex(in c, _boxSize);
            _unsetBoxValues[boxIdx].SetBit(value);
            _UnsetBoxValues(in c, boxIdx);
            _UnsetColumnValues(in c, _puzzle.Size);
            _UnsetRowValues(in c, _puzzle.Size);
        }

        private BitVector _GetPossibleValues(in Coordinate c)
        {
            return BitVector.FindIntersect(
                _unsetRowValues![c.Row],
                BitVector.FindIntersect(
                    _unsetColumnValues![c.Column],
                    _unsetBoxValues![Boxes.CalculateBoxIndex(in c, _boxSize)]));
        }

        private void _UnsetRowValues(in Coordinate c, int numToUnset)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_UnsetRowValues)} with a null puzzle.");
            Coordinate workingCoord;
            for (int col = numToUnset - 1; col >= 0; col--)
            {
                workingCoord = new Coordinate(c.Row, col);
                if (col == c.Column || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                _puzzle.SetPossibleValues(in workingCoord, _GetPossibleValues(in workingCoord));
            }
        }

        private void _UnsetColumnValues(in Coordinate c, int numToUnset)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_UnsetColumnValues)} with a null puzzle.");
            Coordinate workingCoord;
            for (int row = numToUnset - 1; row >= 0; row--)
            {
                workingCoord = new Coordinate(row, c.Column);
                if (row == c.Row || _puzzle[in workingCoord].HasValue)
                {
                    continue;
                }
                _puzzle.SetPossibleValues(in workingCoord, _GetPossibleValues(in workingCoord));
            }
        }

        private void _UnsetBoxValues(in Coordinate c, int boxIdx)
        {
            Debug.Assert(_puzzle is not null,
                $"Can't call {nameof(_UnsetBoxValues)} with a null puzzle.");
            foreach (Coordinate workingCoord in Boxes.YieldUnsetCoordsForBox(boxIdx, _boxSize, _puzzle))
            {
                if (workingCoord.Row == c.Row || workingCoord.Column == c.Column)
                {
                    continue;
                }
                _puzzle.SetPossibleValues(in workingCoord, _GetPossibleValues(in workingCoord));
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public BitVector GetPossibleValues(in Coordinate c) => throw new NotImplementedException();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Revert(in Coordinate c, int val) => throw new NotImplementedException();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker) => throw new NotImplementedException();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker) => throw new NotImplementedException();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public IRule CopyWithNewReference(IReadOnlyPossibleValues? puzzle) => throw new NotImplementedException();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public bool TryInit(IReadOnlyPossibleValues puzzle) => throw new NotImplementedException();
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) => throw new NotImplementedException();
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Always</exception>
        public bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues) => throw new NotImplementedException();
    }
}