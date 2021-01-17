using System;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    class OrGroup : RequirementGroup
    {
        private bool _isSatisfied;

        public static OrGroup CreateFullyConnected(ReadOnlySpan<Requirement> requirements)
        {
            var or = new OrGroup();
            foreach (Requirement toOr in requirements)
            {
                Link<Requirement, RequirementGroup>.CreateConnectedLink(toOr, or);
            }
            return or;
        }

        public static OrGroup CreateFullyConnected(ReadOnlySpan<RequirementGroup> groups)
        {
            var or = new OrGroup();
            foreach (RequirementGroup toOr in groups)
            {
                Link<RequirementGroup, RequirementGroup>.CreateConnectedLink(toOr, or);
            }
            return or;
        }

        #region ChildRequirements

        internal override bool TrySelectPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildRequirementLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TrySelectPossibility)} on a link that is connected to this group.");
            Debug.Assert(!_isSatisfied, "Can't select new link in already satisfied requirement.");
            // Try to drop the other possibilities from this objective.
            if (!Links.TryUpdateOthersOnObjective(
                link,
                toDrop => toDrop.Possibility.TryDrop(toDrop),
                toReturn => toReturn.Possibility.Return(toReturn)))
            {
                return false;
            }
            // Try to select this group in any parent groups.
            if (!_TrySelectOnParents())
            {
                return false;
            }
            _isSatisfied = true;
            return true;
        }

        internal override void DeselectPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildRequirementLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(DeselectPossibility)} on a link that is connected to this group.");
            Debug.Assert(_isSatisfied, "Can't deselect link that wasn't selected.");
            _isSatisfied = false;
            _DeselectFromParents();
            Links.RevertOthersOnObjective(
                link,
                toReturn => toReturn.Possibility.Return(toReturn));
        }

        internal override bool TryDropPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildRequirementLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TryDropPossibility)} on a link that is connected to this group.");
            Debug.Assert(
                PossibilityCount > 0,
                $"{nameof(TryDropPossibility)} called with {nameof(PossibilityCount)} less than 1: {PossibilityCount}");
            if (PossibilityCount == 1)
            {
                return false;
            }
            --PossibilityCount;
            if (ChildRequirementLink == link)
            {
                ChildRequirementLink = link.NextOnObjective;
            }
            link.PopFromObjective();
            return true;
        }

        internal override void ReturnPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                !ChildRequirementLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(ReturnPossibility)} on a link that is not currently connected to this group.");
            link.ReinsertToObjective();
            ++PossibilityCount;
        }

        #endregion

        #region ChildGroups

        internal override bool TrySelectPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildGroupLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TrySelectPossibility)} on a link that is connected to this group.");
            Debug.Assert(!_isSatisfied, "Can't select new link in already satisfied requirement.");
            // Try to drop the other possibilities from this objective.
            if (!Links.TryUpdateOthersOnObjective(
                link,
                toDrop => toDrop.Possibility.TryDrop(toDrop),
                toReturn => toReturn.Possibility.Return(toReturn)))
            {
                return false;
            }
            // Try to select this group in any parent groups.
            if (!_TrySelectOnParents())
            {
                return false;
            }
            _isSatisfied = true;
            return true;
        }

        internal override void DeselectPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildGroupLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TrySelectPossibility)} on a link that is connected to this group.");
            Debug.Assert(_isSatisfied, "Can't deselect link that wasn't selected.");
            _isSatisfied = false;
            _DeselectFromParents();
            Links.RevertOthersOnObjective(
                link,
                toReturn => toReturn.Possibility.Return(toReturn));
        }

        internal override bool TryDropPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildGroupLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TrySelectPossibility)} on a link that is connected to this group.");
            Debug.Assert(
                PossibilityCount > 0,
                $"{nameof(TryDropPossibility)} called with {nameof(PossibilityCount)} less than 1: {PossibilityCount}");
            if (PossibilityCount == 1)
            {
                return false;
            }
            --PossibilityCount;
            if (ChildGroupLink == link)
            {
                ChildGroupLink = link.NextOnObjective;
            }
            link.PopFromObjective();
            return true;
        }

        internal override void ReturnPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                !ChildGroupLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(ReturnPossibility)} on a link that is not currently connected to this group.");
            link.ReinsertToObjective();
            ++PossibilityCount;
        }

        #endregion

        internal override bool TryDrop(Link<RequirementGroup, RequirementGroup> sourceLink) => false;

        internal override void Return(Link<RequirementGroup, RequirementGroup> sourceLink) => 
            throw new InvalidOperationException($"Can't call {nameof(Return)} on {nameof(OrGroup)}.");

        private bool _TrySelectOnParents()
        {
            if (ParentGroupLink is null)
            {
                return true;
            }
            return Links.TryUpdateOnPossibility(
                ParentGroupLink,
                toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
        }

        private void _DeselectFromParents()
        {
            if (ParentGroupLink is not null)
            {
                Links.RevertOthersOnPossibility(
                    ParentGroupLink,
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
            }
        }
    }
}
