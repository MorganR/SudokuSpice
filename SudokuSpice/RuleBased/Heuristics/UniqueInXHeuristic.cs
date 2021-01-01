using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Abstract base class for implementing generic "Unique in X-dimension" heuristics. 
    /// </summary>
    // TODO: Consider util-based approach
    public abstract class UniqueInXHeuristic : ISudokuHeuristic
    {
        private BitVector[]? _possiblesToCheckOnDimension;
        private Stack<IReadOnlyDictionary<Coordinate, BitVector>>? _previousPossiblesStack;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;

        public UniqueInXHeuristic() { }

        /// <summary>
        /// Copy constructor for providing a deep copy with new references.
        /// </summary>
        /// <param name="existing">The existing heuristic to copy.</param>
        /// <param name="puzzle">
        /// The puzzle reference being solved. May reference a new object from the one in the
        /// existing heuristic, but the puzzle's data must be the same.
        /// </param>
        protected UniqueInXHeuristic(UniqueInXHeuristic existing, IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            _possiblesToCheckOnDimension = existing._possiblesToCheckOnDimension;
            _previousPossiblesStack = existing._previousPossiblesStack;
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public abstract ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle, IReadOnlyList<ISudokuRule> rules);

        /// <inheritdoc/>
        public virtual bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            int numDimensions = GetNumDimensions(puzzle);
            if (_possiblesToCheckOnDimension is null
                || _possiblesToCheckOnDimension.Length != numDimensions)
            {
                _possiblesToCheckOnDimension = new BitVector[numDimensions];
            }
            _previousPossiblesStack = new();
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            Debug.Assert(_puzzle is not null
                && _possiblesToCheckOnDimension is not null
                && _previousPossiblesStack is not null,
                $"Heuristic must be initialized before calling {nameof(UndoLastUpdate)}.");
            IReadOnlyDictionary<Coordinate, BitVector>? overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach ((Coordinate coord, BitVector possibles) in overwrittenPossibles)
            {
                _puzzle.SetPossibleValues(in coord, possibles);
            }
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            Debug.Assert(_puzzle is not null
                && _possiblesToCheckOnDimension is not null
                && _previousPossiblesStack is not null,
                $"Heuristic must be initialized before calling {nameof(UpdateAll)}.");
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int dimension = 0; dimension < _possiblesToCheckOnDimension.Length; dimension++)
            {
                _possiblesToCheckOnDimension[dimension] = GetMissingValuesForDimension(dimension, _puzzle);
                _UpdateDimension(dimension, previousPossibles);
            }
            _previousPossiblesStack.Push(previousPossibles);
            return previousPossibles.Count > 0;
        }

        /// <summary>
        /// Computes the number of dimensions this heuristic ensures uniqueness over.
        /// 
        /// For example, <see cref="UniqueInRowHeuristic"/> returns the number of rows in the
        /// puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle being solved.</param>
        /// <returns>
        /// The number of unique dimensions enforced by this heuristic on the given
        /// puzzle.
        /// </returns>
        protected abstract int GetNumDimensions(IReadOnlyPuzzle puzzle);

        /// <summary>
        /// Gets the missing (i.e. still possible) values on the given dimension.
        /// </summary>
        /// <param name="dimension">
        /// The dimension on which to retrieve the missing values.
        /// </param>
        /// <param name="puzzle"></param>
        protected abstract BitVector GetMissingValuesForDimension(int dimension, IReadOnlyPuzzle puzzle);

        /// <summary>
        /// Gets the unset coordinates for the given puzzle on the requested dimension.
        /// </summary>
        protected abstract IEnumerable<Coordinate> GetUnsetCoordinatesOnDimension(int dimension, IReadOnlyPuzzle puzzle);

        private void _UpdateDimension(int dimension, Dictionary<Coordinate, BitVector> previousPossibles)
        {
            Span<int> possibleValues = stackalloc int[_possiblesToCheckOnDimension!.Length];
            int numPossible = _possiblesToCheckOnDimension[dimension].PopulateSetBits(possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possible = possibleValues[i];
                Coordinate? uniqueCoord = null;
                foreach (Coordinate c in GetUnsetCoordinatesOnDimension(dimension, _puzzle!))
                {
                    if (_puzzle!.GetPossibleValues(in c).IsBitSet(possible))
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
                previousPossibles[uniqueCoord.Value] = _puzzle!.GetPossibleValues(uniqueCoord.Value);
                _puzzle.SetPossibleValues(uniqueCoord.Value, possibles);
            }
        }
    }
}