using System;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased.Heuristics
{
    public class UniqueInXHelper
    {
        private readonly Stack<IReadOnlyDictionary<Coordinate, BitVector>> _previousPossiblesStack;
        private readonly int _numPossibleValues;
        private readonly IReadOnlyPuzzleWithMutablePossibleValues _puzzle;

        public UniqueInXHelper(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            _previousPossiblesStack = new();
            _numPossibleValues = puzzle.UniquePossibleValues.ComputeCount();
            _puzzle = puzzle;
        }

        private UniqueInXHelper(UniqueInXHelper existing, IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            _previousPossiblesStack = new(existing._previousPossiblesStack);
            _numPossibleValues = existing._numPossibleValues;
            _puzzle = puzzle;
        }

        public UniqueInXHelper CopyWithNewReference(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            return new UniqueInXHelper(this, puzzle);
        }

        public void UndoLastUpdate()
        {
            IReadOnlyDictionary<Coordinate, BitVector> overwrittenPossibles = _previousPossiblesStack.Pop();
            foreach ((Coordinate coord, BitVector possibles) in overwrittenPossibles)
            {
                _puzzle.SetPossibleValues(in coord, possibles);
            }
        }

        public bool UpdateIfUnique(
            ReadOnlySpan<BitVector> possibleValuesToCheck,
            ReadOnlySpan<Coordinate[]> coordinatesToCheck)
        {
            var previousPossibles = new Dictionary<Coordinate, BitVector>();
            for (int dimension = 0; dimension < possibleValuesToCheck.Length; dimension++)
            {
                _UpdateUniqueCoordinates(
                    possibleValuesToCheck[dimension], coordinatesToCheck[dimension], previousPossibles);
            }
            _previousPossiblesStack.Push(previousPossibles);
            return previousPossibles.Count > 0;
        }

        private void _UpdateUniqueCoordinates(
            BitVector possiblesToCheck,
            ReadOnlySpan<Coordinate> coordinatesToCheck,
            Dictionary<Coordinate, BitVector> previousPossibles)
        {
            Span<int> possibleValues = stackalloc int[_numPossibleValues];
            int numPossible = possiblesToCheck.PopulateSetBits(possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possibleToCheck = possibleValues[i];
                Coordinate? uniqueCoord = null;
                foreach (Coordinate c in coordinatesToCheck)
                {
                    if (_puzzle.GetPossibleValues(in c).IsBitSet(possibleToCheck))
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
                possibles.SetBit(possibleToCheck);
                previousPossibles[uniqueCoord.Value] = _puzzle.GetPossibleValues(uniqueCoord.Value);
                _puzzle.SetPossibleValues(uniqueCoord.Value, possibles);
            }
        }
    }
}