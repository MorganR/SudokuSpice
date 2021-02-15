using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces a constraint that certain regions in a puzzle must be
    /// <a href="https://en.wikipedia.org/wiki/Magic_square">magic squares</a>, i.e. the sums of the
    /// values in each of their rows, columns, and (optionally) their diagonals add up to the same
    /// value.
    ///
    /// Note that this does <em>not</em> enforce uniqueness of values within the magic square as a
    /// whole. It <em>does</em>, however, prevent value duplication within each row, column, and/or
    /// diagonal. This can be combined with the <see cref="BoxUniquenessConstraint"/> if you need
    /// box-level uniqueness.
    /// </summary>
    /// <remarks>
    /// This makes use of <see cref="OptionalObjective"/> objects to construct a complicated graph.
    ///
    /// For example, in a standard 3x3 magic square for a standard 9x9 Sudoku puzzle, the magic sum
    /// (i.e. required sum for each row/column/diagonal) is 15. This can be formed through various
    /// combinations, eg:
    /// 
    ///   * 1,5,9
    ///   * 1,6,8
    ///   ...
    ///   
    /// For each row or column or diagonal, this looks at the existing values to determine the
    /// possible sets. It drops impossible <see cref="Possibility"/> objects, and groups the
    /// remaining possibilities as follows (using the 1,5,9 set as an example):
    /// 
    /// In this row/column/diagonal, create an optional objective to require that a single 1 is
    /// selected from these squares. Repeat for the 5 and the 9. 
    /// 
    /// Then, group each of these optional objectives into another optional objective that requires
    /// all of them to be satisfied. This defines an individual possible set for this
    /// row/column/diagonal.
    /// 
    /// Repeat this for all the possible sets on this row/column/diagonal. Reuse groups where
    /// possible, for example set 1,6,8 would use the same "1" grouping from set 1,5,8.
    /// 
    /// Now group all these optional set objectives into a single required objective that can be
    /// satisfied by any of these optional sets.
    ///
    /// In the end, this results in a single required objective for each row/column/diagonal,
    /// enforcing that this row/column/diagonal is composed of one of the possible sets.
    /// </remarks>
    public class MagicSquaresConstraint : IConstraint
    {
        private readonly int _size;
        private readonly int _squareSize;
        private readonly bool _includeDiagonals;
        private readonly Box[] _magicSquares;
        private readonly BitVector _allPossibleValues;
        private readonly IReadOnlySet<BitVector> _possibleSets;

        /// <summary>
        /// Constructs a constraint that will enforce that the given <paramref name="squares"/> are
        /// magic squares based on the rows, columns, and, optionally, the diagonals.
        /// </summary>
        /// <param name="possibleValues">
        /// The possible values that can be in the magic squares.
        /// </param>
        /// <param name="squares">
        /// The locations of the magic squares.
        /// </param>
        /// <param name="includeDiagonals">
        /// If true, values along the diagonals of the square must also sum to the magic number.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the any of the given <paramref name="squares"/>' sizes are not compatible with the
        /// length of <paramref name="possibleValues"/>.
        /// </exception>
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

        /// <inheritdoc />
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
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

        private bool _TryConstrainBox(Box box, IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
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
            ExactCoverGraph matrix,
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

        private void _ConstrainAndClearOverlappingSets(ExactCoverGraph matrix, List<OptionalObjective> setsToOr)
        {
            Objective.CreateFullyConnected(matrix, setsToOr.ToArray(), countToSatisfy: 1);
            setsToOr.Clear();
        }
    }
}
