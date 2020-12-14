namespace SudokuSpice.ConstraintBased
{
    internal class Link
    {
        internal readonly PossibleValue PossibleSquare;
        internal readonly ConstraintHeader Constraint;
        internal Link Left { get; set; }
        internal Link Right { get; set; }
        internal Link Up { get; set; }
        internal Link Down { get; set; }

        private Link(PossibleValue possibleSquare, ConstraintHeader constraint)
        {
            PossibleSquare = possibleSquare;
            Constraint = constraint;
            Up = Down = Right = Left = this;
        }

        internal static Link CreateConnectedLink(PossibleValue possibleSquare, ConstraintHeader header)
        {
            var squareLink = new Link(possibleSquare, header);
            possibleSquare.Attach(squareLink);
            header.Attach(squareLink);
            return squareLink;
        }

        internal bool TryRemoveFromConstraint()
        {
            // If the constraint is already satisfied then we can skip this.
            if (Constraint.IsSatisfied)
            {
                return true;
            }
            return Constraint.TryDetach(this);
        }

        internal void ReturnToConstraint()
        {
            // If the constraint is satisfied then we can skip this since this link was never removed.
            if (Constraint.IsSatisfied)
            {
                return;
            }
            Constraint.Reattach(this);
        }

        internal bool TrySatisfyConstraint() => Constraint.TrySatisfyFrom(this);

        internal void UnsatisfyConstraint() => Constraint.UnsatisfyFrom(this);
    }
}