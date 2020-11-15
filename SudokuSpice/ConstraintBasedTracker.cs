using SudokuSpice.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    internal class ConstraintBasedTracker<TPuzzle> where TPuzzle : IPuzzle
    {
        private readonly TPuzzle _puzzle;
        private readonly ExactCoverMatrix<TPuzzle> _matrix;
        private readonly Stack<Coordinate> _setCoords;

        internal bool IsSolved => _puzzle.NumEmptySquares == 0;

        internal ConstraintBasedTracker(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix)
        {
            _puzzle = puzzle;
            _matrix = matrix;
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
        }

        private ConstraintBasedTracker(ConstraintBasedTracker<TPuzzle> other)
        {
            // Puzzle is guaranteed to be of type TPuzzle.
            _puzzle = (TPuzzle)other._puzzle.DeepCopy();
            // Copy matrix, focusing only on 'Unknown' possible square values and (therefore) unsatisfied constraints.
            _matrix = other._matrix.CopyUnknowns();
            _setCoords = new Stack<Coordinate>(_puzzle.NumEmptySquares);
        }

        internal ConstraintBasedTracker<TPuzzle> CopyForContinuation()
        {
            return new ConstraintBasedTracker<TPuzzle>(this);
        }

        internal (Coordinate coord, int[] possibleValueIndices) GetBestGuess()
        {
            int maxPossibleValues = _puzzle.Size + 1;
            Square<TPuzzle>? bestSquare = null;
            foreach (var coord in _puzzle.GetUnsetCoords())
            {
                var square = _matrix.GetSquare(in coord);
                Debug.Assert(square != null, $"Square was null at unset coord {coord}.");
                if (square.NumPossibleValues < maxPossibleValues)
                {
                    maxPossibleValues = square.NumPossibleValues;
                    if (maxPossibleValues == 1)
                    {
                        return (coord,
                            new int[] { square.GetStillPossibleValues()[0].ValueIndex });
                    }
                    bestSquare = square;
                }
            }
            foreach (var constraint in _matrix.GetUnsatisfiedConstraintHeaders())
            {
                if (constraint.Count == 1)
                {
                    Debug.Assert(
                        constraint.FirstLink != null,
                        "Unsatisfied constraint had a null first link.");
                    var possibleSquare = constraint.FirstLink.PossibleSquare;
                    return (possibleSquare.Square.Coordinate,
                        new int[] { possibleSquare.ValueIndex });
                }
            }
            Debug.Assert(
                bestSquare != null,
                $"{nameof(bestSquare)} was still null at the end of {nameof(GetBestGuess)}.");
            return (bestSquare.Coordinate,
                _OrderPossibleValuesByProbability(bestSquare.Coordinate));
        }

        internal bool TrySet(in Coordinate c, int valueIndex)
        {
            var square = _matrix.GetSquare(in c);
            Debug.Assert(
                square != null,
                $"Tried to set {c} to value at {valueIndex}, but square was null.");
            if (!square.TrySet(valueIndex))
            {
                return false;
            }
            _puzzle[in c] = _matrix.AllPossibleValues[valueIndex];
            _setCoords.Push(c);
            return true;
        }

        internal void UnsetLast()
        {
            Debug.Assert(
                _setCoords.Count > 0,
                "Tried to call UnsetLast when no squares had been set.");
            var c = _setCoords.Pop();
            _puzzle[in c] = null;
            var square = _matrix.GetSquare(in c);
            Debug.Assert(
                square != null,
                $"Tried to unset the last update to a null square at {c}.");
            square.Unset();
        }

        private int[] _OrderPossibleValuesByProbability(in Coordinate c)
        {
            var square = _matrix.GetSquare(in c);
            Debug.Assert(
                square != null,
                $"Tried to order possible values at {c}, but square was null.");
            var possibleSquares = square.GetStillPossibleValues();
            return possibleSquares.OrderBy(
                ps => ps.GetMinConstraintCount()).Select(ps => ps.ValueIndex).ToArray();
        }
    }
}
