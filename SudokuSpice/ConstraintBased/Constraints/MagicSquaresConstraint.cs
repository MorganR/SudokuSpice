using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased.Constraints
{
    public class MagicSquaresConstraint : IConstraint
    {
        private readonly int _size;
        private readonly int _squareSize;
        private readonly bool _includeDiagonals;
        private readonly Box[] _magicSquares;
        private readonly BitVector _allPossibleValues;
        private readonly IReadOnlySet<BitVector> _possibleSets;

        public MagicSquaresConstraint(ReadOnlySpan<int> possibleValues, IEnumerable<Box> squares, bool includeDiagonals = true)
        {
            _size = possibleValues.Length;
            _magicSquares = squares.ToArray();
            _squareSize = Boxes.IntSquareRoot(_size);
            _includeDiagonals = includeDiagonals;
            if (_magicSquares.Any(
                b => 
                b.TopLeft.Row < 0 || b.TopLeft.Column < 0 ||
                b.TopLeft.Row + b.Size > _size || b.TopLeft.Column + b.Size > _size ||
                b.Size != _squareSize))
            {
                throw new ArgumentException(
                    $"Based on the {nameof(possibleValues)}, {nameof(squares)} must fit in a puzzle of size {_size} and have size {_squareSize}.");
            }
            _allPossibleValues = new BitVector();
            for (int i = 0; i < possibleValues.Length; ++i)
            {
                if (_allPossibleValues.IsBitSet(possibleValues[i]))
                {
                    throw new ArgumentException("Values must be unique.");
                }
                _allPossibleValues.SetBit(possibleValues[i]);
            }
            _possibleSets = MagicSquares.ComputeSets(possibleValues, _squareSize, _allPossibleValues);
        }

        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            if (!_IsCompatible(puzzle))
            {
                return false;
            }
            foreach (Box box in _magicSquares)
            {
                if (!_TryConstrainBox(box, puzzle, matrix))
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
            return copiedSet.IsEmpty;
        }

        private bool _TryConstrainBox(Box box, IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Coordinate startCoord = box.TopLeft;
            Span<Coordinate> toConstrain = stackalloc Coordinate[_squareSize];
            List<OptionalObjective> setsToOr = new();
            for (int rowIdx = 0; rowIdx < _squareSize; ++rowIdx)
            {
                for (int i = 0; i < _squareSize; ++i)
                {
                    toConstrain[i] = new Coordinate(startCoord.Row + rowIdx, startCoord.Column + i);
                }
                if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
                {
                    return false;
                }
                _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            }
            for (int colIdx = 0; colIdx < _squareSize; ++colIdx)
            {
                for (int i = 0; i < _squareSize; ++i)
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
            int lastColumn = startCoord.Column + _squareSize - 1;
            for (int offset = 0; offset < _squareSize; ++offset)
            {
                toConstrain[offset] = new Coordinate(startCoord.Row + offset, lastColumn - offset);
            }
            if (!_TryConstrainToPossibleSets(toConstrain, puzzle, matrix, setsToOr))
            {
                return false;
            }
            _ConstrainAndClearOverlappingSets(matrix, setsToOr);
            for (int offset = 0; offset < _squareSize; ++offset)
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
            var relevantSets = _possibleSets.Where(set => BitVector.FindIntersect(set, alreadySet) == alreadySet)
                .ToArray();
            var relevantValues = relevantSets.Aggregate(BitVector.FindUnion);

            // alreadySet is a subset of relevantValues.
            var unsetRelevantValues = BitVector.FindDifference(relevantValues, alreadySet);
            // Drop all the values that are already set or are not part of any relevant sets.
            var valuesToDrop = BitVector.FindDifference(_allPossibleValues, unsetRelevantValues);
            foreach (var value in valuesToDrop.GetSetBits())
            {
                if (!ConstraintUtil.TryDropPossibilitiesAtIndex(unsetSquares[0..numUnset], matrix.ValuesToIndices[value]))
                {
                    return false;
                }
            }

            Dictionary<int, OptionalObjective> objectivesByPossibleValue = new();
            BitVector failedValues = new();
            foreach (var possibleValue in relevantValues.GetSetBits())
            {
                if (!ConstraintUtil.TryAddOptionalObjectiveForPossibilityIndex(
                    unsetSquares[0..numUnset],
                    matrix.ValuesToIndices[possibleValue],
                    matrix,
                    requiredCount: 1,
                    objective: out OptionalObjective? objective))
                {
                    failedValues.SetBit(possibleValue);
                    continue;
                }
                objectivesByPossibleValue[possibleValue] = objective!;
            }

            // Set requirements on the relevant values.
            OptionalObjective[] valuesToConnect = new OptionalObjective[numUnset];
            BitVector usedValues = new();
            foreach (BitVector set in relevantSets)
            {
                int countToConnect = 0;
                var unsetPossibleValuesInSet = new BitVector(set.Data ^ alreadySet.Data);
                // If one of the possible values in this set can't be grouped, then skip the set.
                if (!BitVector.FindIntersect(
                    unsetPossibleValuesInSet, failedValues).IsEmpty)
                {
                    continue;
                }
                // Create a requirement for each possible value.
                foreach (var possibleValue in unsetPossibleValuesInSet.GetSetBits())
                {
                    usedValues.SetBit(possibleValue);
                    valuesToConnect[countToConnect++] = objectivesByPossibleValue[possibleValue];
                }
                // If we have some requirements, group them in an optional "AND" grouping.
                Debug.Assert(countToConnect > 0);
                var setObjective = OptionalObjective.CreateWithPossibilities(valuesToConnect[0..countToConnect], countToSatisfy: countToConnect);
                setsToOr.Add(setObjective);
            }
            // usedValues are a subset of relevantValues.
            var unusedValues = BitVector.FindDifference(relevantValues, usedValues);
            // unusedValues are a superset of failedValues.
            var groupedValuesToDrop = BitVector.FindDifference(unusedValues, failedValues);
            if (!groupedValuesToDrop.IsEmpty)
            {
                // These values were grouped into an optional objective, but that objective is not
                // connected up to a required objective. That means these values are actually
                // impossible, so drop the possible values altogether.
                foreach (var valueToDrop in groupedValuesToDrop.GetSetBits())
                {
                    if (!ConstraintUtil.TryDropPossibilitiesAtIndex(
                        unsetSquares[0..numUnset], matrix.ValuesToIndices[valueToDrop]))
                    {
                        return false;
                    }
                }
            }
            return setsToOr.Count > 0;
        } 

        private void _ConstrainAndClearOverlappingSets(ExactCoverMatrix matrix, List<OptionalObjective> setsToOr)
        {
            Objective.CreateFullyConnected(matrix, setsToOr.ToArray(), countToSatisfy: 1);
            setsToOr.Clear();
        }
    }
}
