using System;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class RequirementGroupTest
    {
        [Theory]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(3, true)]
        public void Create_WithRequirements_WorksAsExpected(int requiredCount, bool isOptional)
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(3);
            var group = RequirementGroup.CreateFullyConnected(reqs, requiredCount, isOptional);

            // Should never be satisfied when first created.
            Assert.False(group.IsSatisfied);
            if (requiredCount == reqs.Length)
            {
                Assert.True(group.AreAllPossibilitiesRequired);
            } else
            {
                Assert.False(group.AreAllPossibilitiesRequired);
            }

            Assert.Equal(1, reqs[0].FirstGroupLink.GetLinksOnPossibility().Count());
            Assert.Equal(1, reqs[1].FirstGroupLink.GetLinksOnPossibility().Count());
            Assert.Equal(1, reqs[2].FirstGroupLink.GetLinksOnPossibility().Count());
            Assert.Equal(3, group.ChildRequirementLink.GetLinksOnObjective().Count());
            Assert.Contains(reqs[1].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[1].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.Contains(reqs[1].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
        }

        [Fact]
        public void TryDropPossibility_WithAllRequired_FailsIfNotOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(3);
            var group = RequirementGroup.CreateFullyConnected(reqs, requiredCount: reqs.Length, isOptional: false);

            Assert.False(group.TryDropPossibility(reqs[0].FirstGroupLink));
        }

        [Fact]
        public void TryDropThenReturnPossibility_WithAllRequired_SucceedsIfOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(3);
            var group = RequirementGroup.CreateFullyConnected(reqs, requiredCount: reqs.Length, isOptional: true);

            Assert.True(group.TryDropPossibility(reqs[0].FirstGroupLink));
            Assert.DoesNotContain(reqs[0].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.Null(reqs[1].FirstGroupLink);
            Assert.Null(reqs[2].FirstGroupLink);

            group.ReturnPossibility(reqs[0].FirstGroupLink);
            Assert.Contains(reqs[0].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.Contains(reqs[1].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());

            Assert.Contains(reqs[1].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[1].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
        }

        [Fact]
        public void TryDropThenReturnPossibility_WithOnlyOneRequired_SucceedsIfNotOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(3);
            var group = RequirementGroup.CreateFullyConnected(reqs, requiredCount: 1, isOptional: false);

            Assert.True(group.TryDropPossibility(reqs[0].FirstGroupLink));
            Assert.True(group.TryDropPossibility(reqs[1].FirstGroupLink));

            group.ReturnPossibility(reqs[1].FirstGroupLink);
            group.ReturnPossibility(reqs[0].FirstGroupLink);

            Assert.Contains(reqs[1].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[0].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[2].FirstGroupLink, reqs[1].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[0].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
            Assert.Contains(reqs[1].FirstGroupLink, reqs[2].FirstGroupLink.GetLinksOnObjective());
        }

        [Fact]
        public void TrySelectAndDeselect_WithAllRequired_Succeeds()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(3);
            var group = RequirementGroup.CreateFullyConnected(reqs, requiredCount: reqs.Length, isOptional: true);

            Assert.True(group.TrySelectPossibility(reqs[0].FirstGroupLink));
            Assert.False(group.IsSatisfied);
            Assert.DoesNotContain(reqs[0].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.True(group.TrySelectPossibility(reqs[1].FirstGroupLink));
            Assert.DoesNotContain(reqs[1].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            Assert.False(group.IsSatisfied);
            Assert.True(group.TrySelectPossibility(reqs[2].FirstGroupLink));
            Assert.True(group.IsSatisfied);

            group.DeselectPossibility(reqs[2].FirstGroupLink);
            Assert.False(group.IsSatisfied);
            group.DeselectPossibility(reqs[1].FirstGroupLink);
            Assert.Contains(reqs[1].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
            group.DeselectPossibility(reqs[0].FirstGroupLink);
            Assert.Contains(reqs[0].FirstGroupLink, group.ChildRequirementLink.GetLinksOnObjective());
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(3, true)]

        public void Create_WithGroups_WorksAsExpected(int requiredCount, bool isOptional)
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.CreateFullyConnected(children, requiredCount, isOptional);

            Assert.False(group.IsSatisfied);
            Assert.Contains(children[1].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[1].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
        }

        [Fact]
        public void TryDropPossibility_WithAllGroupsRequired_FailsIfNotOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.CreateFullyConnected(children, requiredCount: children.Length, isOptional: false);

            Assert.False(group.TryDropPossibility(children[0].ParentGroupLink));
        }

        [Fact]
        public void TryDropThenReturnPossibility_WithSomeGroupsRequired_SucceedsAsLongAsMoreGroupsExist()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.CreateFullyConnected(children, requiredCount: 1, isOptional: false);

            Assert.True(group.TryDropPossibility(children[0].ParentGroupLink));
            Assert.DoesNotContain(children[0].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.True(group.TryDropPossibility(children[1].ParentGroupLink));
            Assert.DoesNotContain(children[1].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.True(group.AreAllPossibilitiesRequired);
            Assert.False(group.TryDropPossibility(children[2].ParentGroupLink));
            Assert.True(group.AreAllPossibilitiesRequired);
            group.ReturnPossibility(children[1].ParentGroupLink);
            Assert.Contains(children[1].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            group.ReturnPossibility(children[0].ParentGroupLink);
            Assert.Contains(children[0].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());

            Assert.Contains(children[1].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[1].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
        }

        [Fact]
        public void TryDropThenReturnPossibility_WithAllGroupsRequired_SucceedsIfOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.CreateFullyConnected(children, requiredCount: children.Length, isOptional: true);

            Assert.True(group.TryDropPossibility(children[0].ParentGroupLink));
            Assert.DoesNotContain(children[0].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.DoesNotContain(children[1].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.DoesNotContain(children[2].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());

            group.ReturnPossibility(children[0].ParentGroupLink);
            Assert.Contains(children[0].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.Contains(children[1].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, group.ChildGroupLink.GetLinksOnObjective());

            Assert.Contains(children[1].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[0].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[2].ParentGroupLink, children[1].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[0].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
            Assert.Contains(children[1].ParentGroupLink, children[2].ParentGroupLink.GetLinksOnObjective());
        }

        [Fact]
        public void TrySelectAndDeselect_WithAllGroupsRequired_Succeeds()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.RequireAllOf(children, isOptional: true);

            Assert.True(group.TrySelectPossibility(children[0].ParentGroupLink));
            Assert.False(group.IsSatisfied);
            Assert.True(group.TrySelectPossibility(children[1].ParentGroupLink));
            Assert.False(group.IsSatisfied);
            Assert.True(group.TrySelectPossibility(children[2].ParentGroupLink));
            Assert.True(group.IsSatisfied);

            group.DeselectPossibility(children[2].ParentGroupLink);
            Assert.False(group.IsSatisfied);
            group.DeselectPossibility(children[1].ParentGroupLink);
            group.DeselectPossibility(children[0].ParentGroupLink);
        }

        [Fact]
        public void TrySelectAndDeselect_WithOneGroupRequired_Succeeds()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var group = RequirementGroup.RequireOneOf(children, isOptional: true);

            Assert.True(group.TrySelectPossibility(children[0].ParentGroupLink));
            Assert.True(group.IsSatisfied);

            group.DeselectPossibility(children[0].ParentGroupLink);
            Assert.False(group.IsSatisfied);
        }

        [Theory]
        [InlineData(3, true)]
        [InlineData(1, false)]
        public void TryDropAndReturn_WithRequirements_Works(int requiredCount, bool isOptional)
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var parentGroup = RequirementGroup.CreateFullyConnected(children, requiredCount, isOptional);

            var childLink = parentGroup.ChildGroupLink;
            var childGroup = childLink.Possibility;
            Assert.True(childGroup.TryDrop(childLink));
            var grandChildRequirement = childGroup.ChildRequirementLink.Possibility;
            Assert.True(
                grandChildRequirement.FirstGroupLink is null
                || grandChildRequirement.FirstGroupLink.GetLinksOnPossibility().Contains(childGroup.ChildRequirementLink));
            childGroup.Return(childLink);
            Assert.Contains(
                childGroup.ChildRequirementLink,
                grandChildRequirement.FirstGroupLink.GetLinksOnPossibility());
        }

        [Fact]
        public void TryDrop_WithRequirements_FailsIfNonOptional()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: false),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: false),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: false),
            };
            var parentGroup = RequirementGroup.RequireOneOf(children, isOptional: false);

            var childLink = parentGroup.ChildGroupLink;
            var childGroup = childLink.Possibility;
            Assert.False(childGroup.TryDrop(childLink));
            Assert.Contains(childGroup.ChildRequirementLink, childGroup.ChildRequirementLink.Possibility.FirstGroupLink.GetLinksOnPossibility());
        }

        [Fact]
        public void TryDrop_RequiredByOtherParent_Fails()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var parentGroup = RequirementGroup.RequireOneOf(children, isOptional: false);
            _ = RequirementGroup.RequireAllOf(children, isOptional: false);

            var childLink = parentGroup.ChildGroupLink;
            var childGroup = childLink.Possibility;
            Assert.False(childGroup.TryDrop(childLink));
        }

        [Fact]
        public void DetachParent_Succeeds()
        {
            var reqs = Requirements.CreateIndependentOptionalRequirements(9);
            var children = new RequirementGroup[] {
                RequirementGroup.RequireAllOf(reqs[0..3], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[3..6], isOptional: true),
                RequirementGroup.RequireAllOf(reqs[6..9], isOptional: true),
            };
            var parentGroup = RequirementGroup.RequireOneOf(children, isOptional: true);

            var childGroup = children[0];
            var childToParent = childGroup.ParentGroupLink;
            childGroup.TryDetachParent(childToParent);
            Assert.True(
                childGroup.ParentGroupLink is null
                || !childGroup.ParentGroupLink.GetLinksOnPossibility().Any(l => l.Objective == parentGroup));
            childGroup.ReattachParent(childToParent);
            Assert.Contains(childToParent, childGroup.ParentGroupLink.GetLinksOnPossibility());
        }
    }
}
