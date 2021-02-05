using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class OptionalObjectiveTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(4, 1)]
        public void CreateWithPossibilities_ConnectsCorrectly(int numPossibilities, int numToSatisfy)
        {
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), numPossibilities);
            var concreteObjective = OptionalObjective.CreateWithPossibilities(possibilities, numToSatisfy);
            IObjective objective = concreteObjective;

            Assert.Equal(numToSatisfy, concreteObjective.TotalCountToSatisfy);
            Assert.Equal(PossibilityState.UNKNOWN, concreteObjective.State);
            Assert.False(objective.IsRequired);
            Assert.Empty(objective.RequiredObjectives);
            Assert.Equal(
                new HashSet<IPossibility>(possibilities),
                new HashSet<IPossibility>(objective.GetUnknownDirectPossibilities()));
        }

        [Fact]
        public void CreateWithPossibilities_NeedsMoreThanPossible_Throws()
        {
            var possibilities = Possibilities.CreatePossibilities(new Coordinate(), 1);

            Assert.Throws<ArgumentException>(
                () => OptionalObjective.CreateWithPossibilities(possibilities, 2));
        }

        [Fact]
        public void AppendObjective_WithRequiredObjective_ConnectsCorrectly()
        {
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2).Cast<IPossibility>().Append(childOptional).ToArray(),
                1);
            var parent = new FakeObjective(isRequired: true);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Link.CreateConnectedLink(possibility, parent);

            Assert.Equal(
                new HashSet<IObjective>() { parent },
                objective.RequiredObjectives);
            Assert.Equal(
                new HashSet<IObjective>() { parent },
                ((IObjective)childOptional).RequiredObjectives);
        }

        [Fact]
        public void TryDropAndReturnPossibility_WhenOkWithParent_Succeeds()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var optional = OptionalObjective.CreateWithPossibilities(fakePossibilities, 1);
            var parent = new FakeObjective(isRequired: true);
            IPossibility possibility = optional;
            IObjective objective = optional;
            var linkToParent = Link.CreateConnectedLink(possibility, parent);
            var childToDrop = fakePossibilities[0];

            Assert.True(objective.TryDropPossibility(childToDrop.AttachedObjectives.First()));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.DroppedPossibilities);

            Assert.True(objective.TryDropPossibility(fakePossibilities[1].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.DROPPED, optional.State);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Single(parent.DroppedPossibilities, linkToParent);

            objective.ReturnPossibility(fakePossibilities[1].AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.DroppedPossibilities);

            objective.ReturnPossibility(childToDrop.AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.DroppedPossibilities);
        }

        [Fact]
        public void TryDropPossibility_WhenRejectedByParent_LeavesUnchanged()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var possibilities = fakePossibilities.Cast<IPossibility>().Append(childOptional).ToArray();
            var optional = OptionalObjective.CreateWithPossibilities(possibilities, 3);
            var parent = new FakeObjective(isRequired: true);
            parent.CanDropPossibilities = false;
            IPossibility possibility = optional;
            IObjective objective = optional;
            Link.CreateConnectedLink(possibility, parent);
            var childToDrop = fakePossibilities[0];

            Assert.False(objective.TryDropPossibility(childToDrop.AttachedObjectives.First()));

            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Empty(parent.DroppedPossibilities);
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
        }

        [Fact]
        public void TryDropPossibility_WhenRejectedByChild_LeavesUnchanged()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var possibilities = fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray();
            var optional = OptionalObjective.CreateWithPossibilities(possibilities, 4);
            var parent = new FakeObjective(isRequired: true);
            IPossibility possibility = optional;
            IObjective objective = optional;
            Link.CreateConnectedLink(possibility, parent);
            var childToDrop = fakePossibilities[0];
            var childToBlock = fakePossibilities[1];
            childToBlock.CanBeDetached = false;

            Assert.False(objective.TryDropPossibility(childToDrop.AttachedObjectives.First()));

            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Empty(parent.DroppedPossibilities);
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
        }

        [Fact]
        public void TrySelectAndDeselectPossibility_Succeeds()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(fakePossibilities, 2);
            var linkToChildOptional = Link.CreateConnectedLink(childOptional, optional);
            var parent = new FakeObjective(isRequired: true);
            var linkToParent = Link.CreateConnectedLink(optional, parent);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Assert.True(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.SelectedPossibilities);

            Assert.True(objective.TrySelectPossibility(fakePossibilities[1].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.SELECTED, optional.State);
            Assert.Equal(PossibilityState.DROPPED, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.DROPPED, p.State));
            Assert.Single(parent.SelectedPossibilities, linkToParent);
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);

            objective.DeselectPossibility(fakePossibilities[1].AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.SelectedPossibilities);

            objective.DeselectPossibility(fakePossibilities[0].AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.SelectedPossibilities);
        }

        [Fact]
        public void TrySelectPossibility_WhenRejectedByParent_LeavesUnchanged()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(fakePossibilities, 1);
            var linkToChildOptional = Link.CreateConnectedLink(childOptional, optional);
            var parent = new FakeObjective(isRequired: true);
            parent.CanSelectPossibilities = false;
            var linkToParent = Link.CreateConnectedLink(optional, parent);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Assert.False(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.SelectedPossibilities);
        }

        [Fact]
        public void TrySelectPossibility_WhenRejectedBySibling_LeavesUnchanged()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parent = new FakeObjective(isRequired: true);
            var linkToParent = Link.CreateConnectedLink(optional, parent);
            IPossibility possibility = optional;
            IObjective objective = optional;
            fakePossibilities[1].CanBeDetached = false;

            Assert.False(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Empty(parent.SelectedPossibilities);
        }

        [Fact]
        public void TryDetachAndReattachObjective_IfRequiredObjective_DropsThenReturns()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parent = new FakeObjective(isRequired: true);
            var linkToParent = Link.CreateConnectedLink(optional, parent);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Assert.True(possibility.TryDetachObjective(linkToParent));
            Assert.Equal(PossibilityState.DROPPED, optional.State);
            Assert.Equal(PossibilityState.DROPPED, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.DROPPED, p.State));
            Assert.Empty(parent.SelectedPossibilities);
            Assert.Single(fakePossibilities[0].DetachedObjectives, fakePossibilities[0].AttachedObjectives.First());
            Assert.Single(fakePossibilities[1].DetachedObjectives, fakePossibilities[1].AttachedObjectives.First());

            possibility.ReattachObjective(linkToParent);
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(parent.SelectedPossibilities);
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
        }
        
        [Fact]
        public void TryDetachAndReattachObjective_IfLinksToUniqueRequiredObjective_DropsThenReturns()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parentOptionalA = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                }, 1);
            var linkToParentA = Link.CreateConnectedLink(optional, parentOptionalA);
            var parentOptionalB = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                    optional,
                }, 1);
            var requiredA = new FakeObjective(isRequired: true);
            var requiredB = new FakeObjective(isRequired: true);
            var requiredC = new FakeObjective(isRequired: true);
            Link.CreateConnectedLink(parentOptionalA, requiredA);
            Link.CreateConnectedLink(parentOptionalB, requiredB);
            Link.CreateConnectedLink(parentOptionalA, requiredC);
            Link.CreateConnectedLink(parentOptionalB, requiredC);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Assert.True(possibility.TryDetachObjective(linkToParentA));
            Assert.Equal(PossibilityState.DROPPED, optional.State);
            Assert.Equal(PossibilityState.DROPPED, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.DROPPED, p.State));
            Assert.Single(fakePossibilities[0].DetachedObjectives, fakePossibilities[0].AttachedObjectives.First());
            Assert.Single(fakePossibilities[1].DetachedObjectives, fakePossibilities[1].AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, parentOptionalB.State);

            possibility.ReattachObjective(linkToParentA);
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, parentOptionalB.State);
        }

        [Fact]
        public void TryDetachAndReattachObjective_IfRequiredObjectiveNotUnique_DoesNotDrop()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parentOptionalA = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                }, 1);
            var linkToParentA = Link.CreateConnectedLink(optional, parentOptionalA);
            var parentOptionalB = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                }, 1);
            var linkToParentB = Link.CreateConnectedLink(optional, parentOptionalB);
            var required = new FakeObjective(isRequired: true);
            Link.CreateConnectedLink(parentOptionalA, required);
            Link.CreateConnectedLink(parentOptionalB, required);
            IPossibility possibility = optional;
            IObjective objective = optional;

            Assert.True(possibility.TryDetachObjective(linkToParentA));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, parentOptionalB.State);

            Assert.True(possibility.TryDetachObjective(linkToParentB));
            Assert.Equal(PossibilityState.DROPPED, optional.State);
            Assert.Equal(PossibilityState.DROPPED, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.DROPPED, p.State));
            Assert.Single(fakePossibilities[0].DetachedObjectives, fakePossibilities[0].AttachedObjectives.First());
            Assert.Single(fakePossibilities[1].DetachedObjectives, fakePossibilities[1].AttachedObjectives.First());

            possibility.ReattachObjective(linkToParentB);
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, parentOptionalB.State);

            possibility.ReattachObjective(linkToParentA);
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
            Assert.Equal(PossibilityState.UNKNOWN, parentOptionalB.State);
        }

        [Fact]
        public void TryDetach_CausingDrop_IfDropDisallowed_LeavesUnchanged()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parent = new FakeObjective(isRequired: true);
            var linkToParent = Link.CreateConnectedLink(optional, parent);
            IPossibility possibility = optional;
            IObjective objective = optional;
            fakePossibilities[1].CanBeDetached = false;

            Assert.False(possibility.TryDetachObjective(linkToParent));
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(PossibilityState.UNKNOWN, p.State));
            Assert.Empty(parent.SelectedPossibilities);
            Assert.Empty(fakePossibilities[0].DetachedObjectives);
            Assert.Empty(fakePossibilities[1].DetachedObjectives);
        }

        [Fact]
        public void TrySelectPossibility_CausesCascadingDrop_SelectsAndDeselectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverMatrix.Create(puzzle);
            var fakePossibilities = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
            };
            var optional = OptionalObjective.CreateWithPossibilities(fakePossibilities, 1);
            var fakesOnParentToSelect = new FakePossibility[] {
                new FakePossibility(),
                new FakePossibility(),
            };
            var parentToSelect = OptionalObjective.CreateWithPossibilities(
                fakesOnParentToSelect.Cast<IPossibility>().Prepend(optional).ToArray(), 2);
            var fakeOnParentToDrop = new FakePossibility();
            var parentToDrop = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] { 
                    fakeOnParentToDrop,
                    optional,
                }, 2);
            var required = Objective.CreateFullyConnected(
                matrix,
                new IPossibility[] { parentToSelect, parentToDrop },
                1);
            IPossibility possibility = optional;
            IObjective objective = optional;
