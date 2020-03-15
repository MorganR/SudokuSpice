using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Supports the standard rules: uniqueness in each row, column, and box.
    /// </summary>
    public class StandardRuleKeeper : ISudokuRuleKeeper, IRowRestrict, IColumnRestrict, IBoxRestrict
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColumnValues;
        private readonly BitVector[] _unsetBoxValues;

        public StandardRuleKeeper(Puzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _unsetRowValues = new BitVector[puzzle.Size];
            _unsetColumnValues = new BitVector[puzzle.Size];
            _unsetBoxValues = new BitVector[puzzle.Size];

            var allPossibleValues = BitVector.CreateWithSize(puzzle.Size);
            for (int i = 0; i < puzzle.Size; i++)
            {
                _unsetRowValues[i] = allPossibleValues;
                _unsetColumnValues[i] = allPossibleValues;
                _unsetBoxValues[i] = allPossibleValues;
            }

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
                    int bit = val.Value - 1;
                    if (!_unsetRowValues[row].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-row rule at ({row}, {col}).");
                    }
                    if (!_unsetColumnValues[col].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-column rule at ({row}, {col}).");
                    }
                    if (!_unsetBoxValues[boxIdx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle violates the unique-in-box rule at ({row}, {col}).");
                    }
                    _unsetRowValues[row].UnsetBit(bit);
                    _unsetColumnValues[col].UnsetBit(bit);
                    _unsetBoxValues[boxIdx].UnsetBit(bit);
                }
            }
            foreach (var c in puzzle.GetUnsetCoords())
            {
                _possibleValues.Intersect(in c, _GetPossibleValues(in c));
            }
        }

        private StandardRuleKeeper(StandardRuleKeeper existing, Puzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _unsetRowValues = (BitVector[])existing._unsetRowValues.Clone();
            _unsetColumnValues = (BitVector[])existing._unsetColumnValues.Clone();
            _unsetBoxValues = (BitVector[])existing._unsetBoxValues.Clone();
        }

        public ISudokuRuleKeeper CopyWithNewReferences(Puzzle puzzle, PossibleValues possibleValues)
        {
            return new StandardRuleKeeper(this, puzzle, possibleValues);
        }

        public bool TrySet(in Coordinate c, int value)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot run rule checks for an already set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(value - 1);
            _unsetColumnValues[c.Column].UnsetBit(value - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].UnsetBit(value - 1);
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
                    _unsetRowValues[c.Row].SetBit(value - 1);
                    _unsetColumnValues[c.Column].SetBit(value - 1);
                    _unsetBoxValues[boxIdx].SetBit(value - 1);
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
                    _unsetRowValues[c.Row].SetBit(value - 1);
                    _unsetColumnValues[c.Column].SetBit(value - 1);
                    _unsetBoxValues[boxIdx].SetBit(value - 1);
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
                    _unsetRowValues[c.Row].SetBit(value - 1);
                    _unsetColumnValues[c.Column].SetBit(value - 1);
                    _unsetBoxValues[boxIdx].SetBit(value - 1);
                    _UnsetBoxValues(in c, boxIdx);
                    _UnsetColumnValues(in c, _puzzle.Size);
                    _UnsetRowValues(in c, _puzzle.Size);
                    return false;
                }
                _possibleValues[in inBoxCoord] = updatedPossibles;
            }
            return true;
        }

        public IReadOnlyList<ISudokuRestrict> GetRestricts()
        {
            return new List<ISudokuRestrict>() { this };
        }

        public BitVector GetPossibleRowValues(int row) => _unsetRowValues[row];

        public BitVector GetPossibleColumnValues(int column) => _unsetColumnValues[column];

        public BitVector GetPossibleBoxValues(int box) => _unsetBoxValues[box];

        public void Unset(in Coordinate c, int value)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot undo rule checks for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(value - 1);
            _unsetColumnValues[c.Column].SetBit(value - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].SetBit(value - 1);
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

        public BitVector GetPossibleValues(in Coordinate c)
        {
            throw new NotImplementedException();
        }

        public void Revert(in Coordinate c, int val)
        {
            throw new NotImplementedException();
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            throw new NotImplementedException();
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            throw new NotImplementedException();
        }

        public ISudokuRestrict CopyWithNewReference(Puzzle puzzle)
        {
            throw new NotImplementedException();
        }
    }
}
