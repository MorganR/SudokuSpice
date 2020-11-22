using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a box.
    /// Sets the possible values for those squares to just their unique value.
    ///
    /// For example, if a box had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </summary>
    public class UniqueInBoxHeuristic : ISudokuHeuristic
    {
        private readonly PossibleValues _possibleValues;
        private readonly IMissingBoxValuesTracker _boxTracker;
        private readonly BitVector[] _possiblesToCheckInBox;
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;
        private IReadOnlyBoxPuzzle? _puzzle;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use when solving.
        /// </param>
        /// <param name="boxValuesTracker">
        /// Something that tracks the possible values for each box. Rules often do this already,
        /// for example.
        /// </param>
        public UniqueInBoxHeuristic(PossibleValues possibleValues, IMissingBoxValuesTracker boxValuesTracker)
        {
            int size = possibleValues.Size;
            _possibleValues = possibleValues;
            _boxTracker = boxValuesTracker;
            _possiblesToCheckInBox = new BitVector[size];
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
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match current heuristic size ({_possibleValues.Size})");
            Debug.Assert(puzzle.Size == possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match possible values size ({possibleValues.Size})");
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException(
                    $"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(BoxUniquenessRule)}.");
            }
            try {
                return new UniqueInBoxHeuristic(
                    this, boxPuzzle, possibleValues,
                    (IMissingBoxValuesTracker)rules.First(r=> r is IMissingBoxValuesTracker));
            } catch (InvalidOperationException)
            {
                throw new ArgumentException($"{nameof(rules)} must include an {nameof(IMissingBoxValuesTracker)}.");
            }
        }

        /// <summary>
        /// Tries to initialize this heuristic for solving the given puzzle.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the heuristic's state is unchanged
        /// on failure.
        /// </remarks>
        /// <param name="puzzle">
        /// The puzzle to solve. This must implement <see cref="IReadOnlyBoxPuzzle"/>.
        /// </param>
        /// <returns>
        /// False if this heuristic cannot be initialized for the given puzzle, else true.
        /// </returns>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_possibleValues.Size}).");
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                return false;
            }
            _puzzle = boxPuzzle;
            _previousPossiblesStack.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(UpdateAll)} with a null puzzle.");
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
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_UpdateBox)} with a null puzzle.");
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
