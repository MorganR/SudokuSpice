namespace SudokuSpice.ConstraintBased
{
    internal class SquareLink<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        internal readonly PossibleSquareValue<TPuzzle> PossibleSquare;
        internal readonly ConstraintHeader<TPuzzle> Constraint;
        internal SquareLink<TPuzzle> Left { get; set; }
        internal SquareLink<TPuzzle> Right { get; set; }
        internal SquareLink<TPuzzle> Up { get; set; }
        internal SquareLink<TPuzzle> Down { get; set; }

        private SquareLink(PossibleSquareValue<TPuzzle> possibleSquare, ConstraintHeader<TPuzzle> constraint)
        {
            PossibleSquare = possibleSquare;
            Constraint = constraint;
            Up = Down = Right = Left = this;
        }

        internal static SquareLink<TPuzzle> CreateConnectedLink(PossibleSquareValue<TPuzzle> possibleSquare, ConstraintHeader<TPuzzle> header)
        {
            var squareLink = new SquareLink<TPuzzle>(possibleSquare, header);
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
