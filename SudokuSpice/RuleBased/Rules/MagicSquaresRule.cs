using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Enforces a rule that certain regions in a puzzle must be
    /// <a href="https://en.wikipedia.org/wiki/Magic_square">magic squares</a>, i.e. the sums of the
    /// values in each of their rows, columns, and (optionally) their diagonals add up to the same
    /// value.
    ///
    /// Note that this does <em>not</em> enforce uniqueness of values within the magic square as a
    /// whole. It <em>does</em>, however, prevent value duplication within each row, column, and/or
    /// diagonal. This can be combined with other rules if you need proper row, column, box, and/or
    /// diagonal uniqueness.
    /// </summary>
    public class MagicSquaresRule : IRule
    {
        private class BoxPossibleValues
        {
            private readonly BitVector _allPossibleValues;
            private readonly Box _box;
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

            public BoxPossibleValues(Box box, BitVector allPossibleValues, IReadOnlySet<BitVector> allPossibleSets, IReadOnlyPuzzle puzzle, bool includeDiagonals)
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
                var results = BitVector.FindDifference(
                    _rowPossibleSets[relativeCoord.Row].Aggregate(BitVector.FindUnion),
                    _setOnRows[relativeCoord.Row]);
                results = BitVector.FindIntersect(
                    results,
                    BitVector.FindDifference(
                        _columnPossibleSets[relativeCoord.Column].Aggregate(BitVector.FindUnion),
                        _setOnColumns[relativeCoord.Column]));
                if (_includeDiagonals)
                {
                    if (_IsOnBackwardDiagonal(relativeCoord))
                    {
                        results = BitVector.FindIntersect(
                            results,
                            BitVector.FindDifference(
                                _backwardDiagonalPossibleSets!.Aggregate(BitVector.FindUnion),
                                _setOnBackwardDiagonal));
                    }
                    if (_IsOnForwardDiagonal(relativeCoord))
                    {
                        results = BitVector.FindIntersect(
                            results,
                            BitVector.FindDifference(
                                _forwardDiagonalPossibleSets!.Aggregate(BitVector.FindUnion),
                                _setOnForwardDiagonal));
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
                    set => setOnX.IsSubsetOf(set)));
                setOnX = _setOnColumns[relativeCoord.Column];
                _columnPossibleSets[relativeCoord.Column].UnionWith(_allPossibleSets.Where(
                    set => setOnX.IsSubsetOf(set)));
                if (!_includeDiagonals)
                {
                    return;
                }
                if (_IsOnForwardDiagonal(relativeCoord))
                {
                    _setOnForwardDiagonal.UnsetBit(value);
                    _forwardDiagonalPossibleSets!.UnionWith(_allPossibleSets.Where(
                        set => _setOnForwardDiagonal.IsSubsetOf(set)));
                }
                if (_IsOnBackwardDiagonal(relativeCoord))
                {
                    _setOnBackwardDiagonal.UnsetBit(value);
                    _backwardDiagonalPossibleSets!.UnionWith(_allPossibleSets.Where(
                        set => _setOnBackwardDiagonal.IsSubsetOf(set)));
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

        private readonly Box[] _boxes; 
        private readonly int _size;
        private readonly int _boxSize;
        private readonly bool _includeDiagonals;
        private BitVector _allPossibleValues;
        private IReadOnlySet<BitVector>? _possibleSets;
        private BoxPossibleValues[]? _boxPossibleValues;

        /// <summary>
        /// Constructs a rule that will enforce that the given <paramref name="squares"/> are
        /// magic squares based on the rows, columns, and, optionally, the diagonals.
        ///
        /// Unlike other rules, this rule can only be constructed for a specific puzzle size.
        /// Attempting to initialize this rule with an incompatible puzzle will fail.
        /// </summary>
        /// <param name="size">
        /// The size of the puzzle this rule applies to.
        /// </param>
        /// <param name="squares">
        /// The locations of the magic squares. The squares' size must be the square root of the
        /// puzzle <paramref name="size"/>.
        /// </param>
        /// <param name="includeDiagonals">
        /// If true, values along the diagonals of the square must also sum to the magic number.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the any of the given <paramref name="squares"/>' sizes are not compatible with the
        /// given puzzle's <paramref name="size"/>.
        /// </exception>

        public MagicSquaresRule(int size, IEnumerable<Box> squares, bool includeDiagonals)
        {
            _size = size;
            _boxes = squares.ToArray();
            _boxSize = Boxes.IntSquareRoot(size);
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                if (GetPossibleValues(unsetCoord).IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
