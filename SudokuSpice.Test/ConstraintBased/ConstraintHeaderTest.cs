using System;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class ConstraintHeaderTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateConnectedHeader_WithManyPossible_Succeeds(int numRequired)
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            var result = ConstraintHeader.CreateConnectedHeader(matrix, values.AsSpan(), numRequired);

            Assert.Contains(result, matrix.GetUnsatisfiedConstraintHeaders());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateConnectedHeader_WithExactPossibles_Succeeds(int numRequired)
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            var result = ConstraintHeader.CreateConnectedHeader(matrix, values[0..numRequired], numRequired);

            Assert.Contains(result, matrix.GetUnsatisfiedConstraintHeaders());
        }

        [Fact]
        public void CreateConnectedHeader_WithNoPossible_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);

            var emptyPossibles = new PossibleSquareValue[0];
            Assert.Throws<ArgumentException>(() => ConstraintHeader.CreateConnectedHeader(matrix, emptyPossibles.AsSpan(), 1));
        }

        [Fact]
        public void CreateConnectedHeader_WithTooFewPossible_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var values = matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray();

            Assert.Throws<ArgumentException>(() => ConstraintHeader.CreateConnectedHeader(matrix, values[0..1], 2));
        }
    }
}
