using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased
{
    internal class Link<TPossibility, TObjective>
        where TPossibility : class, IPossibility<TPossibility, TObjective>
        where TObjective : class, IObjective<TObjective, TPossibility>
    {
        internal readonly TPossibility Possibility;
        internal readonly TObjective Objective;

        internal Link<TPossibility, TObjective> PreviousOnPossibility { get; private set; }
        internal Link<TPossibility, TObjective> NextOnPossibility { get; private set; }
        internal Link<TPossibility, TObjective> PreviousOnObjective { get; private set; }
        internal Link<TPossibility, TObjective> NextOnObjective { get; private set; }

        private Link(TPossibility possibility, TObjective objective)
        {
            this.Possibility = possibility;
            Objective = objective;
            PreviousOnObjective = NextOnObjective = NextOnPossibility = PreviousOnPossibility = this;
        }

        internal static Link<TPossibility, TObjective> CreateConnectedLink(TPossibility possibility, TObjective objective)
        {
            var link = new Link<TPossibility, TObjective>(possibility, objective);
            possibility.Append(link);
            objective.Append(link);
            return link;
        }

        internal void AppendToPossibility(Link<TPossibility, TObjective> toAppend)
        {
            toAppend.NextOnPossibility = NextOnPossibility;
            toAppend.PreviousOnPossibility = this;
            NextOnPossibility.PreviousOnPossibility = toAppend;
            NextOnPossibility = toAppend;
        }

        internal void AppendToObjective(Link<TPossibility, TObjective> toAppend)
        {
            toAppend.NextOnObjective = NextOnObjective;
            toAppend.PreviousOnObjective = this;
            NextOnObjective.PreviousOnObjective = toAppend;
            NextOnObjective = toAppend;
        }

        internal void PrependToPossibility(Link<TPossibility, TObjective> toPrepend)
        {
            toPrepend.NextOnPossibility = this;
            toPrepend.PreviousOnPossibility = PreviousOnPossibility;
            PreviousOnPossibility.NextOnPossibility = toPrepend;
            PreviousOnPossibility = toPrepend;
        }

        internal void PrependToObjective(Link<TPossibility, TObjective> toPrepend)
        {
            toPrepend.NextOnObjective = this;
            toPrepend.PreviousOnObjective = PreviousOnObjective;
            PreviousOnObjective.NextOnObjective = toPrepend;
            PreviousOnObjective = toPrepend;
        }

        internal void PopFromObjective()
        {
            PreviousOnObjective.NextOnObjective = NextOnObjective;
            NextOnObjective.PreviousOnObjective = PreviousOnObjective;
        }

        internal void ReinsertToObjective()
        {
            PreviousOnObjective.NextOnObjective = this;
            NextOnObjective.PreviousOnObjective = this;
        }

        internal void PopFromPossibility()
        {
            PreviousOnPossibility.NextOnPossibility = NextOnPossibility;
            NextOnPossibility.PreviousOnPossibility = PreviousOnPossibility;
        }

        internal void ReinsertToPossibility()
        {
            PreviousOnPossibility.NextOnPossibility = this;
            NextOnPossibility.PreviousOnPossibility = this;
        }

        internal IEnumerable<Link<TPossibility, TObjective>> GetLinksOnPossibility()
        {
            var link = this;
            do
            {
                yield return link;
                link = link.NextOnPossibility;
            } while (link != this);
        }

        internal IEnumerable<Link<TPossibility, TObjective>> GetLinksOnObjective()
        {
            var link = this;
            do
            {
                yield return link;
                link = link.NextOnObjective;
            } while (link != this);
        }
    }
}
