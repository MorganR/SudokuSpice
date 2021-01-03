# Custom Constraints

Let's continue the [custom rules example](custom-rules.md), where we want to solve a puzzle that also enforces
that the diagonals contain all unique values. In this case, however, we'll solve this by adding a
new constraint: the
[`DiagonalUniquenessConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.DiagonalUniquenessConstraint).

If you haven't yet read the [constraints summary](framework.md#important-concepts), read that first!

## Creating a constraint

A single constraint object can be used to solve multiple puzzles. Generally speaking,
no work needs to be done in the constructor.

### The `TryConstrain` method

The `IConstraint.TryConstrain` operation adds new
[`ConstraintHeader`s](xref:SudokuSpice.ConstraintBased.ConstraintHeader) (and corresponding
links) to the given
[`ExactCoverMatrix`](xref:SudokuSpice.ConstraintBased.ExactCoverMatrix). It must also drop any
[`PossibleSquareValue`s](xref:SudokuSpice.ConstraintBased.PossibleSquareValue) that are now
impossible based on applying this constraint to the puzzle's preset values.

It should return false if the given puzzle cannot satisfy this constraint, else true.

#### Define your headers

The first thing we need to identify is what a constraint header looks like for this constraint.
We could state this constraint as follows:

> Each forward diagonal, and each backward diagonal, needs to contain all possible values.

We can implement this by adding a constraint header for each possible value on each diagonal.

#### Determine unique coordinates

The unique coordinates here are those on the forward diagonal, and those on the backward diagonal.
We can find all the coordinates on the forward diagonal as follows:

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverMatrix matrix)
{
    var forwardDiagonalCoordinates = new Coordinate[puzzle.Size];
    for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
    {
        forwardDiagonalCoordinates[row] = new Coordinate(row, col);
    }

    // TODO: Get backward diagonal coordinates.
}
```

#### Identify possible values

We need to identify which values are actually possible on each diagonal based on the preset
values. Within the constraint-based solver, we map a puzzle's possible values to zero-based indices
based on the order they are returne when we first call `puzzle.AllPossibleValues`. This mapping is
handled by the `ExactCoverMatrix`.

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverMatrix matrix)
{
    ...

    Span<bool> isConstraintSatisfiedAtIndex =
            stackalloc bool[matrix.AllPossibleValues.Length];
    // Zero the array since stackalloc does not guarantee this.
    isConstraintSatisfiedAtIndex.Clear();
    for (int i = 0; i < forwardDiagonalCoordinates.Length; i++)
    {
        var puzzleValue = puzzle[forwardDiagonalCoordinates[i]];
        if (puzzleValue.HasValue)
        {
            isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[puzzleValue.Value]] = true;
        }
    }

    // TODO: Check this for backward diagonal coordinates.
}
```

#### Add headers and drop impossible rows

Now we can iterate through each possible value on each diagonal, drop possible square values that
are no longer possible, and add constraint headers for the rest.

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverMatrix matrix)
{
    ...

    var squares = new Square?[forwardDiagonalCoordinates.Length];
    for (int i = 0; i < squares.Length; i++)
    {
        squares[i] = matrix.GetSquare(in forwardDiagonalCoordinates[i]);
    }
    for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
    {
        if (isConstraintSatisfiedAtIndex[valueIndex])
        {
			if (!ConstraintUtil.TryDropPossibleSquaresForValueIndex(squares, valueIndex))
			{
				return false;
			}
			continue;
        }
		if (!ConstraintUtil.TryAddConstraintHeadersForValueIndex(squares, valueIndex, matrix))
		{
			return false;
		}
    }

    // TODO: Add headers and drop rows for the backward diagonal.

    ...

    return true;
}
```

Note that here we made use of the
[`ConstraintUtil`](xref:SudokuSpice.ConstraintBased.Constraints.ConstraintUtil) to easily
add headers and drop rows. This provides a few useful functions for implementing constraints. In 
fact, we could have replaced all of this work for handling the forward diagonal with the following:

```csharp
Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
{
    coordinates[row] = new Coordinate(row, col);
}
return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, coordinates, matrix);
```
