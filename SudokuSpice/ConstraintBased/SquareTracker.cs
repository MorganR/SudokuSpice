using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    internal class SquareTracker<TPuzzle> where TPuzzle : class, IPuzzle<TPuzzle>
    {
        private readonly TPuzzle _puzzle;
        private readonly ExactCoverMatrix _matrix;
        private readonly Stack<Guess> _setSquares;

        internal bool IsSolved => _puzzle.NumEmptySquares == 0;

        internal SquareTracker(TPuzzle puzzle, ExactCoverMatrix matrix)
        {
            _puzzle = puzzle;
            _matrix = matrix;
            _setSquares = new Stack<Guess>(puzzle.NumEmptySquares);
        }

        private SquareTracker(SquareTracker<TPuzzle> other)
        {
            // Puzzle is guaranteed to be of type TPuzzle.
            _puzzle = other._puzzle.DeepCopy();
            // Copy matrix, focusing only on 'Unknown' possible square values and (therefore) unsatisfied constraints.
            _matrix = other._matrix.CopyUnknowns();
            _setSquares = new Stack<Guess>(_puzzle.NumEmptySquares);
        }

        internal SquareTracker<TPuzzle> CopyForContinuation() => new SquareTracker<TPuzzle>(this);

        internal IEnumerable<Guess> GetBestGuesses()
        {
            int maxPossibleValues = _puzzle.Size + 1;
            Objective? bestObjective = null;
            foreach (Objective? objective in _matrix.GetUnsatisfiedRequiredObjectives())
            {
                if (!objective.AllUnknownPossibilitiesAreConcrete)
                {
                    continue;
                }
                if (objective.AllPossibilitiesAreRequired)
                {
                    Possibility possibility = (Possibility)((IObjective)objective)
                        .GetUnknownDirectPossibilities().First();
                    return new Guess(possibility.Coordinate, possibility.Index).Yield();
                }
                var numPossibilities = objective.CountUnknown;
                if (numPossibilities < maxPossibleValues)
                {
                    maxPossibleValues = numPossibilities;
                    bestObjective = objective;
                }
            }
            Debug.Assert(
                bestObjective is not null,
                $"{nameof(bestObjective)} was still null at the end of {nameof(GetBestGuesses)}.");
            return _RetrieveGuessesFromObjective(bestObjective);
        }

        internal bool TrySet(in Guess guess)
        {
            Possibility?[]? square = _matrix.GetAllPossibilitiesAt(guess.Coordinate);
            Debug.Assert(
                square is not null,
                $"Tried to set {guess.Coordinate} to value at {guess.PossibilityIndex}, but square was null.");
            var possibility = square[guess.PossibilityIndex];
            Debug.Assert(
                possibility is not null,
                $"Tried to set square at {guess.Coordinate} to possibility at {guess.PossibilityIndex}, but possibility was null.");
            if (!possibility.TrySelect())
            {
                return false;
            }
            _puzzle[guess.Coordinate] = _matrix.AllPossibleValues[guess.PossibilityIndex];
            _setSquares.Push(guess);
            // if (IsSolved)
            // {
            //     for (int row = 0; row < _puzzle.Size; ++row)
            //     {
            //         var squaresOnRow = _matrix.GetSquaresOnRow(row);
            //         foreach (var aSquare in squaresOnRow)
            //         {
            //             if (aSquare is not null)
            //             {
            //                 var psq = aSquare.GetPossibleValue(
            //                     _matrix.ValuesToIndices[_puzzle[aSquare.Coordinate].Value]);
            //                 Debug.Assert(psq.State == PossibilityState.SELECTED);
            //                 var requirement = psq.FirstLink.Objective;
            //                 Debug.Assert(requirement.AreAllRequiredPossibilitiesSelected);
            //                 var andGroups = requirement.FirstGroupLink.GetLinksOnPossibility();
            //                 Debug.Assert(andGroups.All(group => group.Objective.AreAllRequiredPossibilitiesSelected));
            //                 Debug.Assert(andGroups.All(group => group.Objective.ParentGroupLink.GetLinksOnPossibility().All(orGroup => orGroup.Objective.AreAllRequiredPossibilitiesSelected)));
            //             }
            //         }
            //     }
            // }
            return true;
        }

        internal void UnsetLast()
        {
            Debug.Assert(
                _setSquares.Count > 0,
                "Tried to call UnsetLast when no squares had been set.");
            Guess setSquare = _setSquares.Pop();
            _puzzle[setSquare.Coordinate] = null;
            Possibility?[]? square = _matrix.GetAllPossibilitiesAt(setSquare.Coordinate);
            Debug.Assert(
                square is not null,
                $"Tried to unset the last update to a null square at {setSquare.Coordinate}.");
            var possibility = square[setSquare.PossibilityIndex];
            Debug.Assert(
                possibility is not null,
                $"Tried to unselect a possibility at index {setSquare.PossibilityIndex} for " +
                $"square {setSquare.Coordinate}, but the possiblity was null.");
            possibility.Deselect();
        }

        private IEnumerable<Guess> _RetrieveGuessesFromObjective(Objective objective)
        {
            Debug.Assert(!objective.IsSatisfied, "Objective must not be satisfied.");
            Debug.Assert(objective.AllUnknownPossibilitiesAreConcrete, "All possibilities must be concrete.");
            return ((IObjective)objective).GetUnknownDirectPossibilities().Select(
                p => new Guess(((Possibility)p).Coordinate, ((Possibility)p).Index));
        }
    }
}