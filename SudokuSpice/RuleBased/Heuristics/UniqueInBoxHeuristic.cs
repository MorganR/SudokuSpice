using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a box.
    /// Sets the possible values for those squares to just their unique value.
    /// </summary>
    /// <remarks>
    /// For example, if a box had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </remarks>
    public class UniqueInBoxHeuristic : ISudokuHeuristic
    {
        private readonly IReadOnlyBoxPuzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IMissingBoxValuesTracker _boxTracker;
        private readonly BitVector[] _possiblesToCheckInBox;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;

        public UniqueInBoxHeuristic(IReadOnlyBoxPuzzle puzzle, PossibleValues possibleValues, IMissingBoxValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _boxTracker = rule;
            _possiblesToCheckInBox = new BitVector[puzzle.Size];
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>();
        }

        private UniqueInBoxHeuristic(
            UniqueInBoxHeuristic existing,
            IReadOnlyBoxPuzzle puzzle,
            PossibleValues possibleValues,
            IMissingBoxValuesTracker rule)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _boxTracker = rule;
            _possiblesToCheckInBox = (BitVector[])existing._possiblesToCheckInBox.Clone();
            _previousPossiblesStack = new Stack<IReadOnlyDictionary<Coordinate, BitVector>>(
                existing._previousPossiblesStack);
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingBoxValuesTracker"/>.
        /// </summary>
        public ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRule> rules)
        {
            if (puzzle is IReadOnlyBoxPuzzle boxPuzzle)
            {
                ISudokuRule? missingValuesTracker = rules.FirstOrDefault(r => r is IMissingBoxValuesTracker);
                if (missingValuesTracker is null)
                {
                    throw new ArgumentException(
                        $"{nameof(rules)} must contain an {nameof(IMissingBoxValuesTracker)} to copy a {nameof(BoxUniquenessRule)}.");
                }
                return new UniqueInBoxHeuristic(
                    this, boxPuzzle, possibleValues, (IMissingBoxValuesTracker)missingValuesTracker);
            }
            throw new ArgumentException(
                $"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(BoxUniquenessRule)}.");
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int box = 0; box < _puzzle.Size; box++)
            {
                _possiblesToCheckInBox[box] = _boxTracker.GetMissingValuesForBox(box);
                _UpdateBox(box, previousPossibles);
            }
            _previousPossiblesStack.Push(previousPossibles);
            return previousPossibles.Count > 0;
        }

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            IReadOnlyDictionary<Coordinate, BitVector>? overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach (KeyValuePair<Coordinate, BitVector> coordPossiblesPair in overwrittenPossibles)
            {
                _possibleValues[coordPossiblesPair.Key] = coordPossiblesPair.Value;
            }
        }

        private void _UpdateBox(int box, IDictionary<Coordinate, BitVector> previousPossibles)
        {
            Span<int> possibleValues = stackalloc int[_puzzle.Size];
            int numPossible = _possiblesToCheckInBox[box].PopulateSetBits(possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possible = possibleValues[i];
                Coordinate? uniqueCoord = null;
                foreach (Coordinate c in _puzzle.YieldUnsetCoordsForBox(box))
                {
                    if (_possibleValues[in c].IsBitSet(possible))
                    {
                        if (uniqueCoord.HasValue)
                        {
                            uniqueCoord = null;
                            break;
                        }
                        uniqueCoord = c;
                    }
                }
                if (!uniqueCoord.HasValue)
                {
                    continue;
                }
                var possibles = new BitVector();
                possibles.SetBit(possible);
                previousPossibles[uniqueCoord.Value] = _possibleValues[uniqueCoord.Value];
                _possibleValues[uniqueCoord.Value] = possibles;
            }
        }
    }
}
