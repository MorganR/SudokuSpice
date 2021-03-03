using System;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class PossibilityTest
    {
        [Fact]
        public void Constructor_SetsValuesAsExpected()
        {
            var possibility = new Possibility(new Coordinate(1, 1), 2);

            Assert.Equal(new Coordinate(1, 1), possibility.Coordinate);
            Assert.Equal(2, possibility.Index);
            Assert.Equal(NodeState.UNKNOWN, possibility.State);
        }

        [Fact]
        public void TryDrop_WithNoObjective_Drops()
        {
            var possibility = new Possibility(new(), 1);

            Assert.True(possibility.TryDrop());

            Assert.Equal(NodeState.DROPPED, possibility.State);
        }

        [Fact]
        public void CannotAddObjectives_AfterStateChange()
        {
            var possibility = new Possibility(new(), 1);

            Assert.True(possibility.TryDrop());

            Assert.Throws<InvalidOperationException>(
                () => Link.CreateConnectedLink(possibility, new FakeObjective()));
        }

        [Fact]
        public void TryDrop_WithObjective_Succeeds()
        {
            var possibility = new Possibility(new(), 1);
            var objective = new FakeObjective();
            var link = Link.CreateConnectedLink(possibility, objective);

            Assert.True(possibility.TryDrop());

            Assert.Single(objective.DroppedPossibilities, link);
            Assert.Equal(NodeState.DROPPED, possibility.State);
        }

        [Fact]
        public void TryDrop_RejectedByObjective_LeavesUnchanged()
        {
            var possibility = new Possibility(new(), 1);
            var objective = new FakeObjective();
            var objectiveToReject = new FakeObjective();
            objectiveToReject.CanDropPossibilities = false;
            Link.CreateConnectedLink(possibility, objective);
            Link.CreateConnectedLink(possibility, objectiveToReject);

            Assert.False(possibility.TryDrop());

            Assert.Equal(NodeState.UNKNOWN, possibility.State);
            Assert.Empty(objective.DroppedPossibilities);
            Assert.Empty(objectiveToReject.DroppedPossibilities);
        }

        [Fact]
        public void TrySelectAndDeselect_IfAcceptedByObjective_Succeeds()
        {
            var possibility = new Possibility(new(), 1);
            var objective = new FakeObjective();
            var link = Link.CreateConnectedLink(possibility, objective);

            Assert.True(possibility.TrySelect());

            Assert.Equal(NodeState.SELECTED, possibility.State);
            Assert.Single(objective.SelectedPossibilities, link);

            possibility.Deselect();

            Assert.Equal(NodeState.UNKNOWN, possibility.State);
            Assert.Empty(objective.SelectedPossibilities);
        }

        [Fact]
        public void TrySelect_IfRejectedByObjective_LeavesUnchanged()
        {
            var possibility = new Possibility(new(), 1);
            var objective = new FakeObjective();
            var objectiveToReject = new FakeObjective();
            objectiveToReject.CanSelectPossibilities = false;
            Link.CreateConnectedLink(possibility, objective);
            Link.CreateConnectedLink(possibility, objectiveToReject);

            Assert.False(possibility.TrySelect());

            Assert.Equal(NodeState.UNKNOWN, possibility.State);
            Assert.Empty(objective.SelectedPossibilities);
            Assert.Empty(objectiveToReject.SelectedPossibilities);
        }

        [Fact]
        public void TryDetachAndReattachObjective_IfRequiredObjective_DropsThenReturns()
        {
            var concretePossibility = new Possibility(new(), 1);
            IPossibility possibility = concretePossibility;
            var objective = new FakeObjective();
            var objectiveToDetach = new FakeObjective(isRequired: true);
            var linkToDrop = Link.CreateConnectedLink(possibility, objective);
            var linkToDetach = Link.CreateConnectedLink(possibility, objectiveToDetach);

            Assert.True(possibility.TryDropFromObjective(linkToDetach));
            Assert.Equal(NodeState.DROPPED, concretePossibility.State);
            Assert.Single(objective.DroppedPossibilities, linkToDrop);
            Assert.Empty(objectiveToDetach.DroppedPossibilities);

            possibility.ReturnFromObjective(linkToDetach);
            Assert.Equal(NodeState.UNKNOWN, concretePossibility.State);
            Assert.Empty(objective.DroppedPossibilities);
            Assert.Empty(objectiveToDetach.DroppedPossibilities);
        }

        [Fact]
        public void TryDetachAndReattachObjective_IfLinksToUniqueRequiredObjective_DropsThenReturns()
        {
            var concretePossibility = new Possibility(new(), 1);
            IPossibility possibility = concretePossibility;
            var objectiveToDropFrom = new FakeObjective();
            var objectiveToDetach = new FakeObjective();
            var sharedParent = new FakeObjective(isRequired: false);
            var uniqueParent = new FakeObjective(isRequired: true);
            var linkToDrop = Link.CreateConnectedLink(possibility, objectiveToDropFrom);
            var linkToDetach = Link.CreateConnectedLink(possibility, objectiveToDetach);

            Assert.True(possibility.TryDropFromObjective(linkToDetach));
            Assert.Equal(NodeState.DROPPED, concretePossibility.State);
            Assert.Single(objectiveToDropFrom.DroppedPossibilities, linkToDrop);
            Assert.Empty(objectiveToDetach.DroppedPossibilities);

            possibility.ReturnFromObjective(linkToDetach);
            Assert.Equal(NodeState.UNKNOWN, concretePossibility.State);
            Assert.Empty(objectiveToDropFrom.DroppedPossibilities);
            Assert.Empty(objectiveToDetach.DroppedPossibilities);
        }

        [Fact]
        public void TryDetach_CausingDrop_IfDropDisallowed_LeavesUnchanged()
        {
            var concretePossibility = new Possibility(new(), 1);
            IPossibility possibility = concretePossibility;
            var objective = new FakeObjective(isRequired: true);
            objective.CanDropPossibilities = false;
            var objectiveToDetach = new FakeObjective(isRequired: true);
            Link linkToOther = Link.CreateConnectedLink(possibility, objective);
            Link linkToDetach = Link.CreateConnectedLink(possibility, objectiveToDetach);

            Assert.False(possibility.TryDropFromObjective(linkToDetach));
            Assert.Equal(NodeState.UNKNOWN, concretePossibility.State);
            Assert.Empty(objective.DroppedPossibilities);
            Assert.Empty(objectiveToDetach.DroppedPossibilities);

            // Verify the attempted-detached objective was still attached by dropping the
            // possibility from it.
            Assert.True(possibility.TryDropFromObjective(linkToOther));
            Assert.Equal(NodeState.DROPPED, concretePossibility.State);
            Assert.Single(objectiveToDetach.DroppedPossibilities, linkToDetach);
        }

        [Fact]
        public void TrySelectAndDeselect_WhenSharedByOpposingObjectives_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var concretePossibility = new Possibility(new(), 1);
            var possibilities = new IPossibility[] { concretePossibility };
            var parentA = OptionalObjective.CreateWithPossibilities(possibilities, 1);
            var parentB = OptionalObjective.CreateWithPossibilities(possibilities.Append(new FakePossibility()).ToArray(), 1);
            var required = Objective.CreateFullyConnected(
                matrix,
                new IPossibility[] { parentA, parentB },
                1);

            Assert.False(concretePossibility.TrySelect());
            Assert.Equal(NodeState.UNKNOWN, parentA.State);
            Assert.Equal(NodeState.UNKNOWN, parentB.State);
            Assert.NotEqual(NodeState.SELECTED, required.State);
        }

        [Fact]
        public void CascadingDropUpDownUp()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), 2);
            var required = Objective.CreateFullyConnected(matrix, new ReadOnlySpan<Possibility>(possibilities), 1);
            var fakePossibility = new FakePossibility();
            var optional = OptionalObjective.CreateWithPossibilities(
                possibilities.Cast<IPossibility>().ToArray(), 2);
            var fakeLinkToOptional = Link.CreateConnectedLink(fakePossibility, optional);
            var separateRequired = Objective.CreateFullyConnected(matrix, new IPossibility[] { optional }, 1);


            // Additionally try one where optional ends up dropped before the cascade somehow... maybe add one to possibilities and to optional's countToSatisfy.

            Assert.True(possibilities[0].TrySelect());
            Assert.Equal(NodeState.SELECTED, possibilities[0].State);
            Assert.Equal(NodeState.DROPPED, possibilities[1].State);
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibility.DroppedFromObjectives);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.NotEqual(NodeState.SELECTED, separateRequired.State);

            Assert.True(((IObjective)optional).TrySelectPossibility(fakeLinkToOptional));
            Assert.Equal(NodeState.SELECTED, possibilities[0].State);
            Assert.Equal(NodeState.DROPPED, possibilities[1].State);
            Assert.Equal(NodeState.SELECTED, optional.State);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.Equal(NodeState.SELECTED, separateRequired.State);

            ((IObjective)optional).DeselectPossibility(fakeLinkToOptional);
            Assert.Equal(NodeState.SELECTED, possibilities[0].State);
            Assert.Equal(NodeState.DROPPED, possibilities[1].State);
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibility.DroppedFromObjectives);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.NotEqual(NodeState.SELECTED, separateRequired.State);

            Assert.False(((IObjective)optional).TryDropPossibility(fakeLinkToOptional));
        }

        [Fact]
        public void CascadingDropUpDownUpWithDropAtMidpoint()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), 3);
            var required = Objective.CreateFullyConnected(matrix, new ReadOnlySpan<Possibility>(possibilities), 1);
            var fakePossibility = new FakePossibility();
            var optional = OptionalObjective.CreateWithPossibilities(
                possibilities[1..3], 2);
            var fakeLinkToOptional = Link.CreateConnectedLink(fakePossibility, optional);
            // Add an extra fake possibility so this allows optional to be dropped.
            var separateRequired = Objective.CreateFullyConnected(matrix, new IPossibility[] { optional, new FakePossibility() }, 1);

            Assert.True(((IObjective)optional).TryDropPossibility(fakeLinkToOptional));
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.True(possibilities[0].TrySelect());
            Assert.Equal(NodeState.SELECTED, possibilities[0].State);
            Assert.Equal(NodeState.DROPPED, possibilities[1].State);
            Assert.Equal(NodeState.DROPPED, possibilities[2].State);
            Assert.Equal(NodeState.DROPPED, optional.State);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.NotEqual(NodeState.SELECTED, separateRequired.State);

            possibilities[0].Deselect();
            Assert.Equal(NodeState.UNKNOWN, possibilities[0].State);
            Assert.Equal(NodeState.UNKNOWN, possibilities[1].State);
            Assert.Equal(NodeState.UNKNOWN, possibilities[2].State);
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.NotEqual(NodeState.SELECTED, required.State);
            Assert.NotEqual(NodeState.SELECTED, separateRequired.State);
        }
    }
}
