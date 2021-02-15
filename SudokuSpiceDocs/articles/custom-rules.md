# Custom Rules

Let's say we want to solve a puzzle that also enforces that the diagonals contain all unique
values. In this case, we can still use the standard
[`PuzzleWithPossibleValues`](xref:SudokuSpice.RuleBased.PuzzleWithPossibleValues) to store the data,
but we need to add a custom rule. In this example, we'll go through the steps for implementing and
using the [`DiagonalUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.DiagonalUniquenessRule).

## Creating a rule

The new rule needs to extend the [`IRule`](xref:SudokuSpice.RuleBased.Rules.IRule), and we'll
need to have some way of tracking the set of values that are available in each diagonal. For this
we'll use the [`BitVector`](xref:SudokuSpice.BitVector) struct, which provides an
efficient set-like struct using the 32 bits of a `uint`.

### TryInit

When starting to solve a puzzle, the rule keeper will call `IRule.TryInit`. This is where we
setup our calls and perform initial checks against this rule.

Note: Rules are stateful, so a given rule should never be used to solve multiple puzzles at once,
or acted on from multiple threads.

```csharp
public class DiagonalUniquenessRule : IRule
{
    // The puzzle being solved
    private IReadOnlyPuzzle? _puzzle;
    // All the possible values for each diagonal
    private BitVector _allPossibleValues;
    // The current unset values on each diagonal
    private BitVector _unsetBackwardDiag;
    private BitVector _unsetForwardDiag;

    public bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
    {
        _puzzle = puzzle;
        _unsetBackwardDiag = _unsetForwardDiag = _allPossibleValues = allPossibleValues;
        // Iterate through the backward diagonal (like a backslash '\')
        for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
        {
            var val = puzzle[row, col];
            if (val.HasValue)
            {
                if (!_unsetBackwardDiag.IsBitSet(val.Value))
                {
                    // Puzzle has a duplicate value on this diagonal, so it already violates the
                    // rule.
                    return false;
                }
                _unsetBackwardDiag.UnsetBit(val.Value);
            }
        }
        
        // TODO: Validate the forward diagonal, and update _unsetForwardDiag accordingly.
    }
}
```

### GetPossibleValues

Now we need to implement the `IRule.GetPossibleValues` operation, which provides the possible
values for any square according to this rule. You might be wondering, what should we return if the
given coordinate is not on one of the diagonals? In that case, we should return *all* possible
values! We definitely don't want to return no possible values, because that would make any puzzle
with unset squares off the diagonal impossible to solve.

```csharp
public BitVector GetPossibleValues(in Coordinate c)
{
    if (_IsOnBackwardDiag(in c))
    {
        return _unsetBackwardDiag;
    } else if (_IsOnForwardDiag(in c))
    {
        return _unsetForwardDiag;
    } else
    {
        return _allUnset;
    }
}

private static bool _IsOnBackwardDiag(in Coordinate c)
{
    return c.Row == c.Column;
}

private bool _IsOnForwardDiag(in Coordinate c)
{
    return c.Column == _puzzle.Size - c.Row - 1;
}
```

### Update

Great, now we need to provide a way for the rule keeper to update this rule. When the rule keeper
wants to set a square's value, it will call `IRule.Update` with the location and new value.
It will also include a [`CoordinateTracker`](xref:SudokuSpice.CoordinateTracker), with which
this rule needs to track any coordinates whose possible values have changed.

```csharp
public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
{
    if (_IsOnBackwardDiag(in c))
    {
        // Remove this value from the list of possible values.
        _unsetBackwardDiag.UnsetBit(val);
        _AddUnsetFromBackwardDiag(in c, coordTracker);
    }
    // TODO: Handle an update that's on the forward diagonal.
}

private void _AddUnsetFromBackwardDiag(in Coordinate c, CoordinateTracker coordTracker)
{
    // Iterate along the backward diagonal, tracking the coordinates of any unset squares.
    for (int row = 0, col = 0; row < _puzzle.Size; row++, col++)
    {
        // Make sure to skip the square that is currently being updated! Its value is still unset.
        if ((row == c.Row && col == c.Column) || _puzzle[row, col].HasValue)
        {
            continue;
        }
        coordTracker.AddOrTrackIfUntracked(new Coordinate(row, col));
    }
}
```

### Revert

Ok, now what if that update needs to be reverted, for example if the solver made a wrong guess?
Let's implement the `IRule.Revert` methods. These should both perform roughly the same
changes: reverting the changes made during the `Update` method. However, in one we can skip
tracking the affected square's in the `CoordinateTracker`.

```csharp
public void Revert(in Coordinate c, int val)
{
    if (_IsOnBackwardDiag(in c))
    {
        _unsetBackwardDiag.SetBit(val);
    }
    // TODO: Handle the case when the square is on the forward diagonal
}

public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
{
    if (_IsOnBackwardDiag(in c))
    {
        _unsetBackwardDiag.SetBit(val);
        _AddUnsetFromBackwardDiag(in c, coordTracker);
    }
    // TODO: Handle the case when the square is on the forward diagonal
}
```

### CopyWithNewReference

Lastly, we must implement the `IRule.CopyWithNewReference` method to provide a deep copy for
some solver and generator methods. The given puzzle should be a copy of the current puzzle, if any,
else null.

```csharp
public IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
{
    return new DiagonalUniquenessRule(this, puzzle);
}

private DiagonalUniquenessRule(DiagonalUniquenessRule existing, IReadOnlyPuzzle? puzzle)
{
    _puzzle = puzzle;
    // BitVectors are structs, so they are copied on assignment.
    _unsetBackwardDiag = existing._unsetBackwardDiag;
    _unsetForwardDiag = existing._unsetForwardDiag;
    _allUnset = existing._allUnset;
}
```

## Using the new rule

Now let's assume we have a `puzzle` already that we want to solve with this rule. We can solve it
as follows:

```csharp
var solver = new PuzzleSolver(
    new DynamicRuleKeeper(
        new List<IRule>()
        {
            new StandardRules(puzzle, possibleValues.AllPossible),
            new DiagonalUniquenessRule(puzzle, possibleValues.AllPossible),
		}));
var solved = solver.Solve(puzzle);
```

Similarly, we can generate new puzzles that follow this rule as below:

```csharp
var generator = new PuzzleGenerator<Puzzle>(
    size => new Puzzle(size),
    new Solver(
		new DynamicRuleKeeper(
			new List<IRule>()
			{
				new StandardRules(puzzle, possibleValues.AllPossible),
				new DiagonalUniquenessRule(puzzle, possibleValues.AllPossible),
			})));
var puzzle = generator.Generate(size, numSetSquares, timeout);
```

Remember to include heuristics for best performance!