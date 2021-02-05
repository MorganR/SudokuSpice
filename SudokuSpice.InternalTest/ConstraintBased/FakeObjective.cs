﻿using System;
using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    internal class FakeObjective : IObjective
    {
        private readonly bool _isRequired;
        private HashSet<IObjective>? _requiredObjectives;
        internal bool CanDropPossibilities = true;
        internal bool CanSelectPossibilities = true;
        internal List<Link> AttachedPossibilities = new();
        internal List<Link> DroppedPossibilities = new();
        internal List<Link> SelectedPossibilities = new();

        internal FakeObjective(bool isRequired = false)
        {
            _isRequired = isRequired;
            if (_isRequired)
            {
                _requiredObjectives = new HashSet<IObjective> { this };
            }
        }

        internal void SetRequiredObjectives(IEnumerable<IObjective> objectives)
        {
            if (_isRequired)
            {
                throw new InvalidOperationException("Can't set required objectives on a required objective.");
            }
            _requiredObjectives = new HashSet<IObjective>(objectives); 
        }

        bool IObjective.IsRequired => _isRequired;

        IReadOnlySet<IObjective> IObjective.RequiredObjectives
        {
            get {
                if (_requiredObjectives is null)
                {
                    throw new InvalidOperationException("Must stub the required objectives before retrieving them.");
                }
                return _requiredObjectives;
            }
        }

        void IObjective.AppendPossibility(Link toNewPossibility)
        {
            AttachedPossibilities.Add(toNewPossibility);
            AttachedPossibilities[0].PrependToObjective(toNewPossibility);
        }

        void IObjective.ReturnPossibility(Link toReturn)
        {
            if (!DroppedPossibilities.Contains(toReturn))
            {
                throw new InvalidOperationException("Can't return a possibility that is not dropped.");
            }
            DroppedPossibilities.Remove(toReturn);
        }

        bool IObjective.TryDropPossibility(Link toDrop)
        {
            if (!AttachedPossibilities.Contains(toDrop))
            {
                throw new InvalidOperationException("Can't drop a possibility that was never attached.");
            }
            if (DroppedPossibilities.Contains(toDrop))
            {
                throw new InvalidOperationException("Can't re-drop a possibility that was already dropped.");
            }
            if (!CanDropPossibilities)
            {
                return false;
            }
            DroppedPossibilities.Add(toDrop);
            return true;
        }

        void IObjective.DeselectPossibility(Link toDeselect)
        {
            if (!SelectedPossibilities.Contains(toDeselect))
            {
                throw new InvalidOperationException("Can't deselect a possibility that is not selected.");
            }
            SelectedPossibilities.Remove(toDeselect);
        }

        bool IObjective.TrySelectPossibility(Link toSelect)
        {
            if (!AttachedPossibilities.Contains(toSelect))
            {
                throw new InvalidOperationException("Can't select a possibility that was never attached.");
            }
            if (SelectedPossibilities.Contains(toSelect))
            {
                throw new InvalidOperationException("Can't re-select a possibility that was already selected.");
            }
            if (!CanSelectPossibilities)
            {
                return false;
            }
            SelectedPossibilities.Add(toSelect);
            return true;
        }

        IEnumerable<IPossibility> IObjective.GetUnknownDirectPossibilities() => throw new NotImplementedException();
    }
}
