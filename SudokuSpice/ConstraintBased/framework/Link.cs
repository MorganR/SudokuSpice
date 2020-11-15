namespace SudokuSpice.ConstraintBased
{
    internal class Link<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        internal readonly PossibleValue<TPuzzle> PossibleSquare;
        internal readonly ConstraintHeader<TPuzzle> Constraint;
        internal Link<TPuzzle> Left { get; set; }
        internal Link<TPuzzle> Right { get; set; }
        internal Link<TPuzzle> Up { get; set; }
        internal Link<TPuzzle> Down { get; set; }

        private Link(PossibleValue<TPuzzle> possibleSquare, ConstraintHeader<TPuzzle> constraint)
        {
            PossibleSquare = possibleSquare;
            Constraint = constraint;
            Up = Down = Right = Left = this;
        }

        internal static Link<TPuzzle> CreateConnectedLink(PossibleValue<TPuzzle> possibleSquare, ConstraintHeader<TPuzzle> header)
        {
            var squareLink = new Link<TPuzzle>(possibleSquare, header);
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
