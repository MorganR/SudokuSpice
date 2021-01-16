using System;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class RequirementTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateFullyConnected_WithManyPossible_Succeeds(int numRequired)
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            var result = Requirement.CreateFullyConnected(matrix, values.AsSpan(), numRequired);

            Assert.Contains(result, matrix.GetUnsatisfiedRequirements());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateFullyConnected_WithExactPossibles_Succeeds(int numRequired)
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            var result = Requirement.CreateFullyConnected(matrix, values[0..numRequired], numRequired);

            Assert.Contains(result, matrix.GetUnsatisfiedRequirements());
        }

        [Fact]
        public void CreateFullyConnected_WithNoPossible_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);

            var emptyPossibles = new Possibility[0];
            Assert.Throws<ArgumentException>(() => Requirement.CreateFullyConnected(matrix, emptyPossibles.AsSpan(), 1));
        }

        [Fact]
        public void CreateFullyConnected_WithTooFewPossible_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            Assert.Throws<ArgumentException>(() => Requirement.CreateFullyConnected(matrix, values[0..1], 2));
        }
    }
}
