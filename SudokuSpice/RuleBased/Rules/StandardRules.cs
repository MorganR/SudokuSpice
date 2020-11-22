using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Combines the standard rules (row, column, and box uniqueness) for efficiency and
    /// convenience.
    /// </summary>
    public class StandardRules : ISudokuRule, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private readonly int _size;
        private readonly BitVector _allUniqueValues;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColValues;
        private readonly BitVector[] _unsetBoxValues;
        private IReadOnlyBoxPuzzle? _puzzle;

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
        public StandardRules(BitVector allUniqueValues)
        {
            _size = allUniqueValues.Count;
            _allUniqueValues = allUniqueValues;
            _unsetRowValues = new BitVector[_size];
            _unsetColValues = new BitVector[_size];
            _unsetBoxValues = new BitVector[_size];
        }

        private StandardRules(StandardRules existing, IReadOnlyBoxPuzzle puzzle)
        {
            _size = existing._size;
            _allUniqueValues = existing._allUniqueValues;
            _unsetRowValues = existing._unsetRowValues.AsSpan().ToArray();
            _unsetColValues = existing._unsetColValues.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            if (puzzle is IReadOnlyBoxPuzzle boxPuzzle)
            {
                return new StandardRules(this, boxPuzzle);
            }
            throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(StandardRules)}.");
        }

        /// <summary>
        /// Tries to initialize this rule to prepare to solve the given puzzle.
        /// </summary>
        /// <param name="puzzle">
        /// The puzzle to be solved. Must implement <see cref="IReadOnlyBoxPuzzle"/>, else this
        /// fails and returns false.
        /// </param>
        /// <returns>
        /// False if the puzzle violates this rule and initialization fails, else true.
        /// </returns>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            // This should be enforced by the rule keeper.
            Debug.Assert(puzzle.Size == _size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_size}).");
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required by {nameof(StandardRules)}.");
            }
            _unsetRowValues.AsSpan().Fill(_allUniqueValues);
            _unsetColValues.AsSpan().Fill(_allUniqueValues);
            _unsetBoxValues.AsSpan().Fill(_allUniqueValues);
            int boxIdx = 0;
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / boxPuzzle.BoxSize) * boxPuzzle.BoxSize;
                    } else if (col % boxPuzzle.BoxSize == 0)
                    {
                        boxIdx++;
                    }
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetRowValues[row].IsBitSet(val.Value)
                        || !_unsetColValues[col].IsBitSet(val.Value)
                        || !_unsetBoxValues[boxIdx].IsBitSet(val.Value))
                    {
                        return false;
                    }
                    _unsetRowValues[row].UnsetBit(val.Value);
                    _unsetColValues[col].UnsetBit(val.Value);
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
            _puzzle = boxPuzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues[row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int column) => _unsetColValues[column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(GetPossibleValues)} with a null puzzle.");
            return BitVector.FindIntersect(
                _unsetRowValues[c.Row],
                BitVector.FindIntersect(
                    _unsetColValues[c.Column],
                    _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)]));
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val);
            _unsetColValues[c.Column].SetBit(val);
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val);
            _unsetColValues[c.Column].SetBit(val);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].SetBit(val);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(val);
            _unsetColValues[c.Column].UnsetBit(val);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].UnsetBit(val);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        private void _AddUnset(in Coordinate c, int boxIdx, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_AddUnset)} with a null puzzle.");
            for (int col = 0; col < _size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(c.Row, col));
                }
            }
            for (int row = 0; row < _size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(row, c.Column));
                }
            }
            foreach (Coordinate inBoxCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
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
