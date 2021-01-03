using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Combines the standard rules (row, column, and box uniqueness) for efficiency and
    /// convenience.
    /// </summary>
    public class StandardRules : IRule, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private int _boxSize;
        private BitVector[]? _unsetRowValues;
        private BitVector[]? _unsetColValues;
        private BitVector[]? _unsetBoxValues;
        private IReadOnlyPuzzle? _puzzle;

        /// <summary>
        /// Constructs a single rule that enforces all the standard rules. This instance can be
        /// reused to solve multiple puzzles with the same <paramref name="size"/> and
        /// <paramref name="allUniqueValues"/>.
        /// </summary>
        /// <param name="allUniqueValues">
        /// The unique values to satisfy for each row, column, and box (eg. 1-9 for a standard
        /// puzzle).
        /// </param>
        /// 
        public StandardRules() { }

        private StandardRules(StandardRules existing, IReadOnlyPuzzle? puzzle)
        {
            _boxSize = existing._boxSize;
            _unsetRowValues = existing._unsetRowValues?.AsSpan().ToArray();
            _unsetColValues = existing._unsetColValues?.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues?.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
        {
            return new StandardRules(this, puzzle);
        }

        /// <inheritdoc/>
        public bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
        {
            int size = puzzle.Size;
            _boxSize = Boxes.CalculateBoxSize(size);
            if (size != _unsetRowValues?.Length)
            {
                _unsetRowValues = new BitVector[size];
                _unsetRowValues.AsSpan().Fill(allPossibleValues);
                _unsetColValues = _unsetRowValues.AsSpan().ToArray();
                _unsetBoxValues = _unsetRowValues.AsSpan().ToArray();
            } else
            {
                _unsetRowValues.AsSpan().Fill(allPossibleValues);
                _unsetColValues.AsSpan().Fill(allPossibleValues);
                _unsetBoxValues.AsSpan().Fill(allPossibleValues);
            }
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
                        || !_unsetColValues![col].IsBitSet(val.Value)
                        || !_unsetBoxValues![boxIdx].IsBitSet(val.Value))
                    {
                        return false;
                    }
                    _unsetRowValues[row].UnsetBit(val.Value);
                    _unsetColValues[col].UnsetBit(val.Value);
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues![row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int column) => _unsetColValues![column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues![box];

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(GetPossibleValues)} with a null puzzle.");
            return BitVector.FindIntersect(
                _unsetRowValues![c.Row],
                BitVector.FindIntersect(
                    _unsetColValues![c.Column],
                    _unsetBoxValues![Boxes.CalculateBoxIndex(in c, _boxSize)]));
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            _unsetRowValues![c.Row].SetBit(val);
            _unsetColValues![c.Column].SetBit(val);
            _unsetBoxValues![Boxes.CalculateBoxIndex(in c, _boxSize)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            _unsetRowValues![c.Row].SetBit(val);
            _unsetColValues![c.Column].SetBit(val);
            int boxIdx = Boxes.CalculateBoxIndex(in c, _boxSize);
            _unsetBoxValues![boxIdx].SetBit(val);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            _unsetRowValues![c.Row].UnsetBit(val);
            _unsetColValues![c.Column].UnsetBit(val);
            int boxIdx = Boxes.CalculateBoxIndex(in c, _boxSize);
            _unsetBoxValues![boxIdx].UnsetBit(val);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        private void _AddUnset(in Coordinate c, int boxIdx, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_AddUnset)} with a null puzzle.");
            int size = _puzzle.Size;
            for (int col = 0; col < size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(c.Row, col));
                }
            }
            for (int row = 0; row < size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(row, c.Column));
                }
            }
            foreach (Coordinate inBoxCoord in Boxes.YieldUnsetCoordsForBox(boxIdx, _boxSize, _puzzle))
            {
                if (inBoxCoord.Row == c.Row || inBoxCoord.Column == c.Column)
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(inBoxCoord);
            }
        }
    }
}