using System;
using System.Collections.Generic;

namespace MorganRoff.Sudoku
{
    public class DiagonalRestrict : IRestrict
    {
        private readonly Puzzle _puzzle;
        private int _unsetBackwardDiag;
        private int _unsetForwardDiag;
        private readonly int _allUnset;

        public DiagonalRestrict(Puzzle puzzle)
        {
            if (puzzle.Size > 32)
            {
                throw new ArgumentException("Max puzzle size is 32.");
            }
            _puzzle = puzzle;
            _allUnset = BitVectorUtils.CreateWithSize(puzzle.Size);
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
                    BitVectorUtils.UnsetBit(val.Value - 1, ref _unsetBackwardDiag);
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
                    BitVectorUtils.UnsetBit(val.Value - 1, ref _unsetForwardDiag);
                }
            }
        }

        public int GetPossibleValues(in Coordinate c)
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
                BitVectorUtils.UnsetBit(val - 1, ref _unsetBackwardDiag);
                _AddUnsetFromBackwardDiag(modifiedCoords);
            }
            else if (_IsOnForwardDiag(in c))
            {
                BitVectorUtils.UnsetBit(val - 1, ref _unsetForwardDiag);
                _AddUnsetFromForwardDiag(modifiedCoords);
            }
            // Else leave everything unchanged.
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> modifiedCoords)
        {
            if (!_puzzle[in c].HasValue)
            {
                throw new ArgumentException("Cannot revert a restrict for an unset puzzle coordinate");
            }
            if (_IsOnBackwardDiag(in c))
            {
                BitVectorUtils.SetBit(val - 1, ref _unsetBackwardDiag);
                _AddUnsetFromBackwardDiag(modifiedCoords);
            }
            else if (_IsOnForwardDiag(in c))
            {
                BitVectorUtils.SetBit(val - 1, ref _unsetForwardDiag);
                _AddUnsetFromForwardDiag(modifiedCoords);
            }
            // Else leave everything unchanged.
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
