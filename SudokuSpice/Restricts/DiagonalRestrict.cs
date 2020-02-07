using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    public class DiagonalRestrict : ISudokuRestrict
    {
        private readonly Puzzle _puzzle;
        private BitVector _unsetBackwardDiag;
        private BitVector _unsetForwardDiag;
        private readonly BitVector _allUnset;

        public DiagonalRestrict(Puzzle puzzle)
        {
            if (puzzle.Size > 32)
            {
                throw new ArgumentException("Max puzzle size is 32.");
            }
            _puzzle = puzzle;
            _allUnset = BitVector.CreateWithSize(puzzle.Size);
            _unsetForwardDiag = _unsetBackwardDiag = _allUnset;
            // Iterate through the backward diagonal (like a backslash '\')
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                var val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetBackwardDiag.IsBitSet(val.Value - 1))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    _unsetBackwardDiag.UnsetBit(val.Value - 1);
                }
            }
            // Iterate through the forward diagonal (like a forward slash '/')
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                var val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetForwardDiag.IsBitSet(val.Value - 1))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    _unsetForwardDiag.UnsetBit(val.Value - 1);
                }
            }
        }

        public BitVector GetPossibleValues(in Coordinate c)
        {
            if (_IsOnBackwardDiag(in c))
            {
                return _unsetBackwardDiag;
            } else if (_IsOnForwardDiag(in c))
            {
                return _unsetForwardDiag;
            } else
            {
                return _allUnset;
            }
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!_puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot update a restrict for an unset puzzle coordinate");
            }
            if (_IsOnBackwardDiag(in c))
            {
                _unsetBackwardDiag.UnsetBit(val - 1);
                _AddUnsetFromBackwardDiag(modifiedCoords);
            }
            if (_IsOnForwardDiag(in c))
            {
                _unsetForwardDiag.UnsetBit(val - 1);
                _AddUnsetFromForwardDiag(modifiedCoords);
            }
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!_puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot revert a restrict for an unset puzzle coordinate");
            }
            if (_IsOnBackwardDiag(in c))
            {
                _unsetBackwardDiag.SetBit(val - 1);
                _AddUnsetFromBackwardDiag(modifiedCoords);
            }
            if (_IsOnForwardDiag(in c))
            {
                _unsetForwardDiag.SetBit(val - 1);
                _AddUnsetFromForwardDiag(modifiedCoords);
            }
        }

        private static bool _IsOnBackwardDiag(in Coordinate c)
        {
            return c.Row == c.Column;
        }

        private bool _IsOnForwardDiag(in Coordinate c)
        {
            return c.Column == _puzzle.Size - c.Row - 1;
        }

        private void _AddUnsetFromBackwardDiag(IList<Coordinate> unsets)
        {
            for (int row = 0, col = 0; row < _puzzle.Size; row++, col++)
            {
                if (!_puzzle[row, col].HasValue)
                {
                    unsets.Add(new Coordinate(row, col));
                }
            }
        }

        private void _AddUnsetFromForwardDiag(IList<Coordinate> unsets)
        {
            for (int row = 0, col = _puzzle.Size - 1; row < _puzzle.Size; row++, col--)
            {
                if (!_puzzle[row, col].HasValue)
                {
                    unsets.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
