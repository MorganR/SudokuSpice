using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    internal sealed class Guesser<TPuzzle> where TPuzzle : class, IPuzzle<TPuzzle>
    {
        private readonly TPuzzle _puzzle;
        private readonly ExactCoverGraph _graph;
        private readonly Stack<Guess> _setSquares;

        internal int MaxGuessCount { get; }
        internal bool IsSolved => _puzzle.NumEmptySquares == 0;

        internal Guesser(TPuzzle puzzle, ExactCoverGraph graph)
        {
            _puzzle = puzzle;
            _graph = graph;
            _setSquares = new Stack<Guess>(puzzle.NumEmptySquares);
            MaxGuessCount = _graph.AllPossibleValues.Length;
        }

        private Guesser(Guesser<TPuzzle> other)
        {
            _puzzle = other._puzzle.DeepCopy();
            // Copy graph, focusing only on 'Unknown' possible square values and (therefore) unsatisfied constraints.
            _graph = other._graph.CopyUnknowns();
            _setSquares = new Stack<Guess>(_puzzle.NumEmptySquares);
            MaxGuessCount = other.MaxGuessCount;
        }

        internal Guesser<TPuzzle> CopyForContinuation() => new Guesser<TPuzzle>(this);

        internal int PopulateBestGuesses(Span<Guess> toPopulate)
        {
            int maxPossibleValues = MaxGuessCount + 1;
            Objective? bestObjective = null;
            foreach (Objective? objective in _graph.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities())
            {
                if (!objective.AllUnknownPossibilitiesAreConcrete)
                {
                    continue;
                }
                if (objective.AllUnknownPossibilitiesAreRequired)
                {
                    Possibility possibility = (Possibility)objective
                        .GetUnknownDirectPossibilities().First();
                    toPopulate[0] = new Guess(possibility.Coordinate, possibility.Index);
                    return 1;
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
                $"{nameof(bestObjective)} was still null at the end of {nameof(PopulateBestGuesses)}.");
            return _PopulateGuessesFromObjective(bestObjective, toPopulate);
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

        private static int _PopulateGuessesFromObjective(Objective objective, Span<Guess> toPopulate)
        {
            Debug.Assert(objective.State != NodeState.SELECTED, "Objective must not be satisfied.");
            Debug.Assert(objective.AllUnknownPossibilitiesAreConcrete, "All possibilities must be concrete.");
            int guessCount = 0;
            // TODO: Try removing all these casts; they're ugly and could be slowing things down
            // See https://www.danielcrabtree.com/blog/164/c-sharp-7-micro-benchmarking-the-three-ways-to-cast-safely
            foreach (IPossibility p in objective.GetUnknownDirectPossibilities())
            {
                var possibility = (Possibility)p;
                toPopulate[guessCount++] = new Guess(possibility.Coordinate, possibility.Index);
            }
            return guessCount;
        }
    }
}