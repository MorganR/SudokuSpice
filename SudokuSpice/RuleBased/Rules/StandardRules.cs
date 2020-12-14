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
        private BitVector[]? _unsetRowValues;
        private BitVector[]? _unsetColValues;
        private BitVector[]? _unsetBoxValues;
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
        public StandardRules() { }

        private StandardRules(StandardRules existing, IReadOnlyBoxPuzzle? puzzle)
        {
            _unsetRowValues = existing._unsetRowValues?.AsSpan().ToArray();
            _unsetColValues = existing._unsetColValues?.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues?.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
        {
            if (puzzle is IReadOnlyBoxPuzzle boxPuzzle)
            {
                return new StandardRules(this, boxPuzzle);
            }
            if (puzzle is null)
            {
                return new StandardRules(this, null);
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
        public bool TryInit(IReadOnlyPuzzle puzzle)
        {
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required by {nameof(StandardRules)}.");
            }
            int size = puzzle.Size;
            if (size != _unsetRowValues?.Length)
            {
                _unsetRowValues = new BitVector[size];
                _unsetRowValues.AsSpan().Fill(puzzle.AllPossibleValues);
                _unsetColValues = _unsetRowValues.AsSpan().ToArray();
                _unsetBoxValues = _unsetRowValues.AsSpan().ToArray();
            } else
            {
                _unsetRowValues.AsSpan().Fill(puzzle.AllPossibleValues);
                _unsetColValues.AsSpan().Fill(puzzle.AllPossibleValues);
                _unsetBoxValues.AsSpan().Fill(puzzle.AllPossibleValues);
            }
            int boxIdx = 0;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
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
            _puzzle = boxPuzzle;
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
                    _unsetBoxValues![_puzzle.GetBoxIndex(c.Row, c.Column)]));
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            _unsetRowValues![c.Row].SetBit(val);
            _unsetColValues![c.Column].SetBit(val);
            _unsetBoxValues![_puzzle!.GetBoxIndex(c.Row, c.Column)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            _unsetRowValues![c.Row].SetBit(val);
            _unsetColValues![c.Column].SetBit(val);
            int boxIdx = _puzzle!.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues![boxIdx].SetBit(val);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            _unsetRowValues![c.Row].UnsetBit(val);
            _unsetColValues![c.Column].UnsetBit(val);
            int boxIdx = _puzzle!.GetBoxIndex(c.Row, c.Column);
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