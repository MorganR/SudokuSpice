using System;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    public class RequirementGroup :
        IObjective<RequirementGroup, Requirement>,
        IObjective<RequirementGroup, RequirementGroup>,
        IPossibility<RequirementGroup, RequirementGroup>
    {
        private readonly int _countRequired;
        private readonly bool _isOptional;

        private int _countPossible;
        private int _countSelected;

        internal bool IsSatisfied => _countSelected == _countRequired;
        internal bool AreAllPossibilitiesRequired => _countPossible == _countRequired;
        internal bool AreAllRequiredPossibilitiesSelected => _countRequired == _countSelected;

        internal Link<Requirement, RequirementGroup>? ChildRequirementLink { get; private set; }
        internal Link<RequirementGroup, RequirementGroup>? ChildGroupLink { get; private set; }
        internal Link<RequirementGroup, RequirementGroup>? ParentGroupLink { get; private set; }

        private RequirementGroup(int requiredCount, bool isOptional) : base()
        {
            _countRequired = requiredCount;
            _isOptional = isOptional;
        }

        public static RequirementGroup CreateFullyConnected(ReadOnlySpan<Requirement> requirements, int requiredCount, bool isOptional = false)
        {
            if (requirements.Length == 0)
            {
                throw new ArgumentException("Must provide at least 1 possibility to connect.");
            }
            if (requiredCount > requirements.Length)
            {
                throw new ArgumentException(
                    $"{nameof(requiredCount)} must be <= {nameof(requirements)}.length. Received {requirements.Length} requirements for a required count of {requiredCount}.");
            }
            var group = new RequirementGroup(requiredCount, isOptional);
            foreach (var toGroup in requirements)
            {
                Link<Requirement, RequirementGroup>.CreateConnectedLink(toGroup, group);
            }
            return group;
        }

        public static RequirementGroup CreateFullyConnected(ReadOnlySpan<RequirementGroup> groups, int requiredCount, bool isOptional = false)
        {
            if (groups.Length == 0)
            {
                throw new ArgumentException("Must provide at least 1 possibility to connect.");
            }
            if (requiredCount > groups.Length)
            {
                throw new ArgumentException(
                    $"{nameof(requiredCount)} must be <= {nameof(groups)}.length. Received {groups.Length} requirements for a required count of {requiredCount}.");
            }
            var group = new RequirementGroup(requiredCount, isOptional);
            foreach (var toGroup in groups)
            {
                Link<RequirementGroup, RequirementGroup>.CreateConnectedLink(toGroup, group);
            }
            return group;
        }

        public static RequirementGroup RequireAllOf(ReadOnlySpan<Requirement> requirements, bool isOptional = false)
        {
            return CreateFullyConnected(requirements, requirements.Length, isOptional);
        }

        public static RequirementGroup RequireAllOf(ReadOnlySpan<RequirementGroup> groups, bool isOptional = false)
        {
            return CreateFullyConnected(groups, groups.Length, isOptional);
        }

        public static RequirementGroup RequireOneOf(ReadOnlySpan<Requirement> requirements, bool isOptional = false)
        {
            return CreateFullyConnected(requirements, 1, isOptional);
        }

        public static RequirementGroup RequireOneOf(ReadOnlySpan<RequirementGroup> groups, bool isOptional = false)
        {
            return CreateFullyConnected(groups, 1, isOptional);
        }

        void IObjective<RequirementGroup, Requirement>.Append(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                ChildGroupLink is null,
                $"Cannot mix {nameof(ChildRequirementLink)} and {nameof(ChildGroupLink)}.");
            ++_countPossible;
            if (ChildRequirementLink is null)
            {
                ChildRequirementLink = link;
                return;
            }
            ChildRequirementLink.PrependToObjective(link);
        }

        void IObjective<RequirementGroup, RequirementGroup>.Append(Link<RequirementGroup, RequirementGroup> link)
        {
            Debug.Assert(
                ChildRequirementLink is null,
                $"Cannot mix {nameof(ChildRequirementLink)} and {nameof(ChildGroupLink)}.");
            ++_countPossible;
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

        #region TrySelectPossibility

        internal bool TrySelectPossibility(Link<Requirement, RequirementGroup> possibilityToThis)
        {
            Debug.Assert(
                !IsSatisfied,
                $"Tried to select link for already satisfied {nameof(RequirementGroup)}.");
            int maybeCountSelected = _countSelected + 1;
            // TODO: Try swapping the order of parent and child updates.
            if (maybeCountSelected == _countRequired)
            {
                if (!_TrySelectForParents())
                {
                    return false;
                }
                // Drop unselected possibilities.
                if (!Links.TryUpdateOthersOnObjective(
                    possibilityToThis,
                    toDrop => toDrop.Possibility.TryDrop(toDrop),
                    toReturn => toReturn.Possibility.Return(toReturn)))
                {
                    return false;
                }
            } else
            {
                // Still need to select more. Hide the selected link from our linked list.
                if (ChildRequirementLink == possibilityToThis)
                {
                    ChildRequirementLink = possibilityToThis.NextOnObjective;
                }
                possibilityToThis.PopFromObjective();
            }
            _countSelected = maybeCountSelected;
            return true;
        }

        internal bool TrySelectPossibility(Link<RequirementGroup, RequirementGroup> possibilityToThis)
        {
            Debug.Assert(
                !IsSatisfied,
                $"Tried to select link for already satisfied {nameof(RequirementGroup)}.");
            int maybeCountSelected = _countSelected + 1;
            if (maybeCountSelected == _countRequired)
            {
                // TODO: Try swapping the order of parent and child updates.
                if (!_TrySelectForParents())
                {
                    return false;
                }

                // Drop unselected possibilities.
                if (!Links.TryUpdateOthersOnObjective(
                    possibilityToThis,
                    toDrop => toDrop.Possibility.TryDrop(toDrop),
                    toReturn => toReturn.Possibility.Return(toReturn)))
                {
                    return false;
                }
            } else
            {
                // Still need to select more. Hide the selected link from our linked list.
                if (ChildGroupLink == possibilityToThis)
                {
                    ChildGroupLink = possibilityToThis.NextOnObjective;
                }
                possibilityToThis.PopFromObjective();
            }
            _countSelected = maybeCountSelected;
            return true;
        }

        #endregion

        #region DeselectPossibility

        internal void DeselectPossibility(Link<Requirement, RequirementGroup> possibilityToThis)
        {
            if (IsSatisfied)
            {
                Links.RevertOthersOnObjective(
                    possibilityToThis,
                    toReturn => toReturn.Possibility.Return(toReturn));
                _DeselectForParents();
            } else
            {
                possibilityToThis.ReinsertToObjective();
            }
            --_countSelected;
            return;
        }

        internal void DeselectPossibility(Link<RequirementGroup, RequirementGroup> possibilityToThis)
        {
            if (IsSatisfied)
            {
                Links.RevertOthersOnObjective(
                    possibilityToThis,
                    toReturn => toReturn.Possibility.Return(toReturn));
                _DeselectForParents();
            } else
            {
                possibilityToThis.ReinsertToObjective();
            }
            --_countSelected;
            return;
        }

        #endregion

        #region TryDropPossibility

        internal bool TryDropPossibility(Link<Requirement, RequirementGroup> possibilityToThis)
        {
            Debug.Assert(
                ChildRequirementLink?.GetLinksOnObjective().Contains(possibilityToThis) ?? false,
                $"Can only call {nameof(TryDropPossibility)} on a link that is connected to this group.");
            Debug.Assert(
                !possibilityToThis.Possibility.AreAllRequiredPossibilitiesSelected,
                "Can't drop a satisfied possibility.");
            Debug.Assert(
                _countPossible >= _countRequired,
                "Can't drop a possibility from an already impossible group.");
            if (AreAllPossibilitiesRequired) {
                if (!_isOptional)
                {
                    return false;
                }
                if (!_TryDropFromParents())
                {
                    return false;
                }
                // This group is no longer possible. This group should be dropped from all
                // remaining possiblities. We can skip the given link since it's already being
                // dropped.
                Links.UpdateOthersOnObjective(
                    possibilityToThis,
                    toDrop => toDrop.Possibility.DetachGroup(toDrop));
            }
            if (ChildRequirementLink == possibilityToThis)
            {
                ChildRequirementLink = possibilityToThis.NextOnObjective;
                if (ChildRequirementLink == possibilityToThis)
                {
                    ChildRequirementLink = null;
                }
            }
            possibilityToThis.PopFromObjective();
            --_countPossible;
            // TODO: Force children to be required if necessary?
            return true;
        }

        internal bool TryDropPossibility(Link<RequirementGroup, RequirementGroup> possibilityToThis)
        {
            Debug.Assert(
                ChildGroupLink?.GetLinksOnObjective().Contains(possibilityToThis) ?? false,
                $"Can only call {nameof(TryDropPossibility)} on a link that is connected to this group.");
            Debug.Assert(
                !possibilityToThis.Possibility.AreAllRequiredPossibilitiesSelected,
                "Can't drop a satisfied possibility.");
            Debug.Assert(
                _countPossible >= _countRequired,
                "Can't drop a possibility from an already impossible group.");
            if (AreAllPossibilitiesRequired) {
                if (!_isOptional)
                {
                    return false;
                }
                if (!_TryDropFromParents())
                {
                    return false;
                }
                // This group is no longer possible. This group should be dropped from all
                // remaining possiblities. We can skip the given link since it's already being
                // dropped.
                Links.UpdateOthersOnObjective(
                    possibilityToThis,
                    toDetach => toDetach.Possibility.DetachParent(toDetach));
            }
            if (ChildGroupLink == possibilityToThis)
            {
                ChildGroupLink = possibilityToThis.NextOnObjective;
                if (ChildGroupLink == possibilityToThis)
                {
                    ChildGroupLink = null;
                }
            }
            possibilityToThis.PopFromObjective();
            --_countPossible;
            // TODO: Force children to be required if necessary?
            return true;
        }

        #endregion

        #region ReturnPossibility

        internal void ReturnPossibility(Link<Requirement, RequirementGroup> possibilityToThis)
        {
            ++_countPossible;
            possibilityToThis.ReinsertToObjective();
            if (ChildRequirementLink is null)
            {
                ChildRequirementLink = possibilityToThis;
            }
            if (AreAllPossibilitiesRequired)
            {
                Debug.Assert(
                    _isOptional,
                    "Shouldn't be possible to try to return a required possibility to a non-optional group.");
                Links.RevertOthersOnObjective(
                    possibilityToThis,
                    toReattach => toReattach.Possibility.ReattachGroup(toReattach));
                _ReturnToParents();
                return;
            }
        }

        internal void ReturnPossibility(Link<RequirementGroup, RequirementGroup> possibilityToThis)
        {
            ++_countPossible;
            possibilityToThis.ReinsertToObjective();
            if (ChildGroupLink is null)
            {
                ChildGroupLink = possibilityToThis;
            }
            if (AreAllPossibilitiesRequired)
            {
                Debug.Assert(_isOptional,
                    "Shouldn't be possible to try to return a required possibility to a non-optional group.");
                // Return this objective to possibilities.
                Links.RevertOthersOnObjective(
                    possibilityToThis,
                    toReattach => toReattach.Possibility.ReattachParent(toReattach));
                _ReturnToParents();
            }
        }

        #endregion

        internal bool TryDrop(Link<RequirementGroup, RequirementGroup> thisToParent)
        {
            Debug.Assert(
                !IsSatisfied,
                "Can't drop an already satisfied group.");
            if (!_isOptional)
            {
                return false;
            }

            if (!Links.TryUpdateOthersOnPossibility(
                thisToParent,
                toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
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

        internal void Return(Link<RequirementGroup, RequirementGroup> parentLink)
        {
            Debug.Assert(
                _isOptional,
                "Can't call return on a non-optional group.");
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
            Links.RevertOthersOnPossibility(
                parentLink,
                toReturn => toReturn.Objective.ReturnPossibility(toReturn));
        }

        internal void DetachParent(Link<RequirementGroup, RequirementGroup> thisToParent)
        {
            Debug.Assert(
                ParentGroupLink is not null,
                $"{nameof(Requirement)} has no parent to detach.");
            if (ParentGroupLink == thisToParent)
            {
                ParentGroupLink = thisToParent.NextOnPossibility;
                if (ParentGroupLink == thisToParent)
                {
                    ParentGroupLink = null;
                }
            }
            thisToParent.PopFromPossibility();
        }

        internal void ReattachParent(Link<RequirementGroup, RequirementGroup> thisToParent)
        {
            thisToParent.ReinsertToPossibility();
            if (ParentGroupLink is null)
            {
                ParentGroupLink = thisToParent;
            }
        }

        private bool _TrySelectForParents()
        {
            if (ParentGroupLink is not null)
            {
                // Try selecting this possibility on all its parents.
                if (!Links.TryUpdateOnPossibility(
                    ParentGroupLink,
                    toSelect => toSelect.Objective.TrySelectPossibility(toSelect),
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect)))
                {
                    return false;
                }
            }
            return true;
        }

        private void _DeselectForParents()
        {
            if (ParentGroupLink is not null)
            {
                Links.RevertOnPossibility(
                    ParentGroupLink,
                    toDeselect => toDeselect.Objective.DeselectPossibility(toDeselect));
            }
        }

        private bool _TryDropFromParents()
        {
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

        internal void _ReturnToParents()
        {
            if (ParentGroupLink is not null)
            {
                Links.RevertOnPossibility(
                    ParentGroupLink,
                    toReattach => toReattach.Objective.ReturnPossibility(toReattach));
            }
        }
    }
}
