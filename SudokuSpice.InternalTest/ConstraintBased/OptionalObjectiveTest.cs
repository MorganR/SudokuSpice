﻿using System;
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
            Assert.Equal(NodeState.UNKNOWN, concreteObjective.State);
            Assert.False(objective.IsRequired);
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
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Empty(parent.DroppedPossibilities);

            Assert.True(objective.TryDropPossibility(fakePossibilities[1].AttachedObjectives.First()));
            Assert.Equal(NodeState.DROPPED, optional.State);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Single(parent.DroppedPossibilities, linkToParent);

            objective.ReturnPossibility(fakePossibilities[1].AttachedObjectives.First());
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Empty(parent.DroppedPossibilities);

            objective.ReturnPossibility(childToDrop.AttachedObjectives.First());
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
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

            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Empty(parent.DroppedPossibilities);
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
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
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Empty(parent.SelectedPossibilities);

            Assert.True(objective.TrySelectPossibility(fakePossibilities[1].AttachedObjectives.First()));
            Assert.Equal(NodeState.SELECTED, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Single(parent.SelectedPossibilities, linkToParent);
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);

            objective.DeselectPossibility(fakePossibilities[1].AttachedObjectives.First());
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Empty(parent.SelectedPossibilities);

            objective.DeselectPossibility(fakePossibilities[0].AttachedObjectives.First());
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
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
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
            Assert.Empty(parent.SelectedPossibilities);
        }

        [Fact]
        public void TryDropAndReturnFromObjective_DropsThenReturns()
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
            var required = new FakeObjective(isRequired: true);
            Link.CreateConnectedLink(parentOptionalA, required);
            Link.CreateConnectedLink(parentOptionalB, required);
            IPossibility possibility = optional;

            Assert.True(possibility.TryDropFromObjective(linkToParentA));
            Assert.Equal(NodeState.DROPPED, optional.State);
            Assert.Equal(NodeState.UNKNOWN, parentOptionalB.State);

            possibility.ReturnFromObjective(linkToParentA);
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, parentOptionalB.State);
        }

        [Fact]
        public void TryDropAndReturnFromObjective_CascadesToOtherParents_DropsThenReturns()
        {
            var fakePossibilities = new FakePossibility[] {
                    new FakePossibility(),
                    new FakePossibility(),
            };
            var childOptional = OptionalObjective.CreateWithPossibilities(
                Possibilities.CreatePossibilities(new(), 2), 1);
            var optional = OptionalObjective.CreateWithPossibilities(
                fakePossibilities.Cast<IPossibility>().Prepend(childOptional).ToArray(), 1);
            var parentToDropFrom = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                }, 1);
            var linkToParent = Link.CreateConnectedLink(optional, parentToDropFrom);
            var parentToCascadeDrop = OptionalObjective.CreateWithPossibilities(
                new IPossibility[] {
                    new FakePossibility(),
                    optional,
                }, 2);
            var required = new FakeObjective(isRequired: true);
            Link.CreateConnectedLink(parentToDropFrom, required);
            Link.CreateConnectedLink(parentToCascadeDrop, required);
            IPossibility possibility = optional;

            Assert.True(possibility.TryDropFromObjective(linkToParent));
            Assert.Equal(NodeState.DROPPED, optional.State);
            Assert.Equal(NodeState.DROPPED, parentToCascadeDrop.State);

            possibility.ReturnFromObjective(linkToParent);
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, parentToCascadeDrop.State);
        }

        [Fact]
        public void TryDropFromObjective_LeavesPossibilitiesUnchanged()
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

            Assert.True(possibility.TryDropFromObjective(linkToParent));
            Assert.Equal(NodeState.DROPPED, optional.State);
            Assert.Equal(NodeState.UNKNOWN, childOptional.State);
            Assert.All(((IObjective)childOptional).GetUnknownDirectPossibilities().Cast<Possibility>(),
                p => Assert.Equal(NodeState.UNKNOWN, p.State));
            Assert.Empty(parent.SelectedPossibilities);
            Assert.Empty(fakePossibilities[0].DroppedFromObjectives);
            Assert.Empty(fakePossibilities[1].DroppedFromObjectives);
        }

        [Fact]
        public void TrySelectPossibility_CausesCascadingDrop_SelectsAndDeselectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverGraph.Create(puzzle);
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
            IObjective objective = optional;
            ;
            // Select one fake on parentToSelect so that selecting this optional will satisfy the
            // objective.
            Assert.True(((IObjective)parentToSelect).TrySelectPossibility(fakesOnParentToSelect[0].AttachedObjectives.First()));
            Assert.True(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(NodeState.SELECTED, optional.State);
            Assert.Equal(NodeState.SELECTED, parentToSelect.State);
            Assert.Empty(fakesOnParentToSelect[1].DroppedFromObjectives);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.Equal(NodeState.DROPPED, parentToDrop.State);
            Assert.Empty(fakeOnParentToDrop.DroppedFromObjectives);

            objective.DeselectPossibility(fakePossibilities[0].AttachedObjectives.First());
            Assert.Equal(NodeState.UNKNOWN, optional.State);
            Assert.Equal(NodeState.UNKNOWN, parentToSelect.State);
            Assert.Empty(fakesOnParentToSelect[1].DroppedFromObjectives);
            Assert.Equal(NodeState.UNKNOWN, required.State);
            Assert.Equal(NodeState.UNKNOWN, parentToDrop.State);
            Assert.Empty(fakeOnParentToDrop.DroppedFromObjectives);

            // Select a possibility on parentToDrop first. This should result in no change overall.
            Assert.True(((IObjective)parentToDrop).TrySelectPossibility(fakeOnParentToDrop.AttachedObjectives.First()));

            // Selecting the objective would satisfy both optional parents, which violates the
            // required objective.
            Assert.False(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));

            // Deselect a possibility from parentToSelect so that we can now select parentToDrop.
            ((IObjective)parentToSelect).DeselectPossibility(fakesOnParentToSelect[0].AttachedObjectives.First());
            Assert.True(objective.TrySelectPossibility(fakePossibilities[0].AttachedObjectives.First()));
            Assert.Equal(NodeState.SELECTED, optional.State);
            Assert.Equal(NodeState.DROPPED, parentToSelect.State);
            Assert.Empty(fakesOnParentToSelect[1].DroppedFromObjectives);
            Assert.Equal(NodeState.SELECTED, required.State);
            Assert.Equal(NodeState.SELECTED, parentToDrop.State);
            Assert.Empty(fakeOnParentToDrop.DroppedFromObjectives);
        }
    }
}