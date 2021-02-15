using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    internal class Guesser<TPuzzle> where TPuzzle : class, IPuzzle<TPuzzle>
    {
        private readonly TPuzzle _puzzle;
        private readonly ExactCoverGraph _graph;
        private readonly Stack<Guess> _setSquares;

        internal bool IsSolved => _puzzle.NumEmptySquares == 0;

        internal Guesser(TPuzzle puzzle, ExactCoverGraph graph)
        {
            _puzzle = puzzle;
            _graph = graph;
            _setSquares = new Stack<Guess>(puzzle.NumEmptySquares);
        }

        private Guesser(Guesser<TPuzzle> other)
        {
            _puzzle = other._puzzle.DeepCopy();
            // Copy graph, focusing only on 'Unknown' possible square values and (therefore) unsatisfied constraints.
            _graph = other._graph.CopyUnknowns();
            _setSquares = new Stack<Guess>(_puzzle.NumEmptySquares);
        }

        internal Guesser<TPuzzle> CopyForContinuation() => new Guesser<TPuzzle>(this);

        internal IEnumerable<Guess> GetBestGuesses()
        {
            int maxPossibleValues = _puzzle.Size + 1;
            Objective? bestObjective = null;
            foreach (Objective? objective in _graph.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities())
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
            Possibility?[]? square = _graph.GetAllPossibilitiesAt(guess.Coordinate);
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
            _puzzle[guess.Coordinate] = _graph.AllPossibleValues[guess.PossibilityIndex];
            _setSquares.Push(guess);
            return true;
        }

        internal void UnsetLast()
        {
            Debug.Assert(
                _setSquares.Count > 0,
                "Tried to call UnsetLast when no squares had been set.");
            Guess setSquare = _setSquares.Pop();
            _puzzle[setSquare.Coordinate] = null;
            Possibility?[]? square = _graph.GetAllPossibilitiesAt(setSquare.Coordinate);
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