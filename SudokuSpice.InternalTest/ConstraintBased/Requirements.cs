using System;
using System.Linq;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    class Requirements
    {
        internal static Requirement[] CreateIndependentOptionalRequirements(int number)
        {
            int size = 4;
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);
            _CreateOptionalRowUniquenessRequirements(size, matrix);
            var requirements = new Requirement[number];
            for(int valueIndex = 0; valueIndex < size; ++valueIndex)
            {
                for (int row = 0; row < size; ++row)
                {
                    if (number == 0)
                    {
                        return requirements;
                    }
                    requirements[--number] = matrix.GetSquare(new Coordinate(row, 0)).GetPossibleValue(valueIndex).FirstLink.Objective;
                }
            }
            throw new ArgumentException($"Requested too many requirements: {number}");
        }

        private static void _CreateOptionalRowUniquenessRequirements(int size, ExactCoverMatrix matrix)
        {
            for (int row = 0; row < size; ++row)
            {
                var possibleSquareValues = new PossibleSquareValue[size];
                for (int valueIdx = 0; valueIdx < size; ++valueIdx)
                {
                    Requirement.CreateFullyConnected(matrix, matrix.GetSquaresOnRow(row).ToArray().Select(s => s.GetPossibleValue(valueIdx)!).ToArray(), isOptional: true);
                }
            }
        }
    }
}
