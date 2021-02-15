﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.RuleBased.Rules
{
    public class MagicSquaresRule : IRule
    {
        private class BoxPossibleValues
        {
            private readonly BitVector _allPossibleValues;
            private readonly Square _box;
            private readonly IReadOnlyPuzzle _puzzle;
            private readonly IReadOnlySet<BitVector> _allPossibleSets;
            private readonly BitVector[] _setOnRows;
            private readonly BitVector[] _setOnColumns;
            private readonly bool _includeDiagonals;

            private readonly HashSet<BitVector>[] _rowPossibleSets;
            private readonly HashSet<BitVector>[] _columnPossibleSets;
            private readonly HashSet<BitVector>? _forwardDiagonalPossibleSets;
            private readonly HashSet<BitVector>? _backwardDiagonalPossibleSets;

            private BitVector _setOnForwardDiagonal;
            private BitVector _setOnBackwardDiagonal;

            public BoxPossibleValues(Square box, BitVector allPossibleValues, IReadOnlySet<BitVector> allPossibleSets, IReadOnlyPuzzle puzzle, bool includeDiagonals)
            {
                _allPossibleValues = allPossibleValues;
                _box = box;
                _puzzle = puzzle;
                _allPossibleSets = allPossibleSets;
                _includeDiagonals = includeDiagonals;

                _setOnRows = new BitVector[box.Size];
                _setOnColumns = new BitVector[box.Size];
                _rowPossibleSets = new HashSet<BitVector>[box.Size];
                _columnPossibleSets = new HashSet<BitVector>[box.Size];
                for (int i = 0; i < box.Size; ++i)
                {
                    _rowPossibleSets[i] = new HashSet<BitVector>(allPossibleSets);
                    _columnPossibleSets[i] = new HashSet<BitVector>(allPossibleSets);
                }
                if (includeDiagonals)
                {
                    _forwardDiagonalPossibleSets = new HashSet<BitVector>(allPossibleSets);
                    _backwardDiagonalPossibleSets = new HashSet<BitVector>(allPossibleSets);
                }
            }

            private BoxPossibleValues(BoxPossibleValues existing, IReadOnlyPuzzle puzzle)
            {
                _allPossibleValues = existing._allPossibleValues;
                _box = existing._box;
                _puzzle = puzzle;
                _allPossibleSets = existing._allPossibleSets;
                _includeDiagonals = existing._includeDiagonals;

                _setOnRows = existing._setOnRows.AsSpan().ToArray();
                _setOnColumns = existing._setOnColumns.AsSpan().ToArray();
                _rowPossibleSets = existing._rowPossibleSets.Select(set => new HashSet<BitVector>(set)).ToArray();
                _columnPossibleSets = existing._columnPossibleSets.Select(set => new HashSet<BitVector>(set)).ToArray();
                if (_includeDiagonals)
                {
                    _forwardDiagonalPossibleSets = new HashSet<BitVector>(existing._forwardDiagonalPossibleSets!);
                    _backwardDiagonalPossibleSets = new HashSet<BitVector>(existing._backwardDiagonalPossibleSets!);
                }
            }

            public BoxPossibleValues CopyWithNewReference(IReadOnlyPuzzle puzzle)
            {
                if (_puzzle.Size != puzzle.Size)
                {
                    throw new ArgumentException("Puzzle sizes should match.");
                }
                return new BoxPossibleValues(this, puzzle);
            }

            public BitVector GetPossibleValues(in Coordinate coord)
            {
                if (!_box.Contains(coord))
                {
                    return _allPossibleValues;
                }
                var relativeCoord = _GetRelative(in coord);
                var setValues = BitVector.FindUnion(
                    _setOnColumns[relativeCoord.Column], _setOnRows[relativeCoord.Row]);
                var results = new BitVector(
                    _rowPossibleSets[relativeCoord.Row].Aggregate(BitVector.FindUnion).Data
                    ^ _setOnRows[relativeCoord.Row].Data);
                results = BitVector.FindIntersect(
                    results,
                    new BitVector(
                        _columnPossibleSets[relativeCoord.Column].Aggregate(BitVector.FindUnion).Data
                        ^ _setOnColumns[relativeCoord.Column].Data));
                if (_includeDiagonals)
                {
                    if (_IsOnBackwardDiagonal(relativeCoord))
                    {
                        results = BitVector.FindIntersect(
                            results,
                            new BitVector(
                                _backwardDiagonalPossibleSets!.Aggregate(BitVector.FindUnion).Data
                                ^ _setOnBackwardDiagonal.Data));
                    }
                    if (_IsOnForwardDiagonal(relativeCoord))
                    {
                        results = BitVector.FindIntersect(
                            results,
                            new BitVector(
                                _forwardDiagonalPossibleSets!.Aggregate(BitVector.FindUnion).Data
                                ^ _setOnForwardDiagonal.Data));
                    }
                }
                return results;
            }

            public void Update(in Coordinate coord, int value, CoordinateTracker affectedCoordinates)
            {
                if (!_box.Contains(in coord))
                {
                    return;
                }
                Update(in coord, value);
                _TrackAffectedCoordinates(in coord, affectedCoordinates);
            }

            public void Update(in Coordinate coord, int value)
            {
                if (!_box.Contains(in coord))
                {
                    return;
                }
                var relativeCoord = _GetRelative(in coord);
                _setOnRows[relativeCoord.Row].SetBit(value);
                _setOnColumns[relativeCoord.Column].SetBit(value);
                _rowPossibleSets[relativeCoord.Row].RemoveWhere(set => !set.IsBitSet(value));
                _columnPossibleSets[relativeCoord.Column].RemoveWhere(set => !set.IsBitSet(value));
                if (!_includeDiagonals)
                {
                    return;
                }
                if (_IsOnBackwardDiagonal(relativeCoord))
                {
                    _setOnBackwardDiagonal.SetBit(value);
                    _backwardDiagonalPossibleSets!.RemoveWhere(set => !set.IsBitSet(value));
                }
                if (_IsOnForwardDiagonal(relativeCoord))
                {
                    _setOnForwardDiagonal.SetBit(value);
                    _forwardDiagonalPossibleSets!.RemoveWhere(set => !set.IsBitSet(value));
                }
            }

            public void Revert(in Coordinate coord, int value)
            {
                if (!_box.Contains(in coord))
                {
                    return;
                }
                var relativeCoord = _GetRelative(in coord);
                _setOnRows[relativeCoord.Row].UnsetBit(value);
                _setOnColumns[relativeCoord.Column].UnsetBit(value);
                var setOnX = _setOnRows[relativeCoord.Row];
                _rowPossibleSets[relativeCoord.Row].UnionWith(_allPossibleSets.Where(
                    set => BitVector.FindIntersect(set, setOnX) == setOnX));
                setOnX = _setOnColumns[relativeCoord.Column];
                _columnPossibleSets[relativeCoord.Column].UnionWith(_allPossibleSets.Where(
                    set => BitVector.FindIntersect(set, setOnX) == setOnX));
                if (!_includeDiagonals)
                {
                    return;
                }
                if (_IsOnForwardDiagonal(relativeCoord))
                {
                    _setOnForwardDiagonal.UnsetBit(value);
                    _forwardDiagonalPossibleSets!.UnionWith(_allPossibleSets.Where(
                        set => BitVector.FindIntersect(set, _setOnForwardDiagonal) == _setOnForwardDiagonal));
                }
                if (_IsOnBackwardDiagonal(relativeCoord))
                {
                    _setOnBackwardDiagonal.UnsetBit(value);
                    _backwardDiagonalPossibleSets!.UnionWith(_allPossibleSets.Where(
                        set => BitVector.FindIntersect(set, _setOnBackwardDiagonal) == _setOnBackwardDiagonal));
                }
            }

            public void Revert(in Coordinate coord, int value, CoordinateTracker affectedCoordinates)
            {
                if (!_box.Contains(in coord))
                {
                    return;
                }
                Revert(in coord, value);
                _TrackAffectedCoordinates(in coord, affectedCoordinates);
            }

            private void _TrackAffectedCoordinates(in Coordinate coord, CoordinateTracker affectedCoordinates)
            {
                Coordinate relativeCoord = _GetRelative(in coord);
                for (int i = 0; i < _box.Size; ++i)
                {
                    // Update row
                    Coordinate absolute = _GetAbsolute(new Coordinate(relativeCoord.Row, i));
                    if (absolute != coord && !_puzzle[in absolute].HasValue)
                    {
                        affectedCoordinates.AddOrTrackIfUntracked(in absolute);
                    }
                    // Update column
                    absolute = _GetAbsolute(new Coordinate(i, relativeCoord.Column));
                    if (absolute != coord && !_puzzle[in absolute].HasValue)
                    {
                        affectedCoordinates.AddOrTrackIfUntracked(in absolute);
                    }
                }
                if (!_includeDiagonals)
                {
                    return;
                }
                if (_IsOnBackwardDiagonal(relativeCoord))
                {
                    var endCoord = new Coordinate(_box.TopLeft.Row + _box.Size, _box.TopLeft.Column + _box.Size);
                    for (
                        Coordinate toCheck = _box.TopLeft;
                        toCheck != endCoord;
                        toCheck = new Coordinate(toCheck.Row + 1, toCheck.Column + 1)) {
                        if (toCheck != coord && !_puzzle[in toCheck].HasValue)
                        {
                            affectedCoordinates.AddOrTrackIfUntracked(in toCheck);
                        }
                    } 
                }
                if (_IsOnForwardDiagonal(relativeCoord))
                {
                    var endCoord = new Coordinate(_box.TopLeft.Row + _box.Size, _box.TopLeft.Column - 1);
                    for (
                        Coordinate toCheck = new Coordinate(_box.TopLeft.Row, _box.TopLeft.Column + _box.Size - 1);
                        toCheck != endCoord;
                        toCheck = new Coordinate(toCheck.Row + 1, toCheck.Column - 1)) {
                        if (toCheck != coord && !_puzzle[in toCheck].HasValue)
                        {
                            affectedCoordinates.AddOrTrackIfUntracked(in toCheck);
                        }
                    }
                }
            }

            private Coordinate _GetRelative(in Coordinate coord)
            {
                return  new Coordinate(
                    coord.Row - _box.TopLeft.Row, coord.Column - _box.TopLeft.Column);
            }

            private Coordinate _GetAbsolute(in Coordinate coord)
            {
                return new Coordinate(_box.TopLeft.Row + coord.Row, _box.TopLeft.Column + coord.Column);
            }

            private bool _IsOnBackwardDiagonal(in Coordinate relativeCoord)
            {
                return relativeCoord.Row == relativeCoord.Column;
            }

            private bool _IsOnForwardDiagonal(in Coordinate relativeCoord)
            {
                return relativeCoord.Column == _box.Size - 1 - relativeCoord.Row;
            }
        }

        private readonly Square[] _boxes; 
        private readonly int _size;
        private readonly int _boxSize;
        private readonly bool _includeDiagonals;
        private BitVector _allPossibleValues;
        private IReadOnlySet<BitVector>? _possibleSets;
        private BoxPossibleValues[]? _boxPossibleValues;

        public MagicSquaresRule(int size, IEnumerable<Square> squares, bool includeDiagonals)
        {
            _size = size;
            _boxes = squares.ToArray();
            _boxSize = Boxes.CalculateBoxSize(size);
            _includeDiagonals = includeDiagonals;
            if (!_boxes.All(b => b.Size == _boxSize))
            {
                throw new ArgumentException("Boxes must all have the same size.");
            }
        }

        private MagicSquaresRule(MagicSquaresRule existing)
        {
            _size = existing._size;
            _boxes = existing._boxes;
            _boxSize = existing._boxSize;
            _includeDiagonals = existing._includeDiagonals;
        }

        public IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
        {
            var copy = new MagicSquaresRule(this);
            if (puzzle is null)
            {
                return copy;
            }
            if (puzzle.Size != copy._size)
            {
                throw new ArgumentException(
                    $"Puzzle size {puzzle.Size} did not match expected size {copy._size}.");
            }
            copy._allPossibleValues = _allPossibleValues;
            copy._boxPossibleValues = _boxPossibleValues!.Select(bp => bp.CopyWithNewReference(puzzle)).ToArray();
            return copy;
        }

        public bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
        {
            if (_size != puzzle.Size)
            {
                return false;
            }
            if (_allPossibleValues != allPossibleValues)
            {
                _allPossibleValues = allPossibleValues;
                _possibleSets = MagicSquares.ComputeSets(puzzle.AllPossibleValuesSpan);
            }
            _boxPossibleValues = new BoxPossibleValues[_boxes.Length];
            for (int i = 0; i < _boxPossibleValues.Length; ++i)
            {
                _boxPossibleValues[i] = new BoxPossibleValues(
                    _boxes[i], allPossibleValues, _possibleSets!, puzzle, _includeDiagonals);
            }
            for (int row = 0; row < _size; ++row)
            {
                for (int col = 0; col < _size; ++col)
                {
                    var value = puzzle[row, col];
                    if (!value.HasValue)
                    {
                        continue;
                    }
                    foreach (var boxPossibles in _boxPossibleValues)
                    {
                        boxPossibles.Update(new Coordinate(row, col), value.Value);
                    }
                }
            }
            foreach (var unsetCoord in puzzle.GetUnsetCoords())
            {
                if (GetPossibleValues(unsetCoord).IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        public BitVector GetPossibleValues(in Coordinate c)
        {
            if (_boxPossibleValues is null)
            {
                throw new InvalidOperationException(
                    $"Must initialize this rule before calling {nameof(GetPossibleValues)}.");
            }
            var coordCopy = c;
            return _boxPossibleValues.Aggregate(
                _allPossibleValues,
                (intersect, bp) => BitVector.FindIntersect(bp.GetPossibleValues(coordCopy), intersect));
        }

        public void Revert(in Coordinate c, int val)
        {
            if (_boxPossibleValues is null)
            {
                throw new InvalidOperationException(
                    $"Must initialize this rule before calling {nameof(Revert)}.");
            }
            foreach (var boxPossible in _boxPossibleValues)
            {
                boxPossible.Revert(in c, val);
            }
        }

        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            if (_boxPossibleValues is null)
            {
                throw new InvalidOperationException(
                    $"Must initialize this rule before calling {nameof(Revert)}.");
            }
            foreach (var boxPossible in _boxPossibleValues)
            {
                boxPossible.Revert(in c, val, coordTracker);
            }
        }

        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            if (_boxPossibleValues is null)
            {
                throw new InvalidOperationException(
                    $"Must initialize this rule before calling {nameof(Update)}.");
            }
            foreach (var boxPossible in _boxPossibleValues)
            {
                boxPossible.Update(in c, val, coordTracker);
            }
        }
    }
}