;
            // Select one fake on parentToSelect so that selecting this optional will satisfy the
            // objective.
            Assert.True(((IObjective)parentToSelect).TrySelectPossibility(fakesOnParentToSelect[0].AttachedObjectives.First()));
            Assert.True(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.SELECTED, optional.State);
            Assert.Equal(PossibilityState.SELECTED, parentToSelect.State);
            Assert.Single(fakesOnParentToSelect[1].DetachedObjectives);
            Assert.True(required.IsSatisfied);
            Assert.Equal(PossibilityState.DROPPED, parentToDrop.State);
            Assert.Single(fakeOnParentToDrop.DetachedObjectives);

            objective.DeselectPossibility(fakePossibilities[0].AttachedObjectives.First());
            Assert.Equal(PossibilityState.UNKNOWN, optional.State);
            Assert.Equal(PossibilityState.UNKNOWN, parentToSelect.State);
            Assert.Empty(fakesOnParentToSelect[1].DetachedObjectives);
            Assert.False(required.IsSatisfied);
            Assert.Equal(PossibilityState.UNKNOWN, parentToDrop.State);
            Assert.Empty(fakeOnParentToDrop.DetachedObjectives);

            // Select a possibility on parentToDrop first. This should result in no change overall.
            Assert.True(((IObjective)parentToDrop).TrySelectPossibility(fakeOnParentToDrop.AttachedObjectives.First()));
            
            // Selecting the objective would satisfy both optional parents, which violates the
            // required objective.
            Assert.False(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));

            // Deselect a possibility from parentToSelect so that we can now select parentToDrop.
            ((IObjective)parentToSelect).DeselectPossibility(fakesOnParentToSelect[0].AttachedObjectives.First());
            Assert.True(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(PossibilityState.SELECTED, optional.State);
            Assert.Equal(PossibilityState.DROPPED, parentToSelect.State);
            Assert.Single(fakesOnParentToSelect[1].DetachedObjectives);
            Assert.True(required.IsSatisfied);
            Assert.Equal(PossibilityState.SELECTED, parentToDrop.State);
            Assert.Empty(fakeOnParentToDrop.DetachedObjectives);
        }
    }
}
