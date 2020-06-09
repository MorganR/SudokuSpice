namespace SudokuSpice.Data
{
    internal class SquareLink
    {
        internal readonly PossibleSquareValue PossibleSquare;
        internal readonly ConstraintHeader Constraint;
        internal SquareLink Left { get; set; }
        internal SquareLink Right { get; set; }
        internal SquareLink Up { get; set; }
        internal SquareLink Down { get; set; }

        private SquareLink(PossibleSquareValue possibleSquare, ConstraintHeader constraint)
        {
            PossibleSquare = possibleSquare;
            Constraint = constraint;
            Up = Down = Right = Left = this;
        }

        internal static SquareLink CreateConnectedLink(PossibleSquareValue possibleSquare, ConstraintHeader header)
        {
            var squareLink = new SquareLink(possibleSquare, header);
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

        internal bool TrySatisfyConstraint()
        {
            return Constraint.TrySatisfyFrom(this);
        }

        internal void UnsatisfyConstraint()
        {
            Constraint.UnsatisfyFrom(this);
        }
    }
}
