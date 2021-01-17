using System.Diagnostics;

namespace SudokuSpice.ConstraintBased
{
    public abstract class RequirementGroup :
        IObjective<RequirementGroup, Requirement>,
        IObjective<RequirementGroup, RequirementGroup>,
        IPossibility<RequirementGroup, RequirementGroup>
    {
        private protected Link<Requirement, RequirementGroup>? ChildRequirementLink { get; set; }
        private protected Link<RequirementGroup, RequirementGroup>? ChildGroupLink { get; set; }
        private protected Link<RequirementGroup, RequirementGroup>? ParentGroupLink { get; set; }

        private protected int PossibilityCount { get; set; }

        void IObjective<RequirementGroup, Requirement>.Append(Link<Requirement, RequirementGroup> link)
        {
            ++PossibilityCount;
            if (ChildRequirementLink is null)
            {
                ChildRequirementLink = link;
                return;
            }
            ChildRequirementLink.PrependToObjective(link);
        }

        void IObjective<RequirementGroup, RequirementGroup>.Append(Link<RequirementGroup, RequirementGroup> link)
        {
            ++PossibilityCount;
            if (ChildGroupLink is null)
            {
                ChildGroupLink = link;
                return;
            }
            ChildGroupLink.PrependToObjective(link);
        }

        void IPossibility<RequirementGroup, RequirementGroup>.Append(Link<RequirementGroup, RequirementGroup> link)
        {
            if (ParentGroupLink is null)
            {
                ParentGroupLink = link;
                return;
            }
            ParentGroupLink.PrependToPossibility(link);
        }

        internal abstract bool TrySelectPossibility(Link<Requirement, RequirementGroup> link);
        internal abstract void DeselectPossibility(Link<Requirement, RequirementGroup> link);
        internal abstract bool TryDropPossibility(Link<Requirement, RequirementGroup> link);
        internal abstract void ReturnPossibility(Link<Requirement, RequirementGroup> link);
        internal abstract bool TrySelectPossibility(Link<RequirementGroup, RequirementGroup> link);
        internal abstract void DeselectPossibility(Link<RequirementGroup, RequirementGroup> link);
        internal abstract bool TryDropPossibility(Link<RequirementGroup, RequirementGroup> link);
        internal abstract void ReturnPossibility(Link<RequirementGroup, RequirementGroup> link);
        internal abstract bool TryDrop(Link<RequirementGroup, RequirementGroup> sourceLink);
        internal abstract void Return(Link<RequirementGroup, RequirementGroup> sourceLink);

        internal void DetachParent(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ParentGroupLink is not null,
                $"{nameof(Requirement)} has no parent to detach.");
            if (ParentGroupLink == link)
            {
                ParentGroupLink = link.NextOnPossibility;
                if (ParentGroupLink == link)
                {
                    ParentGroupLink = null;
                }
            }
            link.PopFromPossibility();
        }

        internal void ReattachParent(Link<RequirementGroup, RequirementGroup> link)
        {
            link.ReinsertToPossibility();
            if (ParentGroupLink is null)
            {
                ParentGroupLink = link;
            }
        }
    }
}
