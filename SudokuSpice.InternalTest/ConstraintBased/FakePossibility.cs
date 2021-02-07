using System;
using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    internal class FakePossibility : IPossibility
    {
        internal bool CanBeDetached = true;
        internal List<Link> AttachedObjectives = new List<Link>();
        internal List<Link> DetachedObjectives = new List<Link>();

        public NodeState State { get; set; }

        void IPossibility.AppendObjective(Link toNewObjective)
        {
            AttachedObjectives.Add(toNewObjective);
            AttachedObjectives[0].PrependToPossibility(toNewObjective);
        }

        void IPossibility.NotifyReattachedToObjective(Link toReattach)
        {
            if (!DetachedObjectives.Contains(toReattach))
            {
                throw new InvalidOperationException("Can't reattach an objective that is not detached.");
            }
            DetachedObjectives.Remove(toReattach);
        }

        bool IPossibility.TryNotifyDroppedFromObjective(Link toDetach)
        {
            if (!AttachedObjectives.Contains(toDetach))
            {
                throw new InvalidOperationException("Can't detach an objective that was never attached.");
            }
            if (DetachedObjectives.Contains(toDetach))
            {
                throw new InvalidOperationException("Can't re-detach an objective that was already detached.");
            }
            if (!CanBeDetached)
            {
                return false;
            }
            DetachedObjectives.Add(toDetach);
            return true;
        }
    }
}
