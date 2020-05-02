using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    class ConstraintBasedTracker
    {
        private IPuzzle _puzzle;
        private ExactCoverMatrix _matrix;
        private Stack<Coordinate> _setCoords;

        public ConstraintBasedTracker(IPuzzle puzzle, ExactCoverMatrix matrix)
        {
            _puzzle = puzzle;
            _matrix = matrix;
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var coordinate = new Coordinate(row, col);
                    var value = puzzle[row, col];
                    if (value.HasValue)
                    {
                        if (!_matrix.GetSquare(in coordinate).TrySetValue(value.Value))
                        {
                            throw new ArgumentException($"Given puzzle violates the constraints at {(coordinate)}.");
                        }
                    }
                }
            }
        }

        public (Coordinate coord, IReadOnlyList<int> values) GetBestGuess()
        {
            int maxPossibleValues = _puzzle.Size + 1;
            Square? bestSquare = null;
            foreach (var coord in _puzzle.GetUnsetCoords())
            {
                var square = _matrix.GetSquare(in coord);
                if (square.NumPossibleValues < maxPossibleValues)
                {
                    maxPossibleValues = square.NumPossibleValues;
                    if (maxPossibleValues == 1)
                    {
                        return (coord, new List<int> { square.GetStillPossibleValues()[0].Value });
                    }
                    bestSquare = square;
                }
            }
            foreach (var constraint in _matrix.ConstraintHeaders)
            {
                if (constraint.IsSatisfied)
                {
                    continue;
                }
                if (constraint.Count == 1)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var possibleSquare = constraint.FirstLink.PossibleSquare;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    return (possibleSquare.Square.Coordinate, new List<int> { possibleSquare.Value });
                }
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference. Not actually possible to be null unless _unsetCoords is empty.
            return (bestSquare.Coordinate, _OrderPossibleValuesByProbability(bestSquare.Coordinate));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private IReadOnlyList<int> _OrderPossibleValuesByProbability(in Coordinate c)
        {
            var possibleSquares = _matrix.GetSquare(in c).GetStillPossibleValues();
            return possibleSquares.OrderBy(ps => ps.GetMinConstraintCount()).Select(ps => ps.Value).ToList();
        }

        public bool TrySet(in Coordinate c, int value)
        {
            if (!_matrix.GetSquare(in c).TrySetValue(value))
            {
                return false;
            }
            _puzzle[in c] = value;
            _setCoords.Push(c);
            return true;
        }

        public void UnsetLast()
        {
            Debug.Assert(_setCoords.Count > 0, "Tried to call UnsetLast when no squares had been set.");
            var c = _setCoords.Pop();
            _puzzle[in c] = null;
            _matrix.GetSquare(in c).UnsetValue();
        }
    }
}
