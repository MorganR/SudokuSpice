namespace SudokuSpice.ConstraintBased
{
    internal class Link
    {
        internal readonly PossibleSquareValue PossibleSquareValue;
        internal readonly Requirement Requirement;
        internal Link Left { get; private set; }
        internal Link Right { get; private set; }
        internal Link Up { get; private set; }
        internal Link Down { get; private set; }

        private Link(PossibleSquareValue possibleValue, Requirement requirement)
        {
            this.PossibleSquareValue = possibleValue;
            Requirement = requirement;
            Up = Down = Right = Left = this;
        }

        internal static Link CreateConnectedLink(PossibleSquareValue possibleValue, Requirement requirement)
        {
            var link = new Link(possibleValue, requirement);
            possibleValue.Attach(link);
            requirement.Attach(link);
            return link;
        }

        internal bool TryRemoveFromRequirement()
        {
            // If the requirement is already satisfied then we can skip this.
            if (Requirement.AreAllLinksSelected)
            {
                return true;
            }
            return Requirement.TryDetach(this);
        }

        internal void ReturnToRequirement()
        {
            // If the constraint is satisfied then we can skip this since this link was never removed.
            if (Requirement.AreAllLinksSelected)
            {
                return;
            }
            Requirement.Reattach(this);
        }

        internal bool TrySelectForRequirement() => Requirement.TrySelect(this);

        internal void DeselectFromRequirement() => Requirement.Deselect(this);

        internal void AppendRight(Link toAppend)
        {
            toAppend.Right = Right;
            toAppend.Left = this;
            Right.Left = toAppend;
            Right = toAppend;
        }

        internal void AppendDown(Link toAppend)
        {
            toAppend.Down = Down;
            toAppend.Up = this;
            Down.Up = toAppend;
            Down = toAppend;
        }

        internal void PrependLeft(Link toPrepend)
        {
            toPrepend.Right = this;
            toPrepend.Left = Left;
            Left.Right = toPrepend;
            Left = toPrepend;
        }

        internal void PrependUp(Link toPrepend)
        {
            toPrepend.Down = this;
            toPrepend.Up = Up;
            Up.Down = toPrepend;
            Up = toPrepend;
        }

        internal void PopVertically()
        {
            Up.Down = Down;
            Down.Up = Up;
        }

        internal void ReinsertVertically()
        {
            Up.Down = this;
            Down.Up = this;
        }
    }
}