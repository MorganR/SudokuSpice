using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class RequirementTest
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(1, false)]
        [InlineData(3, true)]
        [InlineData(3, false)]
        public void CreateFullyConnected_Works(int requiredCount, bool isOptional)
        {
            int size = 4;
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            var requirement = Requirement.CreateFullyConnected(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray(),
                requiredCount,
                isOptional);

            Assert.False(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.Equal(requiredCount == size, requirement.AreAllPossibilitiesRequired);
            Assert.Null(requirement.FirstGroupLink);
            Assert.All(requirement.FirstPossibilityLink.GetLinksOnObjective(), link => Assert.Equal(0, link.Possibility.Square.Coordinate.Row));
            Assert.Equal(4, requirement.FirstPossibilityLink.GetLinksOnObjective().Count());
            Assert.Equal(1, requirement.FirstPossibilityLink.GetLinksOnPossibility().Count());
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.Equal(1, matrix.GetUnsatisfiedRequirements().Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TrySelectAndDeselectPossibility_WithOneRequired_Works(bool isOptional)
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(matrix, possibleValues, requiredCount: 1, isOptional: isOptional);

            Assert.True(requirement.TrySelectPossibility(possibleValues[0].FirstLink));
            Assert.True(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.Equal(PossibilityState.DROPPED, possibleValues[1].State);
            Assert.Equal(PossibilityState.DROPPED, possibleValues[2].State);
            Assert.DoesNotContain(requirement, matrix.GetUnsatisfiedRequirements());

            requirement.DeselectPossibility(possibleValues[0].FirstLink);
            Assert.False(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[1].State);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[2].State);
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TrySelectAndDeselectPossibility_WithAllRequired_Works(bool isOptional)
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: possibleValues.Length, isOptional: isOptional);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);

            Assert.True(requirement.TrySelectPossibility(possibleValues[0].FirstLink));
            Assert.False(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[1].State);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[2].State);
            Assert.True(requirement.TrySelectPossibility(possibleValues[1].FirstLink));
            Assert.True(requirement.TrySelectPossibility(possibleValues[2].FirstLink));
            Assert.True(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.True(group.IsSatisfied);
            Assert.DoesNotContain(requirement, matrix.GetUnsatisfiedRequirements());

            requirement.DeselectPossibility(possibleValues[2].FirstLink);
            Assert.False(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.False(group.IsSatisfied);
            requirement.DeselectPossibility(possibleValues[1].FirstLink);
            requirement.DeselectPossibility(possibleValues[0].FirstLink);
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TrySelectPossibility_WithImpossibleDropFromPossibleValues_Fails(bool isOptional)
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: 1, isOptional: isOptional);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);
            _MakeOnlyPossibleOnSquare(possibleValues[1]);

            Assert.False(requirement.TrySelectPossibility(possibleValues[0].FirstLink));
            Assert.False(requirement.AreAllRequiredPossibilitiesSelected);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[1].State);
            Assert.Equal(PossibilityState.UNKNOWN, possibleValues[2].State);
            Assert.False(group.IsSatisfied);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(1, false)]
        public void TryDropAndReturnPossibility_Succeeds(int requiredCount, bool isOptional)
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: requiredCount, isOptional: isOptional);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);

            Assert.True(requirement.TryDropPossibility(possibleValues[0].FirstLink));
            Assert.Equal(2, requirement.FirstPossibilityLink.GetLinksOnObjective().Count());
            Assert.DoesNotContain(possibleValues[0].FirstLink, requirement.FirstPossibilityLink.GetLinksOnObjective());

            requirement.ReattachPossibility(possibleValues[0].FirstLink);
            Assert.Equal(3, requirement.FirstPossibilityLink.GetLinksOnObjective().Count());
            Assert.Contains(possibleValues[0].FirstLink, requirement.FirstPossibilityLink.GetLinksOnObjective());
        }

        [Theory]
        [InlineData(3)]
        [InlineData(1)]
        public void TryDropAndReturnRequiredPossibilities_WithOptionalRequirement_Succeeds(int requiredCount)
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: requiredCount, isOptional: true);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);

            Assert.True(requirement.TryDropPossibility(possibleValues[0].FirstLink));
            Assert.True(requirement.TryDropPossibility(possibleValues[1].FirstLink));
            Assert.True(requirement.TryDropPossibility(possibleValues[2].FirstLink));
            Assert.Null(requirement.FirstPossibilityLink);
            Assert.DoesNotContain(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.Null(group.ChildRequirementLink);

            requirement.ReattachPossibility(possibleValues[2].FirstLink);
            requirement.ReattachPossibility(possibleValues[1].FirstLink);
            requirement.ReattachPossibility(possibleValues[0].FirstLink);
            Assert.Equal(3, requirement.FirstPossibilityLink.GetLinksOnObjective().Count());
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.NotNull(group.ChildRequirementLink);
        }

        [Fact]
        public void TryDropPossibility_WhenNeededForGroup_Fails()
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: 3, isOptional: true);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: false);

            Assert.False(requirement.TryDropPossibility(possibleValues[0].FirstLink));
            Assert.Equal(3, requirement.FirstPossibilityLink.GetLinksOnObjective().Count());
        }

        [Fact]
        public void DetachAndReattachGroup_Succeeds()
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: 3, isOptional: true);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: false);

            requirement.DetachGroup(group.ChildRequirementLink);
            Assert.Null(requirement.FirstGroupLink);
            requirement.ReattachGroup(group.ChildRequirementLink);
            Assert.Equal(group.ChildRequirementLink, requirement.FirstGroupLink);
        }

        [Fact]
        public void TryDropAndReturn_WhenOptional_Succeeds()
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: 3, isOptional: true);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);

            Assert.True(requirement.TryDrop(group.ChildRequirementLink));
            Assert.DoesNotContain(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.All(possibleValues, value => Assert.Null(value.FirstLink));
            Assert.All(possibleValues, value => Assert.Equal(PossibilityState.DROPPED, value.State));

            requirement.Return(group.ChildRequirementLink);
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.All(possibleValues, value => Assert.NotNull(value.FirstLink));
            Assert.All(possibleValues, value => Assert.Equal(PossibilityState.UNKNOWN, value.State));
        }

        [Fact]
        public void TryDrop_WhenNotOptional_Fails()
        {
            (var matrix, var possibleValues) = _CreateIndependentPossibleSquareValues(3);
            var requirement = Requirement.CreateFullyConnected(
                matrix, possibleValues, requiredCount: 3, isOptional: false);
            var group = RequirementGroup.RequireAllOf(new Requirement[] { requirement }, isOptional: true);

            Assert.False(requirement.TryDrop(group.ChildRequirementLink));
            Assert.Contains(requirement, matrix.GetUnsatisfiedRequirements());
            Assert.All(possibleValues, value => Assert.NotNull(value.FirstLink));
        }



        private static (ExactCoverMatrix, PossibleSquareValue[]) _CreateIndependentPossibleSquareValues(int count)
        {
            int size = 4;
            if (count < 1 || count > size * size)
            {
                throw new ArgumentOutOfRangeException($"{nameof(count)} must be between 1 and {size * size}.");
            }
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            var psqs = new PossibleSquareValue[count];

            int row = 0;
            int col = 0;
            for (int i = 0; i < count; ++i)
            {
                psqs[i] = matrix.GetSquare(new Coordinate(row, col)).GetPossibleValue(0);
                ++col;
                if (col >= size)
                {
                    col = 0;
                    ++row;
                }
            }
            return (matrix, psqs);
        }

        private static void _MakeOnlyPossibleOnSquare(PossibleSquareValue psq)
        {
            int valueToKeep = psq.ValueIndex;
            foreach (var possibleValue in psq.Square.GetStillPossibleValues())
            {
                if (possibleValue.ValueIndex != valueToKeep)
                {
                    Assert.True(possibleValue.TryDrop());
                }
            }
        }
    }
}
