using System;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    public class AndGroup : RequirementGroup
    {
        private readonly bool _isOptional;

        private int _countSelected;
        private int _countDropped;

        internal bool IsSatisfied => PossibilityCount == _countSelected;

        private AndGroup(bool isOptional) : base()
        {
            _isOptional = isOptional;
        }

        public static AndGroup CreateFullyConnected(ReadOnlySpan<Requirement> requirements, bool isOptional = false)
        {
            var and = new AndGroup(isOptional);
            foreach (var toAnd in requirements)
            {
                Link<Requirement, RequirementGroup>.CreateConnectedLink(toAnd, and);
            }
            return and;
        }

        public static AndGroup CreateFullyConnected(ReadOnlySpan<RequirementGroup> groups, bool isOptional = false)
        {
            var and = new AndGroup(isOptional);
            foreach (var toAnd in groups)
            {
                Link<RequirementGroup, RequirementGroup>.CreateConnectedLink(toAnd, and);
            }
            return and;
        }

        #region ChildRequirements

        internal override bool TrySelectPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            return _TrySelect();
        }

        internal override void DeselectPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            _Deselect();
        }

        internal override bool TryDropPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildRequirementLink?.GetLinksOnObjective().Contains(link) ?? false,
                $"Can only call {nameof(TryDropPossibility)} on a link that is connected to this group.");
            return _TryDetach();
        }

        internal override void ReturnPossibility(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                "Cannot mix child groups with child requirements.");
            _Reattach();
        }

        #endregion

        #region ChildGroups

        internal override bool TrySelectPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            return _TrySelect();
        }

        internal override void DeselectPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            _Deselect();
        }

        internal override bool TryDropPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            Debug.Assert(
                ChildGroupLink?.GetLinksOnPossibility().Contains(link) ?? false,
                $"Can only call {nameof(TryDropPossibility)} on a link that is connected to this group.");
            return _TryDetach();
        }

        internal override void ReturnPossibility(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                "Cannot mix child groups with child requirements.");
            _Reattach();
        }

        #endregion

        private bool _TrySelect()
        {
            Debug.Assert(
                !IsSatisfied,
                $"Tried to select link for already satisfied {nameof(AndGroup)}.");
            ++_countSelected;
            if (IsSatisfied && ParentGroupLink is not null)
            {
                if (!Links.TryUpdateOnPossibility(
                    ParentGroupLink,
                    toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
                {
                    --_countSelected;
                    return false;
                }
            }
            return true;
        }

        private void _Deselect()
        {
            if (IsSatisfied && ParentGroupLink is not null)
            {
                Links.RevertOnPossibility(
                    ParentGroupLink,
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
            }
            --_countSelected;
        }

        private bool _TryDetach()
        {
            Debug.Assert(
                !IsSatisfied,
                $"Tried to drop requirement from already satisfied {nameof(AndGroup)}.");
            if (!_isOptional)
            {
                return false;
            }

            // Do nothing if this was already dropped from parents.
            if (_countDropped++ > 1)
            {
                return true;
            }

            // Drop this group as a possibility from all parent groups.
            if (ParentGroupLink is not null)
            {
                if (!Links.TryUpdateOnPossibility(
                    ParentGroupLink,
                    toDetach => toDetach.Objective.TryDropPossibility(toDetach),
                    toReattach => toReattach.Objective.ReturnPossibility(toReattach)))
                {
                    return false;
                }
            }
            return true;
        }

        private void _Reattach()
        {
            if (!_isOptional)
            {
                throw new InvalidOperationException($"Can't call reattach on {nameof(AndGroup)}.");
            }
            // Skip reattaching to parents if this is still impossible.
            if (--_countDropped > 0)
            {
                return;
            }
            // Reattach this group as a possibility on parent groups.
            if (ParentGroupLink is not null)
            {
                Links.RevertOnPossibility(
                    ParentGroupLink,
                    toReattach => toReattach.Objective.ReturnPossibility(toReattach));
            }
        }

        internal override bool TryDrop(Link<RequirementGroup, RequirementGroup> sourceLink)
        {
            if (!_isOptional)
            {
                return false;
            }

            if (ChildRequirementLink is not null)
            {
                Links.UpdateOnObjective(
                    ChildRequirementLink,
                    toDetach => toDetach.Possibility.DetachGroup(toDetach));
            } else if (ChildGroupLink is not null)
            {
                Links.UpdateOnObjective(
                    ChildGroupLink,
                    toDetach => toDetach.Possibility.DetachParent(toDetach));
            }
            return true;
        }

        internal override void Return(Link<RequirementGroup, RequirementGroup> sourceLink)
        {
            if (!_isOptional)
            {
                throw new InvalidOperationException($"Cannot call {nameof(Return)} on {nameof(AndGroup)}.");
            }
            if (ChildRequirementLink is not null)
            {
                Links.RevertOnObjective(
                    ChildRequirementLink,
                    toReattach => toReattach.Possibility.ReattachGroup(toReattach));
            } else if (ChildGroupLink is not null)
            {
                Links.RevertOnObjective(
                    ChildGroupLink,
                    toReattach => toReattach.Possibility.ReattachParent(toReattach));
            }
        }
    }
}
