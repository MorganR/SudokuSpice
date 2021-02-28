using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SudokuSpice.ConstraintBased
{
    internal class Link
    {
        internal readonly IPossibility Possibility;
        internal readonly IObjective Objective;

        internal Link PreviousOnPossibility { get; private set; }
        internal Link NextOnPossibility { get; private set; }
        internal Link PreviousOnObjective { get; private set; }
        internal Link NextOnObjective { get; private set; }

        private Link(IPossibility possibility, IObjective objective)
        {
            Possibility = possibility;
            Objective = objective;
            PreviousOnObjective = NextOnObjective = NextOnPossibility = PreviousOnPossibility = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Link CreateConnectedLink(IPossibility possibility, IObjective objective)
        {
            var link = new Link(possibility, objective);
            possibility.AppendObjective(link);
            objective.AppendPossibility(link);
            return link;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AppendToPossibility(Link toAppend)
        {
            toAppend.NextOnPossibility = NextOnPossibility;
            toAppend.PreviousOnPossibility = this;
            NextOnPossibility.PreviousOnPossibility = toAppend;
            NextOnPossibility = toAppend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AppendToObjective(Link toAppend)
        {
            toAppend.NextOnObjective = NextOnObjective;
            toAppend.PreviousOnObjective = this;
            NextOnObjective.PreviousOnObjective = toAppend;
            NextOnObjective = toAppend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PrependToPossibility(Link toPrepend)
        {
            toPrepend.NextOnPossibility = this;
            toPrepend.PreviousOnPossibility = PreviousOnPossibility;
            PreviousOnPossibility.NextOnPossibility = toPrepend;
            PreviousOnPossibility = toPrepend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PrependToObjective(Link toPrepend)
        {
            toPrepend.NextOnObjective = this;
            toPrepend.PreviousOnObjective = PreviousOnObjective;
            PreviousOnObjective.NextOnObjective = toPrepend;
            PreviousOnObjective = toPrepend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PopFromObjective()
        {
            PreviousOnObjective.NextOnObjective = NextOnObjective;
            NextOnObjective.PreviousOnObjective = PreviousOnObjective;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReinsertToObjective()
        {
            PreviousOnObjective.NextOnObjective = this;
            NextOnObjective.PreviousOnObjective = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PopFromPossibility()
        {
            PreviousOnPossibility.NextOnPossibility = NextOnPossibility;
            NextOnPossibility.PreviousOnPossibility = PreviousOnPossibility;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReinsertToPossibility()
        {
            PreviousOnPossibility.NextOnPossibility = this;
            NextOnPossibility.PreviousOnPossibility = this;
        }

        internal IEnumerable<Link> GetLinksOnPossibility()
        {
            var link = this;
            do
            {
                yield return link;
                link = link.NextOnPossibility;
            } while (link != this);
        }

        internal IEnumerable<Link> GetLinksOnObjective()
        {
            var link = this;
            do
            {
                yield return link;
                link = link.NextOnObjective;
            } while (link != this);
        }

        internal IEnumerable<IPossibility> GetPossibilitiesOnObjective()
        {
            var link = this;
            do
            {
                yield return link.Possibility;
                link = link.NextOnObjective;
            } while (link != this);
        }
    }
}
