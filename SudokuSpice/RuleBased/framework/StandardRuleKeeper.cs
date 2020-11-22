using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Enforces the standard rules: uniqueness in each row, column, and box.
    /// </summary>
    public class StandardRuleKeeper : ISudokuRuleKeeper, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private readonly PossibleValues _possibleValues;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColumnValues;
        private readonly BitVector[] _unsetBoxValues;
        private IReadOnlyBoxPuzzle? _puzzle;

        /// <summary>
        /// Constructs rule keeper that enforces standard Sudoku rules.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use while solving.
        /// </param>
        public StandardRuleKeeper(PossibleValues possibleValues)
        {
            _possibleValues = possibleValues;
            int size = _possibleValues.Size;
            _unsetRowValues = new BitVector[size];
            _unsetColumnValues = new BitVector[size];
            _unsetBoxValues = new BitVector[size];
        }

        /// <summary>
        /// Tries to initialize this rule keeper to solve the given puzzle.
        /// 
        /// When reusing a rule keeper to solve multiple puzzles, this must be called with each new
        /// puzzle to be solved.
        ///
        /// The <see cref="StandardRuleKeeper"/> also requires that the puzzle implements
        /// <see cref="IReadOnlyBoxPuzzle"/>. It will return false if it doesn't.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the rule keeper's state is
        /// unchanged on failure.
        /// </remarks>
        /// <param name="puzzle">
        /// The puzzle to be solved. Must implement <see cref="IReadOnlyBoxPuzzle"/>
        /// </param>
        /// <returns>
        /// False if the rule keeper couldn't be initialized, for example if the puzzle already
        /// violates one of the rules. Else returns true.
        /// </returns>

        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            int size = _possibleValues.Size;
            if (puzzle.Size != size || puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                return false;
            }
            _puzzle = boxPuzzle;
            _unsetRowValues.AsSpan().Fill(_possibleValues.AllPossible);
            _unsetColumnValues.AsSpan().Fill(_possibleValues.AllPossible);
            _unsetBoxValues.AsSpan().Fill(_possibleValues.AllPossible);

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
                _possibleValues.Intersect(in c, _GetPossibleValues(in c));
                if (_possibleValues[in c].IsEmpty())
                {
                    return false;
                }
            }
            return true; 
        } 

        private StandardRuleKeeper(
            StandardRuleKeeper existing, IReadOnlyBoxPuzzle puzzle, PossibleValues possibleValues)
        {
            _unsetRowValues = existing._unsetRowValues.AsSpan().ToArray();
            _unsetColumnValues = existing._unsetColumnValues.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
            _puzzle = puzzle;
            _possibleValues = possibleValues;
        }

        /// <inheritdoc/>
        public ISudokuRuleKeeper CopyWithNewReferences(
            IReadOnlyPuzzle puzzle, PossibleValues possibleValues)
        {
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match current rule keeper size ({_possibleValues.Size})");
            Debug.Assert(puzzle.Size == possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match possible values size ({possibleValues.Size})");
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(StandardRuleKeeper)}.");
            }
            return new StandardRuleKeeper(this, boxPuzzle, possibleValues);
        }

        /// <inheritdoc/>
        public bool TrySet(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(TrySet)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot run rule checks for an already set puzzle coordinate");
            if (!_possibleValues[in c].IsBitSet(value))
            {
                return false;
            }
            _unsetRowValues[c.Row].UnsetBit(value);
            _unsetColumnValues[c.Column].UnsetBit(value);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
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
            for (int row = 0; row < size; row++)
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
                    _UnsetRowValues(in c, size);
                    return false;
                }
                _possibleValues[in workingCoord] = updatedPossibles;
            }
            foreach (Coordinate inBoxCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
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
        public IReadOnlyList<ISudokuRule> GetRules() => new List<ISudokuRule>() { this };

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues[row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int column) => _unsetColumnValues[column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        /// <inheritdoc/>
        public void Unset(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Unset)} with a null puzzle.");
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
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_GetPossibleValues)} with a null puzzle.");
            return BitVector.FindIntersect(
                _unsetRowValues[c.Row],
                BitVector.FindIntersect(
                    _unsetColumnValues[c.Column],
                    _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)]));
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
                _possibleValues[in workingCoord] = _GetPossibleValues(in workingCoord);
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
                _possibleValues[in workingCoord] = _GetPossibleValues(in workingCoord);
            }
        }

        private void _UnsetBoxValues(in Coordinate c, int boxIdx)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_UnsetBoxValues)} with a null puzzle.");
            foreach (Coordinate workingCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
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
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle) => throw new NotImplementedException();
    }
}
