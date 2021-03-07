using System;
using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    internal class FakePossibility : IPossibility
    {
        internal bool CanBeDetached = true;
        internal List<Link> AttachedObjectives = new List<Link>();
        internal List<Link> DroppedFromObjectives = new List<Link>();

        internal FakePossibility(bool isConcrete = true)
        {
            IsConcrete = isConcrete;
        }

        public bool IsConcrete { get; set; }

        public NodeState State { get; set; }

        void IPossibility.AppendObjective(Link toNewObjective)
        {
            AttachedObjectives.Add(toNewObjective);
            AttachedObjectives[0].PrependToPossibility(toNewObjective);
        }

        void IPossibility.ReturnFromObjective(Link toReattach)
        {
            if (!DroppedFromObjectives.Contains(toReattach))
            {
                throw new InvalidOperationException("Can't reattach an objective that is not detached.");
            }
            DroppedFromObjectives.Remove(toReattach);
        }

        bool IPossibility.TryDropFromObjective(Link toDetach)
        {
            if (!AttachedObjectives.Contains(toDetach))
            {
                throw new InvalidOperationException("Can't detach an objective that was never attached.");
            }
            if (DroppedFromObjectives.Contains(toDetach))
            {
                throw new InvalidOperationException("Can't re-detach an objective that was already detached.");
            }
            if (!CanBeDetached)
            {
                return false;
            }
            DroppedFromObjectives.Add(toDetach);
            return true;
        }
    }
}