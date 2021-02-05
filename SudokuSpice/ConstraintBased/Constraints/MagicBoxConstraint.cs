using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased.Constraints
{
    public class MagicBoxConstraint : IConstraint
    {
        private readonly int _size;
        private readonly int _boxSize;
        private readonly bool _includeDiagonals;
        private readonly int[] _magicBoxIndices;
        private readonly BitVector _allPossibleValues;
        private readonly IReadOnlySet<BitVector> _possibleSets;

        public MagicBoxConstraint(int size, ReadOnlySpan<int> magicBoxIndices, bool includeDiagonals = true)
        {
            _size = size;
            _boxSize = Boxes.CalculateBoxSize(size);
            _includeDiagonals = includeDiagonals;
            _magicBoxIndices = magicBoxIndices.ToArray();
            if (_magicBoxIndices.Any(idx => idx < 0 || idx >= _size))
            {
                throw new ArgumentException($"Box indices must be in the range [0,{nameof(size)}-1].");
            }
            _allPossibleValues = BitVector.CreateWithSize(size + 1);
            _allPossibleValues.UnsetBit(0);
            _possibleSets = _ComputeMagicSets(_size, _boxSize, _allPossibleValues);
        }

        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            if (!_IsCompatible(puzzle))
            {
                return false;
            }
            foreach (int boxIndex in _magicBoxIndices)
            {
                if (!_TryConstrainBox(boxIndex, puzzle, matrix))
                {
                    return false;
                }
            }
            return true;
        }

        private bool _IsCompatible(IReadOnlyPuzzle puzzle)
        {
            if (_size != puzzle.Size)
            {
                return false;
            }
            BitVector copiedSet = _allPossibleValues;
            for (int i = 0; i < _size; ++i)
            {
                int possibleValue = puzzle.AllPossibleValuesSpan[i];
                if (!copiedSet.IsBitSet(possibleValue))
                {
                    return false;
                }
                copiedSet.UnsetBit(possibleValue);
            }
            return copiedSet.IsEmpty();
        }

        private bool _TryConstrainBox(int boxIndex, IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Coordinate startCoord = Boxes.GetStartingBoxCoordinate(boxIndex, _boxSize);
            Span<Coordinate> toConstrain = stackalloc Coordinate[_boxSize];
            List<OptionalObjective> setsToOr = new();
            for (int rowIdx = 0; rowIdx < _boxSize; ++rowIdx)
            {
                for (int i = 0; i < _boxSize; ++i)
                {
                    toConstrain[i] = new Coordinate(startCoord.Row + rowIdx, startCoord.Column + i);
                }
                if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
                {
                    return false;
                }
                _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            }
            for (int colIdx = 0; colIdx < _boxSize; ++colIdx)
            {
                for (int i = 0; i < _boxSize; ++i)
                {
                    toConstrain[i] = new Coordinate(startCoord.Row + i, startCoord.Column + colIdx);
                }
                if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
                {
                    return false;
                }
                _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            }
            if (!_includeDiagonals)
            {
                return true;
            }
            int lastColumn = startCoord.Column + _boxSize - 1;
            for (int offset = 0; offset < _boxSize; ++offset)
            {
                toConstrain[offset] = new Coordinate(startCoord.Row + offset, lastColumn - offset);
            }
            if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
            {
                return false;
            }
            _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            for (int offset = 0; offset < _boxSize; ++offset)
            {
                toConstrain[offset] = new Coordinate(startCoord.Row + offset, startCoord.Column + offset);
            }
            if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
            {
                return false;
            }
            _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            return true;
        }

        private bool _TryConstrainToPossibleSets(
            ReadOnlySpan<Coordinate> toConstrain,
            IReadOnlyPuzzle puzzle,
            ExactCoverMatrix matrix,
            List<OptionalObjective> setsToOr)
        {
            Possibility?[]?[] unsetSquares = new Possibility[toConstrain.Length][];
            BitVector alreadySet = new BitVector();
            int numUnset = 0;
            for (int i = 0; i < toConstrain.Length; ++i)
            {
                var square = puzzle[in toConstrain[i]];
                if (square.HasValue)
                {
                    alreadySet.SetBit(square.Value);
                } else
                {
                    unsetSquares[numUnset++] = matrix.GetAllPossibilitiesAt(in toConstrain[i])!;
                }
            }
            if (numUnset == 0)
            {
                return _possibleSets.Contains(alreadySet);
            }
            Possibility[] possibilities = new Possibility[numUnset];
            Dictionary<int, OptionalObjective> objectivesByPossibleValue = new();
            OptionalObjective[] objectivesToConnect = new OptionalObjective[numUnset];
            var relevantSets = _possibleSets.Where(set => BitVector.FindIntersect(set, alreadySet) == alreadySet)
                .ToArray();
            var relevantValues = relevantSets.Aggregate((union, set) => BitVector.FindUnion(union, set));

            // Drop the irrelevant values.

            // Just the values in relevantValues that are not part of alreadySet, since alreadySet
            // is a subset of relevantValues.
            var unsetValues = new BitVector(relevantValues.Data ^ alreadySet.Data);
            var valuesToDrop = new BitVector(_allPossibleValues.Data ^ unsetValues.Data);
            foreach (var value in valuesToDrop.GetSetBits())
            {
                if (!ConstraintUtil.TryDropPossibilitiesAtIndex(unsetSquares[0..numUnset], matrix.ValuesToIndices[value]))
                {
                    return false;
                }
            }

            // Set requirements on the relevant values.
            foreach (BitVector set in relevantSets)
            {
                int countToConnect = 0;
                var unsetPossibleValues = new BitVector(set.Data ^ alreadySet.Data);
                // Create a requirement for each possible value.
                foreach (var possibleValue in unsetPossibleValues.GetSetBits())
                {
                    if (objectivesByPossibleValue.TryGetValue(possibleValue, out OptionalObjective? existingObjective))
                    {
                        objectivesToConnect[countToConnect++] = existingObjective;
                        continue;
                    }
                    if (!ConstraintUtil.TryAddOptionalObjectiveForPossibilityIndex(
                        unsetSquares[0..numUnset],
                        matrix.ValuesToIndices[possibleValue],
                        matrix,
                        requiredCount: 1,
                        objective: out OptionalObjective? objective))
                    {
                        continue;
                    }
                    Debug.Assert(objective is not null); // Should always be set if above was true.
                    objectivesToConnect[countToConnect++] = objective;
                    objectivesByPossibleValue[possibleValue] = objective;
                }
                // If we have some requirements, group them in an optional "AND" grouping.
                if (countToConnect == 0)
                {
                    continue;
                }
                var group = OptionalObjective.CreateWithPossibilities(objectivesToConnect[0..countToConnect], countToSatisfy: countToConnect);
                setsToOr.Add(group);
            }
            return setsToOr.Count > 0;
        } 

        private void _ConstrainAndClearOverlappingSets(ExactCoverMatrix matrix, List<OptionalObjective> setsToOr)
        {
            Objective.CreateFullyConnected(matrix, setsToOr.ToArray(), countToSatisfy: 1);
            setsToOr.Clear();
        }

        private static HashSet<BitVector> _ComputeMagicSets(int size, int boxSize, BitVector allPossibleValues)
        {
            int magicSum = _ComputeMagicSquareSum(size, boxSize);
            var sets = new HashSet<BitVector>();
            foreach (int possibleValue in allPossibleValues.GetSetBits())
            {
                var set = new BitVector();
                set.SetBit(possibleValue);
                BitVector possibleValues = allPossibleValues;
                possibleValues.UnsetBit(possibleValue);
                sets = new (sets.Union(_ComputeMagicSetsForRemainder(boxSize - 1, magicSum - possibleValue, possibleValues, set)));
            }
            return sets;
        }

        private static int _ComputeMagicSquareSum(int size, int boxSize)
        {
            int sum = 0;
            for (int i = 1; i <= size; ++i)
            {
                sum += i;
            }
            return sum / boxSize;
        }

        private static HashSet<BitVector> _ComputeMagicSetsForRemainder(int remainingSize, int remainder, BitVector possibleValues, BitVector partialSet)
        {
            var result = new HashSet<BitVector>();
            if (remainingSize == 0)
            {
                if (remainder == 0)
                {
                    result.Add(partialSet);
                }
                return result;
            }
            foreach (var possibleValue in possibleValues.GetSetBits())
            {
                if (remainder - possibleValue >= 0)
                {
                    BitVector set = partialSet;
                    set.SetBit(possibleValue);
                    BitVector reducedPossibleValues = possibleValues;
                    reducedPossibleValues.UnsetBit(possibleValue);
                    result = new (result.Union(_ComputeMagicSetsForRemainder(remainingSize - 1, remainder - possibleValue, reducedPossibleValues, set)));
                }
            }
            return result;
        }
    }
}
