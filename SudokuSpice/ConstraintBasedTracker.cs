using SudokuSpice.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    class ConstraintBasedTracker
    {
        private readonly IPuzzle _puzzle;
        private readonly ExactCoverMatrix _matrix;
        private readonly Stack<Coordinate> _setCoords;

        public ConstraintBasedTracker(IPuzzle puzzle, ExactCoverMatrix matrix)
        {
            _puzzle = puzzle;
            _matrix = matrix;
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            Dictionary<int, int> valuesToIndices = new Dictionary<int, int>(matrix.AllPossibleValues.Length);
            for (int index = 0; index < matrix.AllPossibleValues.Length; index++)
            {
                valuesToIndices[matrix.AllPossibleValues[index]] = index;
            }
        }

        public (Coordinate coord, int[] possibleValueIndices) GetBestGuess()
        {
            int maxPossibleValues = _puzzle.Size + 1;
            Square? bestSquare = null;
            foreach (var coord in _puzzle.GetUnsetCoords())
            {
                var square = _matrix.GetSquare(in coord);
                Debug.Assert(square != null, $"Square was null at unset coord {coord}.");
                if (square.NumPossibleValues < maxPossibleValues)
                {
                    maxPossibleValues = square.NumPossibleValues;
                    if (maxPossibleValues == 1)
                    {
                        return (coord, new int[] { square.GetStillPossibleValues()[0].ValueIndex });
                    }
                    bestSquare = square;
                }
            }
            foreach (var constraint in _matrix.GetUnsatisfiedConstraintHeaders())
            {
                if (constraint.Count == 1)
                {
                    Debug.Assert(constraint.FirstLink != null, "Unsatisfied constraint had a null first link.");
                    var possibleSquare = constraint.FirstLink.PossibleSquare;
                    return (possibleSquare.Square.Coordinate, new int[] { possibleSquare.ValueIndex });
                }
            }
            Debug.Assert(bestSquare != null, $"{nameof(bestSquare)} was still null at the end of {nameof(GetBestGuess)}.");
            return (bestSquare.Coordinate, _OrderPossibleValuesByProbability(bestSquare.Coordinate));
        }

        private int[] _OrderPossibleValuesByProbability(in Coordinate c)
        {
            var possibleSquares = _matrix.GetSquare(in c).GetStillPossibleValues();
            return possibleSquares.OrderBy(ps => ps.GetMinConstraintCount()).Select(ps => ps.ValueIndex).ToArray();
        }

        public bool TrySet(in Coordinate c, int valueIndex)
        {
            if (!_matrix.GetSquare(in c).TrySet(valueIndex))
            {
                return false;
            }
            _puzzle[in c] = _matrix.AllPossibleValues[valueIndex];
            _setCoords.Push(c);
            return true;
        }

        public void UnsetLast()
        {
            Debug.Assert(_setCoords.Count > 0, "Tried to call UnsetLast when no squares had been set.");
            var c = _setCoords.Pop();
            _puzzle[in c] = null;
            var square = _matrix.GetSquare(in c);
            Debug.Assert(square != null, $"Tried to unset the last update to a null square at {c}.");
            square.Unset();
        }
    }
}
